/*
This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.
*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Simitone.Client
{
    /// <summary>
    /// AUD-22 'aud22' — the native near-screen-edge rolloff law + the
    /// track-keyed channel semantics, decoded from the original PPC PEF and
    /// implemented in the port (full decode receipt:
    /// coordination/evidence/AUD-22/edge-law-decode.md).
    ///
    /// Layers:
    ///   A. EDGE MEASURE (hard, offline): VMEntity.NativeEdgeMeasure — the
    ///      native rectangular inner window X in [20%,80%] of width, Y in
    ///      [30% of height, bottom] (cBoxX::GetInstanceVolPan FLAT
    ///      0x2E1E80 @ 0x2E2014-98: margins W*20/100 / H*30/100, NO bottom
    ///      margin), edge = max(signed X-band distance, clamped Y
    ///      distance), exactly 0 inside.
    ///   B. EDGE VOL LAW (hard, offline): VMEntity.NativeEdgeVol through
    ///      the PRODUCTION composition TickSounds multiplies by —
    ///      NativeEdgeVol(NativeInstanceVol(zoom, att), att, edge): linear
    ///      fade of the above-floor part over 600 screen px past the
    ///      window, floor = 800 only for kSplStereo 0x2711; edge >= 600
    ///      lands exactly on the floor.
    ///   C. CORPUS TRACK GROUPS (hard, offline): the event names that share
    ///      one TRACK (the AUD-21 §(d) census: 49 name-groups) resolve to
    ///      the SAME Track object through the mounted corpus — the key the
    ///      new HITVM dedup uses. footstep_ash + footstep_trash (track
    ///      1954) is the pinned pair.
    ///   D. TRACK-KEYED DEDUP (live, degrades without a sound device): a
    ///      same-track two-name pair plays through ONE thread (no layering
    ///      — the native one-sound-object-per-track law); names on
    ///      different tracks DO fire separately; a same-name retrigger
    ///      keeps returning the same thread.
    /// </summary>
    public static partial class AutotestRunner
    {
        private static void CheckAud22()
        {
            try
            {
                bool ok = true;
                string info = "";

                var audio = FSO.Content.Content.Get().Audio;
                var evts = audio?.Events;

                // ---------- A. EDGE MEASURE (native rectangular window) ----------

                // 640x480 reference frame (any viewport: the law is
                // fractional on the window, absolute-px on the fade).
                // Window: X in [128, 512], Y in [144, 480].
                var edgeCases = new[]
                {
                    // sx, sy, W, H, want
                    new[] { 320, 240, 640, 480, 0 },    // center: inside
                    new[] { 128, 144, 640, 480, 0 },    // window top-left corner
                    new[] { 512, 480, 640, 480, 0 },    // window bottom-right corner
                    new[] { 511, 479, 640, 480, 0 },    // just inside
                    new[] { 513, 240, 640, 480, 1 },    // 1px right of the 80% line
                    new[] { 127, 240, 640, 480, 1 },    // 1px left of the 20% line
                    new[] { 320, 143, 640, 480, 1 },    // 1px above the 30% line
                    new[] { 320, 481, 640, 480, 1 },    // 1px below the bottom
                    new[] { 0,   240, 640, 480, 128 },  // left screen edge = 20% margin
                    new[] { 640, 240, 640, 480, 128 },  // right screen edge
                    new[] { 320, 0,   640, 480, 144 },  // top screen edge = 30% margin
                    new[] { 320, 1080, 640, 480, 600 }, // far below bottom
                    new[] { -200, 240, 640, 480, 328 }, // far left (signed distance)
                    new[] { 200, 100, 1280, 960, 188 }, // window scales with viewport (Y top 288)
                    new[] { 1030, 500, 1280, 960, 6 },  //   ... 80% of 1280 = 1024
                };
                bool edgeLaw = true;
                foreach (var c in edgeCases)
                {
                    int got = FSO.SimAntics.VMEntity.NativeEdgeMeasure(c[0], c[1], c[2], c[3]);
                    if (got != c[4])
                    {
                        edgeLaw = false;
                        Log("AUTOTEST aud22: edge measure (" + c[0] + "," + c[1] + "," + c[2] + "x" + c[3] + ") got " + got + " want " + c[4]);
                    }
                }
                info += " edgeMeasure=" + edgeLaw;
                if (!edgeLaw) ok = false;

                // ---------- B. EDGE VOL LAW (production composition) -------------

                // (zoom, att, edge) -> NativeEdgeVol(NativeInstanceVol(zoom,att),att,edge)
                // att 20: zoom3=1024, zoom2=744; the fade is the above-floor
                // part over 600px; att 0x2711: floor 800 everywhere below
                // zoom 3, and the fade never lands under 800.
                var volCases = new[]
                {
                    new[] { 3, 20,    0,   1024 },  // inside window: untouched
                    new[] { 3, 20,    1,   1022 },  // 1024*599/600 = 1022 (int trunc)
                    new[] { 3, 20,    300, 512 },
                    new[] { 3, 20,    599, 1 },
                    new[] { 3, 20,    600, 0 },     // at the fade end: the floor
                    new[] { 3, 20,    700, 0 },     // past it: stays the floor
                    new[] { 2, 20,    0,   744 },
                    new[] { 2, 20,    300, 372 },
                    new[] { 2, 20,    600, 0 },
                    new[] { 3, 10001, 0,   1024 },  // stereo at zoom 3: no floor needed
                    new[] { 3, 10001, 300, 912 },   // 800 + 224*300/600
                    new[] { 3, 10001, 600, 800 },   // fades TO the floor, never under
                    new[] { 3, 10001, 5000, 800 },
                    new[] { 2, 10001, 0,   800 },   // stereo zoom 2: the 0x2711 floor
                    new[] { 2, 10001, 300, 800 },   //   ... above-floor part is 0
                    new[] { 1, 0,     600, 0 },     // law is total at att 0 (TickSounds bypasses it instead)
                    new[] { 1, 100,   0,   0 },     // kSplQuiet clamp arm at zoom 1
                };
                bool volLaw = true;
                foreach (var c in volCases)
                {
                    int got = FSO.SimAntics.VMEntity.NativeEdgeVol(
                        FSO.SimAntics.VMEntity.NativeInstanceVol(c[0], (uint)c[1]), (uint)c[1], c[2]);
                    if (got != c[3])
                    {
                        volLaw = false;
                        Log("AUTOTEST aud22: edge vol zoom=" + c[0] + " att=" + c[1] + " edge=" + c[2] + " got " + got + " want " + c[3]);
                    }
                }
                info += " edgeVol=" + volLaw;
                if (!volLaw) ok = false;

                // ---------- C. CORPUS TRACK GROUPS --------------------------------

                bool corpus = true;
                var fails = new List<string>();
                Func<string, FSO.Content.Model.HITEventRegistration> regOf = delegate (string n)
                {
                    FSO.Content.Model.HITEventRegistration r;
                    if (evts == null || !evts.TryGetValue(n, out r)) return null;
                    return r;
                };
                Func<string, FSO.Files.HIT.Track> trkOf = delegate (string n)
                {
                    var r = regOf(n);
                    if (r == null || r.ResGroup == null) return null;
                    try { return audio.GetTrack(r.TrackID, r.TrackID, r.ResGroup); } catch { return null; }
                };

                // C1 the pinned pair: footstep_ash + footstep_trash -> ONE
                // track (native id 1954) -> ONE Track object.
                var ash = trkOf("footstep_ash");
                var trash = trkOf("footstep_trash");
                var hard = trkOf("footstep_hard");
                bool pairSame = ash != null && ReferenceEquals(ash, trash);
                bool trackId1954 = ash != null && ash.TrackID == 1954;
                bool controlDiff = ash != null && hard != null && !ReferenceEquals(ash, hard);
                if (!(pairSame && trackId1954 && controlDiff))
                {
                    corpus = false;
                    fails.Add("pair(same=" + pairSame + ",id1954=" + trackId1954 + ",ctrl=" + controlDiff + ")");
                }

                // C2 the shared-track census: event-name groups per
                // (ResGroup, TrackID) — at least 49 groups carry >= 2 names
                // (AUD-21 §(d) audit; observed count goes to the log).
                int sharedGroups = 0;
                try
                {
                    var byTrack = new Dictionary<string, HashSet<string>>();
                    if (evts != null)
                    {
                        foreach (var r in evts.Values)
                        {
                            if (r == null || r.ResGroup == null || r.TrackID == 0) continue;
                            var key = System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(r.ResGroup).ToString("x8") + ":" + r.TrackID;
                            HashSet<string> names;
                            if (!byTrack.TryGetValue(key, out names)) { names = new HashSet<string>(); byTrack[key] = names; }
                            names.Add(r.Name);
                        }
                    }
                    sharedGroups = byTrack.Values.Count(v => v.Count >= 2);
                }
                catch (Exception ec) { corpus = false; fails.Add("census EXC " + ec.GetType().Name); }
                if (sharedGroups < 49) { corpus = false; fails.Add("census=" + sharedGroups + "<49"); }
                info += " corpus(pair=" + pairSame + ",sharedGroups=" + sharedGroups + ")";
                if (!corpus) ok = false;

                // ---------- D. TRACK-KEYED DEDUP (live) ---------------------------

                var hit = FSO.HIT.HITVM.Get();
                if (hit == null) info += " dedup=skip(no-hitvm)";
                else if (FSO.HIT.HITVM.DISABLE_SOUND) info += " dedup=skip(no-device)";
                else
                {
                    bool dedup = true;
                    try
                    {
                        var tAsh = hit.PlaySoundEvent("footstep_ash");
                        var tTrash = hit.PlaySoundEvent("footstep_trash"); // same track 1954
                        var tAgain = hit.PlaySoundEvent("footstep_ash");   // same name retrigger
                        var tHard = hit.PlaySoundEvent("footstep_hard");   // different track
                        bool sameTrackSame = tAsh != null && ReferenceEquals(tAsh, tTrash);
                        bool retriggerSame = tAsh != null && ReferenceEquals(tAsh, tAgain);
                        bool diffTrackDiff = tAsh != null && tHard != null && !ReferenceEquals(tAsh, tHard);
                        if (!(sameTrackSame && retriggerSame && diffTrackDiff))
                        {
                            dedup = false;
                            Log("AUTOTEST aud22: dedup sameTrack=" + sameTrackSame + " retrigger=" + retriggerSame + " diffTrack=" + diffTrackDiff);
                        }
                        info += " dedup(sameTrack=" + sameTrackSame + ",retrigger=" + retriggerSame + ",diffTrack=" + diffTrackDiff + ")";
                        // cleanup: do not leak probe threads into later checks
                        if (tAsh != null) (tAsh as FSO.HIT.HITThread)?.Kill();
                        if (tHard != null) (tHard as FSO.HIT.HITThread)?.Kill();
                    }
                    catch (Exception ed)
                    {
                        dedup = false;
                        info += " dedup=EXC:" + ed.GetType().Name;
                    }
                    if (!dedup) ok = false;
                }

                if (fails.Count > 0)
                    Log("AUTOTEST aud22: section failures: " + string.Join(",", fails));

                Log("AUTOTEST aud22:" + info + (ok ? "" : " (FAILED)"));
                if (ok) { Pass("aud22"); } else { Fail("aud22"); }
            }
            catch (Exception e)
            {
                Log("AUTOTEST aud22 EXC " + e.GetType().Name + " " + e.Message + "\n" + e.StackTrace);
                Fail("aud22");
            }
        }
    }
}
