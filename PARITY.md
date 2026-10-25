# PARITY.md — 1:1 fidelity gap tracker

> Hash note (MAP-04, 2026-09-15): round rows citing parent `c027914` / FreeSO `46495bb3`
> predate the 2026-09-10 history squash — those hashes are local pre-squash history
> (CURRENT.md); the squashed baseline is 49bf0e3 / 978da2af (now 85522e7 / 4d4b3fe1 with the
> 2026-09-15 SAV-10/11/12 integration).

**Goal:** drive the game as close to 1:1 behavior with the original *The Sims* (2000) as feasible, verified against the original IFF data and the original PowerPC binary. Every claim is labelled with its source; nothing is asserted from live play unless a read/write trace proves it. IFF-literalism and binary decode are the verification standards.

## Where the game stands — v0.9.2+, 2026-10-09 (READ THIS FIRST)

This section is the current truth. Everything below it is the historical round record
(newest-first-ish through late September) — rows there may be superseded; where a row was
later overturned, corrections are noted in place or here.

**Verified working** (the default 156-check battery boots the real game headlessly and
exercises all of this on every change; `tools/run-autotest.sh`; opt-in batteries
`nbr06`, `nbr05ui`, `nghbtns`, `freewillwin`, `moodlaw`, `trv05`, `hdserve`, `homepark`
cover the decode-pinned laws; `ulpets` rides the default suite since EXP-15's
follow-up; **ENV-04 (2026-10-09): the two documented battery flakes are
DE-FLAKED — carseek's "wall-clock" FAIL was ulpets probe corruption (the
show window's G10=555 gate flipping employed residents' pd32 to the visitor
class when the ~7:2x guest-bring landed inside it — 100% correlation across
~15 logs), fixed probe-side with baseline-snapshot-scoped restores + a
red-flag tripwire (evidence/ENV-04/deflake.md); ulpets' show-flake was a
bare default-priority push starving at AttemptPush, fixed with the train-leg
idiom + a bounded watchdog. TWO consecutive 156/0 batteries on merged main
(c4bc550e4) — the suite is deterministic.**

- Boot → neighbourhood → lot → live play; saves and reload (byte-audited against the
  original record format); CAS on the original desktop law (portraits, personality
  allocation, family flow, delete/unspent confirms); original UI chrome end-to-end
  (toolbar, panels, dialogs, pie menus, cursors, fonts, boot/logo screens, live-animated
  neighbourhood screens incl. the nessie cheat); budget window on the native ledger law
  (byte-routed expense categories, 3-day history); camera/scrapbook; PIP + cutaway;
  censor mosaic (native mean+jitter law); speech balloons; tutorial lifecycle; motives
  and mood on the native curve law; relationships (threshold-25 mutual-friend law
  everywhere); base careers; buy/build with walls/floors undo; autonomy on the fully
  restored native gather-scoring law (per-candidate scoring, sentinel, descending pool,
  best-K draw, pool-score cutoff); Export HTML to the decoded template law; sounds
  infrastructure (4479-event corpus, live interpreter).

**Open — base game (the honest remainder):**

- ~~Neighbourhood management~~ **CLOSED 2026-10-08 (NBR-06)**: bulldoze
  (all four dialog arms, STR#131[0]/[1] + 132[4]/[5] + failure 131[10]/[11]),
  eviction-with-demolition (EvictFamily → MoveOut's house-file deletion), the
  same-click rezone chain, and the community build/buy entry + SAVE matrix
  (the port's save gate was inverted) are decoded, implemented and gated
  (`nbr06` opt-in, 9 phases; nghbtns/nbr05ui re-pinned). **Residuals
  CLOSED 2026-10-09 (NBR-07 + NBR-08)**: all four expansion-venue evict
  handlers DECODED + PORTED (Downtown/Studio occupied = native silent
  no-op; Vacation's [18]/[19] confirm ladder + sounds-only unbuilt leg +
  RemoveFromVacation executor; Magic = the UL net-worth law + the [14]/[15]
  venue pair; the [20] "visiting" string corrected to the import flow), the
  zone-choice strings (STR# 250/251) decoded + wired, and the hide-slot
  pairing resolved (native BUILD+CAMERA / BUY+LIVE drives; port pairing
  equivalent on the correlated visit-lock path, disclosed). NBR-08 mounts
  the zone-choice DIALOG itself + the venue/base bulldoze sound bracket
  (`nbr07` gate). Remaining: evict web-export (ENG-27 scope), the Magic
  [16]/[17] vacation pre-scan (needs port vacation state), the +0x100/+0x1a8
  hide elements (no port counterpart).
- ~~Sound trace~~ **ROUND 1 DONE (AUD-19)** + **FOOTSTEPS CLOSED (AUD-20,
  2026-10-08)**: the footstep class is live on the decoded native law —
  authored anim-event triggers (±1/±2 forward/backward), the native gate
  order, the surface classifier (floor sound class from the SPR2 label
  first letter M/H/S/C), the 12-case name jumptable (14 live names wired;
  `footstep_terrain_noshoe` stays corpus-dead) and the barefoot outfits
  {1,5,10,14}. The TS1 zoom-volume byte-law `(1024−(3−zoom)·280)/1024` is
  implemented (TS1-guarded, TSO path unchanged). Round 1 also fixed the CAS
  personality-deny sound and cross-level attenuation (×3/5). Residuals
  (disclosed): the UI-button down-vs-up firing law is decoded+banked but
  NOT implemented (protected-scale UI change); nhood-terrain snow branch
  unmapped (no port terrain-type field); pool/deck markers mapped from port
  floor-grid sentinels; HITVM dedup vs native per-footfall replay (marginal
  at step cadence); gated listen-run on real hardware. **The down-vs-up firing
  law CLOSED 2026-10-09 (the owner's closeout directive lifted the gate):
  button sounds fire on mouse-DOWN from PlayUISound's per-button slots
  (default EMPTY = silent); the UL filter bar carries 'UI_Nhood_click'
  (pool 0x6c6e0+0xe, byte-proven); the blanket up-click removed (several
  surfaces that double-played now sound once). **Wave-2 (2026-10-09
  subagent decode, evidence/AUD-20/filterbar-override-table.md): the ODUIS
  NULL-fallback built-ins are {"", UI_click, "", UI_error} — the BASE press
  default is ui_click, not silence (silence is per-button only); all FIVE
  filter bars pass the identical UI_Nhood_click; community view windows
  install it as their class press default while mounted; the lot button
  carries UI_Nhood_rollover on the hover slot (the only native non-press
  override) with its click on the down edge; the CAS cycle arrows use
  UI_CAC_CycleParts/UI_CAC_Cyclehead press slots — all ported + gated.**
  The footstep snow/99 branch closed on the lot-id decode
  (footstep-residuals-decode.md: the class-0 default reads cSimulator global
  #10 = the CURRENT LOT — 40-42 snow, 99 soft; ported with 8 lot-cases); the
  0x620&8 suppressor is a DEAD GATE (zero writers corpus-wide — the note
  retires). **kSpl CLOSED 2026-10-09 (AUD-21): the per-track attenuation law
  PORTED — the [Track] kSpl column (speaker placement) is parsed and the
  TS1 volume law is att-driven per track (`0x400 − (3−zoom)·att·0xE`, stereo
  0x2711 floors at 800, att 0 = stings/horns/ambience with NO zoom/edge/pan
  attenuation; TV channels 10, radio stations 0x2711; every footstep = the
  20 default, no-change asserted). Runtime census matches the decode
  digit-for-digit ({0:51, 10:9, 20:4236, 0x2711:9}); review ACCEPT-WITH-NOTES
  (receipt evidence/AUD-21/kspl-attenuation-port.md). Channel-sharing stays
  disclosed as behaviorally-equivalent (the port's dedup is name-global ≈ the
  native's per-track unit; 49 corpus name-groups sharing one track can layer;
  SetVolume is max-wins vs the native instance-map recompute — both
  marginal). Receipts:
  evidence/AUD-20/button-firing-law.md + footstep-residuals-decode.md.**
- ~~Custom animated objects~~ **CLOSED 2026-10-08 (CC-06)**: five defects
  fixed on decoded native laws — duplicate-skeleton registration threw
  (native silent-replace law), corrupt and hostile custom CFPs crashed the
  game outright (now skip+report on the native decoder semantics,
  ReadNFloats restructured 1:1), untranslated animations and unresolvable
  skeletons no longer NRE the renderer. Gated by `cc06` in the default
  suite (synthetic fixture through the real providers). Residuals:
  duplicate-winner order is enumeration-dependent (the fixed law is
  order-independent); UI-preview animation consumers unexercised.
- ~~UI residuals~~ **LARGELY CLOSED (UI-37)** and **MONTAGE + UL TOOLBAR
  CLOSED (UI-38, 2026-10-08)**: phonebook/help columns scroll on the DECODED
  animated law (ScrollTo pool + 200ms autorepeat); view-pie cell ladder +
  magnitude + commit clamps decoded; the UL community filter-toolbar is
  mounted live on the decoded cWinNeighborhoodUL law (strip 5045 as the
  community screen's top chrome — no navbar on the original community view;
  7-button engine-literal ladder, STR#171 labels, click/highlight/plaque
  laws; `uidtbar` gate); the montage's SpriteSlot water positions are
  decoded closed (UL waves = lot-layer window image at (0,0); TS1.0 deltas
  = buffer blit with damage bands 375/565..800 × 400..600, mod-4 hold) —
  the port's mounts were already engine-exact. Remaining: ~~the view-pie pop
  gesture (re-proved vtable-unrecoverable — permanent disclosure)~~
  **SUPERSEDED (UI-41, 2026-10-25)**: the "vtable-unrecoverable" premise is
  retired — the binary's CodeWarrior traceback tables name every function
  (tools/pef-reloc/tb_names.py, 11,896 entries) and the pie vtable is
  resolved (data 0x75d00, pool-pointer entries, 0x209b50 call glue). The
  pop is COMMAND-DRIVEN and MODAL (TSOnCommand cmd 13 / sender==pie → the
  UpdateViewMenu driver direction 0 → the pie Popup family); there is NO
  drag/tear-off gesture in the binary. Port trigger corrected to HOLD-DWELL
  (DwellMs=250, a disclosed model constant — the native time constant sits
  in the undecoded WM-queue post, the one remaining hop; drag retained as
  secondary arm; 3D hold-steering excluded). `uiviewpie` re-pinned with the
  dwell truth table. UI-38b
  CLOSED the two unbanked filter laws (2026-10-09): **filter persistence**
  on the native LoadCurrentFilter law (STR# 6 'Filter Bar Settings' slot 3
  in LotLocations.iff — byte-proven; entry applies the 0x10 default then
  restores the persisted cmd; every filter change saves atomically and
  syncs the cached provider instance) and the **production lot-category
  bits** (the native lot+0x1ac writer remains unlocated — re-verified
  across every direct stw site; the port computes per-lot bits from the
  lot's own house-file OBJT census against a 74-entry corpus-derived
  venue-marker table, gated on the live n=1 census: 58=0x46 61=0x6e
  70=0x7c 71=0x2e 72=0x04 73=0x46 74=0x26 75=0x2e — six of seven buttons
  visible). Receipt: evidence/UI-38/ui38b-filter-laws.md). +0x1dc CLOSED
  2026-10-09: the plaque ANIMATES — a 3-frame ping-pong (0→F0, 1→F1,
  2|3→F2, 4→F1, 5→F0, byte-verified at DrawFilterAt) in lockstep with the
  wave counter's advance gate; ported + gated in uidtbar (receipt:
  evidence/UI-38/plaque-pingpong-law.md). **+0x108 CLOSED (subagent decode,
  evidence/UI-38/plus108-and-ts1-cadence.md): it IS the show_zones cheat
  flag (the UL CheatCallback's only runtime writer, ctor-zeroed, bare form
  arg-count-gated) — now a working cheat in the port (show_zones on/off
  re-routes the lot highlighting; gated incl. the off + bare no-op arms).
  The TS1.0 discrete-cadence residual CLOSED: 200 ms/step, counter mod 4
  with slot-0 SKIP (visible cycle f1,f2,f3,skip — period 800 ms; the 2^52
  magic correction), mounted via the SkipSlots capability + gated. UI-38's
  residual list is now EMPTY.** The **Quick Tips presenter is CLOSED by UI-39
  (2026-10-10)**: the TS1QuickTips hover-dwell help-popup layer on the original
  options panel — native cOptionsMgr::SetQuickTips / cTSWinBtn arm+pump law,
  the 250 ms dwell, the same-button toggle, and the live
  `GlobalSettings.TS1QuickTips` gate; `ui39` gate in the default suite
  (receipt evidence/UI-39/quicktips-decode.md). Its named bounded follow-up is
  **CLOSED by UI-39b (2026-10-12)**: the camera-panel surface — the STR# 140
  [16]/[17] 'Snapshot Sizes' + [18]/[19] 'Quality Settings' group popups with
  the kCamPopupDetail/kCamPopupQualDetail (0x455/0x456) art, through the same
  cWinLivePopup presenter (UIOptionAboutPopup); the eight camera button
  one-liners remain tooltips (not popup triples). The remaining per-surface
  registrations (People/subpanel/catalog/roof tip triples — presenter + law
  now exist) stay enumerated on the UI-39 card.
- ~~**Disclosed (visit-session park, 2026-10-08)**: away-lot visit sessions load in
  BUY mode + paused per the port's own NBR-06 native entry decode
  (`cSimsApp::LoadGame` visit → SetMode(BUY)+Pause) and the HUD panel re-parks
  the clock until the player enters LIVE — until then the whole lot (including
  NPC townsfolk) is frozen, where the native's visit streets simulate. The
  autotest probes apply the V4.2 "livelift" (panel → LIVE at rebound); a
  player entering LIVE sees normal simulation. Bounded engine-side follow-up:
  keep townies ticking during the visit park.~~ **CLOSED (ENG-28, 2026-10-09):
  the native restricted-tick law decoded and ported.** The native park NEVER
  stops the object pump — `cDDDSimsView::Simulate` 0x1020e000 →
  `cSimulator::Simulate` 0x1012fb80 (no pause/mode gate) → `SimulateOneTick`
  0x10130770 every pulse; pause (+0x32) / non-LIVE mode (the CPState mirror
  +0x36, `UpdateWorldFromCPState` 0x10208790: BUY→2/LIVE→0) restrict INSIDE
  `TickAllObjects` 0x10130570 — per-object Simulate (→ `cXObject::Simulate`
  0x100e7e70 → `TreeSim::Simulate` 0x1014a900, per pass, tick-counter-blind)
  keeps running (SetSimFlag enum-2 = obj+0xc2 ≥ 0, only ever zeroed ⇒
  effectively universal), and ONLY the clock/counter freezes
  (minute/hour/day + 0x98; motives are clock-minute gated). Ported scoped to
  TS1 visit sessions (`VMTS1LotState.VisitSession` = the NativeEntryVisit
  matrix): `VM.InternalTick` falls through the park to the object pass with
  the clock gate intact; `VMServerDriver.Tick` advances TickID on the −1 park
  so the scheduler's wakes fire (native scheduling is pass-driven — TryIdle
  0x100e8530). Focused magicbook-ALONE — the standing FAIL class — now PASSES
  with zero probe change; petname PASS with the new VISIT-PARK-NPC-TICKS
  evidence (schedAdv=80, npcDelta, clockFrozen). **HOME-LOT BUY PARK CLOSED
  (ENG-28b, 2026-10-09)**: the −1 park now falls through + advances TickID on
  EVERY TS1 lot, gated on `VM.PrimingTick` (the load-time priming pass keeps
  its non-ticking semantics — without the gate the arrival choreography
  stalls deterministically, hdserve bisect-proven). `homepark` gate (lot-21
  buy soak: schedAdv + street-liveness + clock frozen) PASS 3/3. The
  EXPLICIT-PAUSE (speed 0) half CLOSED 2026-10-09 on the newly decoded
  DIALOG-PARK LAW: SetBlockSimulator (flat 0x209ae0) stores view+0x111 and
  cDDDSimsView::Simulate gates its cSimulator::Simulate call on it — a
  blocking modal STOPS the pump entirely, while the pause button (+0x32)
  only restricts inside the tick. The port splits the conflated speed-0:
  PauseButtonPark (button/focus parks ride the restricted law incl. TickID)
  + ModalPumpBlock (UIModalSimPause windows stop the object pass at ANY
  park). homepark pause/modal legs + homeparkfam (lot-5 family soak,
  dialogs answered — the family buy-soak measurement residual CLOSED) all
  PASS; hdserve holds (the retraction's stall class keeps its freeze).
  Receipt: evidence/ENG-28/dialog-park-law.md. **The non-vacation travel-edge transit residual CLOSED on the cab-chain
  decode (2026-10-09 subagent, evidence/ENG-28/cab-chain-decode.md): ALL
  FIVE travel plugins park their persisted transit in global.iff 281 before
  sims_call mode 17 (downtown 0xA6F31853, vacation 0xABA9DF4A, community
  0xEAA79D86, studio 0xC61F8102, magic 0x99197314 — the 281-frame
  requirement excludes the parkless portals); natively NO BHAV frame
  survives a lot switch (LoadHouse clears histories on every load) — the
  residue is a port construction, now completed at every travel edge
  (CompleteStaleTransit's callee predicate = the five-GUID set, armed at
  every home return).** The same lane fixed the stale return-transit
  ENGINE-side: at the vacation-return arrival, the persisted transit action
  parked in global 281 'Wait For Notify' is completed (player-cancel law;
  the native arrival clears trip residue — RemoveFromVacation
  ClearHistory/ClearRouteHistory); `trv05` passes with zero probe
  intervention (the former lifelift removed). Receipt:
  evidence/ENG-28/eng28b-restricted-tick.md.
- Platform: `-3d` is experimental; `-jit`/`-dx` are inherited and unverified; trackpad
  natural-scroll and other hardware feel items lack systematic human verification —
  **human verification is partial** (portions played and accepted hands-on; no
  systematic end-to-end pass yet). Apple Silicon only; no Intel build.

**Open — expansions (substantial parts landed; completeness is the gap, not presence):**

- Hot Date/Downtown: travel, shopping, restaurants, interests, downtown NPCs verified
  live. The serve-choreography SELF-START law is DECODED (EXP-14, 2026-10-08:
  podium-anchored autonomous dining — the Eat interaction itself creates
  Controller-Restaurant-Eat; the hunger-gate hypothesis CONFIRMED as the ad/curve
  law; the port's freewill law proven faithful on zoning-0 lots) and the
  self-start has been OBSERVED live (an autonomous eat-family engagement with
  zero pushes, from-dist). **The away-lot traveler grant landed the same day**:
  single-traveler builds (TS1 call modes 18/26) restored the traveller with the
  NPC person-class and zero autonomy — the record-restore computes the class
  against a stale family word and the pd36=50 person-init only runs on home
  lots; both are now granted at build (the same law family as the Unleashed
  pet-class fix). **CLOSED (residual round, 2026-10-08)**: the `hdserve`
  opt-in gate PASSES — the adult traveler's own free-will gather draws the
  podium's 'Eat Alone' row (the hunger curve lawfully outscores the bar's fun
  rows at low hunger; the earlier bar win was a non-hungry NPC — lawful),
  the interaction's op-42 creates Controller-Restaurant-Eat, and the meal
  engages (SERVE-SELF-START-PROVEN). One more real engine defect fixed on
  Ghidra decode: VMTestObjectType returned ERROR on a null StackObject
  where the native (op-32, TryElement table 0x10601898) reports error 0x17
  and CONTINUES — the podium's test chain died on the no-controller path.
  Bounded lawful residuals: child diners never draw podium rows 0/2
  (TS1NoChild); stratum-1 travelers are distance-capped (7.0).
- Unleashed: pets are live (adoption, naming, transforms incl. the dragon, nests);
  **pet-AI depth root-caused (EXP-15, 2026-10-08): two decoded port defects
  fixed** — family pets carried the NPC person-class 2 (which Cat/DogGlobals
  8200's eligibility gate rejects as an owner-interaction target, hiding Call
  Over/Scold/Praise/toys/Play/Tricks/Train) and pet avatars lacked the pd[65]
  species bits the TTAB gates read; shows/training were found largely
  receipt-closed already (EXP-05 unl-show/unl-mice, EXP-09 exp09train).
  **CLOSED (follow-up, 2026-10-08)**: the record-restore class path and the
  probe soak-release are fixed and `ulpets` rides the DEFAULT suite green
  from dist (156/0; the owner pie rows, the training push, and the show
  pedestal all live).
- Vacation: **ALL FOUR AREAS CLOSED (TRV-05/TRV-06, 2026-10-08)**. Save-on-
  vacation + bookings on the decoded native law (Family::DoStream field-8
  rental +0x13C in the version-9 shape, the SaveGame matrix — vacation lot
  saves house only, booking persists at arrival and clears on return — and
  mode 27 corrected to the true rental-occupied loop; shipped-original
  byte-identity censused). **Score + souvenirs live-verified (TRV-06)** on
  the `trv05` opt-in gate: the per-family score chain (Controller - Vacation
  - Score attr[5]/attr[6] tag; type-6 score token 0x842A8C3B spawned/restored
  by 4126; Process Score 4104 accumulation; the 4108 threshold — BCON tuning
  indices are 0-BASED at runtime, Tuning[3]=150, correcting the TRV-05
  receipt — and the 8547 award check via RunTreeByName), souvenir purchase
  (CT 4116: type-5 base + mood-gated type-6 good/bad variants, 17
  base→variant pairs byte-pinned), and carry-home (4106/4107 op-42-create
  BOTH bases at the byte-matched tree sites, tokens consumed). Two engine
  fixes landed with it: the VMFindLocationFor null-object law (a missing
  stack/reference object is the native FALSE branch, not an NRE — the
  exception had killed VacationPedMarker 4128 'Main - Family Spawn', so
  vacation families never materialized on template rentals) and the
  transit-seeding/visit-lifelift probe levers (disclosed, each citing its
  law). **Souvenir persistence CLOSED (TRV-06b, 2026-10-09)**: the "~2-tick
  vanish" was an execution-model artifact, not an object-lifecycle defect —
  the spawn trees must run as a REAL queued interaction (4104's op-50
  change_action_string needs a queue item; global 'do the drop' 313 ins8
  goto_routing_slot needs a walkable thread — sync check-context drives
  lawfully short-circuit routing and the drop idiom's give-up path destroys
  the held base at 313@31). The actual port gap was the missing return-edge
  push of the umbrella: TS1GameScreen now enqueues 4104 on each token-carrying
  member at the vacation-return booking edge (the port's representation of the
  native RemoveFromVacation→ObjectModule broadcast push site, disclosed).
  `trv05` gates it on the automatic engine-push path: BOTH stocked bases
  placed in-world (un-contained) + the 240-frame persistence soak + both
  token sets consumed (trv06b-persistence.md + trv06c-both-bases.md).
  **Second-souvenir continuation CLOSED (TRV-06c, 2026-10-09) — a verdict
  artifact, not an engine defect**: the baseEntities dump proved the second
  base alive, still in-hand, mid-put-away when the probe's queue-name
  completion signal fired early (after the engine's transit cancel the
  queue reads empty while the spawn chain still executes on the thread
  stack). The gate now waits for true stack drain; both souvenirs carry
  home end-to-end (PASS ×4). The stale 'Go Home' transit starvation is
  fixed ENGINE-side (ENG-28b lane B: completed at the arrival edge).
- Superstar: fame promote/demote and session machinery verified live;
  **fame screens + objects + gendered job titles CLOSED (EXP-16,
  2026-10-08)** — the gendered title law (career STR block layout +
  cJob::GetName(bool)) wired into every consumer (job panel, dialog tokens,
  web export; also fixed the TSO-remnant level*3+4 desc index), fame
  screens reconciled against cWinSubpanelFame (the once-disclosed
  IsFriendsNeededShown TOC float pair was decoded by EXP-16b: K = 0.5f,
  port law exact), and the 47-file Studio Town object census found zero missing
  interactions (1125 interactions, 0 unresolved trees; `ssfame` gate).
  ~~Residuals: EXP-11's award-ceremony choreography blockers (BHAV-level),
  home fame-decay self-start.~~ **CORRECTED 2026-10-09: EXP-11 is TERMINAL
  (its acceptance map closed every row 2026-10-03 — V4 home fame-decay and
  V5 award-ceremony CLOSED-BY-DISPOSITION at the controller-idle boundary:
  a CONFIRMED cross-cutting IFF-native scheduler law (global-280 long-phase
  idle) that executes in real play; every live primitive mechanically proven
  across runs 1-14; V6 save/reload CLOSED LIVE PASS 5/0). The fame-decay
  law itself is DECODED and the controller mounts (ss census); nothing
  remains implementable without native-side changes.** No remaining
  Superstar residual: the fame-screen TOC float pair was decoded by
  EXP-16b (IsFriendsNeededShown K = 0.5f; port law confirmed exact).
- Makin' Magic: the major systems are verified live (spells/recipes via the
  mode-38 primitive, duels, nectar, dragon, spellbound, family spells, Magic
  Town transit); **quest-line coverage CLOSED (EXP-17, 2026-10-08)**: the quest
  lines enumerated from the IFFs (quest-giver set + Choose Quest/Choose Reward
  core), two genuine engine gaps fixed on native decodes (the
  `$TokenNameLocal:` dialog substitution; the FindToken index-dst selector
  `(Flags2.0x18)>>3`, superseding ENG-06's zero-caller guess), everything else
  proven live-by-construction; `mmquest` rides the default suite (PASS from
  dist). Bounded residuals enumerated on the card (delivery-quest fixture,
  zero-caller selector/native substitutions, the lot-93 headless NPC-dialog
  pause storm — a harness limitation, banked).

**Deliberate divergences (documented, not accidents):** ~~the malformed
GenericCall-13 path throws instead of running the native destructive
cleanup (unused by the owned corpus)~~ **RETIRED (SIM-21, 2026-10-24, owner
directive to reconcile the REL-10 disclosures):** the malformed path now runs
the full native `CleanupPeople(module, null)` law — error-10 report, persons
flush queues, every non-person object runs its entry-point-3 check tree and
deregisters (scheduler-safe end-of-frame), conditional tutorial-pointer
clear; `g13clean` in the default suite. Web export
still omits the dead TheSims.com teleport packages (`faminfo` .pub/.ini) and the
`_Sim_Lane` path fragment is unresolved (the gendered-job-titles omission was RETIRED
by EXP-16 — titles now export on the native law).

**Corrections to earlier rows in this file (decode superseded decode):**

- **R250's "score law" is REFUTED** (AUD-18, 2026-10-08, decompiler call-site proof):
  GetInteractionScore runs INSIDE the per-candidate gather loop; the winner is NOT
  re-scored at hand-off. The engine now implements per-candidate scoring with the
  sentinel drop, the FCNS min-autonomy push gate, a DESCENDING pool (the comparator
  polarity was transposed in the first landing), a uniform draw over the K best, and the
  cutoff on the winner's pool score. Live pool observed sorting best-first; focused
  `freewillwin` gate green.
- **R251's mood Room weight "=1 disclosed default" is SUPERSEDED** (ORIG-02 + AUD-18):
  the Room weight is HappyWeightCurves entry 5 (the (−100;2)(0;1)(100;2) curve), the
  participant set excludes Energy, and the curve↔motive pairing follows the builder
  index array {7,6,8,9,15,13,14}; cadence 60 time-units confirmed. `moodlaw` gate green.
- The friend threshold is **25** on every surface (UCP count, relationship panel, fame
  star power) — one shared FloatConstants global; the earlier 50 readings were misreads.

## 2026-09-20/21 integration waves (zcode-uiwave-20260920a + zcode-uiwave2-20260921a/b; ledger sweep 2026-09-21)

Two coordinator waves landed on the combined revision parent `05ea938` / engine `a4b926dc`; full default suite FROM DIST **141/0** incl. uibrand (receipts evidence/COORD/integ-20260921/ + integ-20260921-wave2/; independent fresh-context reviews in evidence/COORD/indep-review-*). Product changes, by card:

- **UI-18 floor-button gate** — the Lev2/2nd-floor gate decoded as cFixedWorld WORLD state (story-2 content + support, recomputed by ComputeRooms), not a camera-visit latch; gate now derived per frame from the architecture, latch removed (`uicp` lev2-live pin; r254-lev2-gate-law.md).
- **CAS-02 preview framing + name editor** — preview follows the decoded cWinVitaBtn UpdateTransform law (root anchor (50,200), Near scale, look Z 1.1482; pixel-identical bounds at 800x600 and 1024x768); single-line name editor live (LF never enters the buffer, Return=Done, cap 25/16 lang-15). Both CAS residuals CLOSED.
- **UI-21 FAM import UI + Export/re-export** — R248's decoded UI landed: neighborhood Import button → STR# 143 dialog → production consumer; lot-query Export button + Export/ re-export mirror; auto-import restricted to the native Tutorial.FAM-only law (`importui` 52-check + `impexport` PASS).
- **UI-22 build-mode undo/redo, tranche 1 — CONDITIONS CLOSED 2026-09-22** — walls/floors undo/redo wired to VMArchitectureUndoStack (cap 100, story snapshots, money hooks; ArchUndo=50; UIOriginalArchChrome buttons un-disabled + refresh law); the four remaining laws (undo-refund, redo-replay, refresh, clear-on-exit) re-proven live (archundo 0-8 PASS, evidence/ENG-02/archundo-integrated-20260922.log). Terrain/objects are later tranches.
- **UI-23 people sub-pie PORTED** — two-ring sub-pie on the r257 law (ring 2 = byte-identical second cTSPieMenu instance, item-0 Back affordance, ESC steps sub→main then full CancelPieMenu; census split 8|overflow). R191's "sub-pie" residual CLOSED.
- **UI-25 cutaway orphan fixes** — AdjustForDrag inputs wired from the held footprint (null in PIP); PIP passes null inputs on the same floor, wipes cut history cross-floor; target-floor picker line landed; rotation-LUT doc aligned (pure 3-page BuildRotationLookup map).
- **UI-26 options WIRE tranche** — Lighting, CharacterDetail, SimInBackground, Shadows live-wired; the OPTIONS dummy category DELETED (plaque hidden on every platform). Remaining R121 rows are named BUILD follow-ups (Quick Tips CLOSED by UI-39 2026-10-10 + camera-panel surface UI-39b 2026-10-12; Export HTML at save; grass/TerrainDetail); Interface FX CLOSED by UI-40.
- **UI-27 relationship filters + grade letters** — the R159 filter residual RESOLVED on the decoded law (no stored flags, computed predicates: friends = mutual RelMatrix slot-0 >= 50 both directions; famous = persisted PD81 != 0); grade letters landed (work.iff STR# 4097 GetString; >= 10 flashes; GetCareer==0 blanks) — Tier A item 7's disclosure CLOSED.
- **UI-30 Credits screen + armed Bulldoze** — the cWinCredits overlay ported on the decoded timeline law (incl. the Mac STR# 10000 text), entered from the navbar Credits button AND the banner credits-picker; Bulldoze ARMS a mode with the decoded confirm/branch matrix through the real MoveOut/BulldozeLot backends (occupied killSims second confirm included).
- **ENG-02 RESOLVED AS NO-DEFECT (2026-09-22, review-confirmed)** — the "DEFECT-2 deterministic freeze" was a MISREAD SIGNAL: HelpSystem.iff's `main` tree (ent=307, ip10, opcode 36 = dialog_private) latches GlobalBlockingDialog at tick 3 -> SpeedMultiplier=-2 -> a legitimately PAUSED VM (zero entity ticks -> frozen watchdog heartbeat; lldb-confirmed). The old "hard-freeze at SetMode" was the archundo gate's own frame-0 guard bug (fixed 09a626f); the watchdog now PRINTS VM PAUSE STATE (bb5eaf13). EXP-03's HD-window spin lineage stays open.
- **UI-19 / R237 supersession** — the R237 Budget/Help/Phonebook failure list was STALE (repaired by the R238-era restorations inside the squashed baseline); closure independently verified (live gate PASS 9/0 + receipts, evidence/COORD/ui19-closure-verify-20260921.md). R237 section below carries the supersession note — kept.

### TOOLING ERRATUM — ppc_decode.py conditional branches print INVERTED

`tools/iff-dump/ppc_decode.py` keys B-form branch mnemonics on BO alone (prints BO=12 as `bne`, BO=4 as `beq`), while the real PPC law is **BO=12 = branch-if-TRUE (beq family) and BO=4 = branch-if-FALSE (bne family)**, BI picking the CR bit (0=LT,1=GT,2=EQ,3=SO); opcode 8 (`subfic`) is missing from the decoder entirely. Found INDEPENDENTLY TWICE (r258/UI-24 and r261/UI-27). Authoritative notes: [`tools/iff-dump/r261-relgrade/r261-relgrade-laws.md`](tools/iff-dump/r261-relgrade/r261-relgrade-laws.md) §0, [`tools/iff-dump/cas02-casframe/frame-law.md`](tools/iff-dump/cas02-casframe/frame-law.md) §0, [`tools/iff-dump/r258-pef-revisit/`](tools/iff-dump/r258-pef-revisit/). **SWEEP COMPLETE (UI-34, 2026-09-29):** the blanket re-audit this erratum promised is DONE — the UI-34 sweep record classifies every conclusion in all 26 citing docs (~20 rounds with inline PARITY corrections + the three authoritative notes + the seven remaining docs conclusion-by-conclusion): every branch-sense-dependent item is runtime-gate-pinned, corrected post-fix, or immune arithmetic; UNCOVERED = empty (two benign unused doc-level residues named there).

## R248 FAM import consumer

The tutorial reset loop is complete on the decoded import law (EXPi/uChr/Gtab/FINV codecs; eviction with net-worth preservation; character files rebuilt from NBRS/uChr through the template-person path with the corrected RelMatrix carry; the FAM atomically becomes Houses/HouseNN.iff; the tutorial latch is set; the neighborhood saves immediately), and the neighborhood poll auto-imports a staged `Tutorial.FAM` per the native gate. The adversarial review caught and repaired a P0 (a shallow template snapshot that would have corrupted every future CAS character after one import) and two P1s (non-atomic Gtab splice; uncontained file deletion). Record: [`tools/iff-dump/r248/round.md`](tools/iff-dump/r248/round.md).

## R250 free-will winners serve

> **REFUTED 2026-10-08 (AUD-18 decompiler audit — see "Where the game stands"):** the "score exists only post-draw" adjudication below was a misread of the call sites — GetInteractionScore runs per candidate INSIDE the gather loop; there is no hand-off re-test. The R249 "serving residual" is retired: the drops were native-identical **guard failures** (UrnStone Mourn window, phone ring latch, magic tokens — the battery's "chair-sit" label was a misclassification), and the port's slot/routing serving was already complete. `freewillwin` upgraded to a hard served-winner pass (4/4 served, identity+priority matched, zero drops). Record: [`tools/iff-dump/r250/round.md`](tools/iff-dump/r250/round.md).

## R249 free-will winner priority

Autonomous winner selection follows the original's own law, implemented on the TS1 path (the TSO path is byte-identical to before): the native "Find Best Action" primitive decoded end-to-end — the target-conditioned actor gate and native gender-class encoding, **mean-of-9 motive-curve scoring with FCNS attenuation**, the original's **broken-sort quirk (a heapsort whose one-sided comparator net-rotates the candidate array by one)**, the **uniform top-K draw with K = count − LogicSkill/2000**, and the **sitting cutoff 1e-6**. The round also fixed genuine engine bugs the rebuild exposed: **MotiveTabInv 13/23 rows**, TS1 unknown-opcode must fall through ('try autonomy' depends on it), a check-yield law now explicit, the port's gender encoding vs the native species encoding, and a **save-restore clobber of the free-will mirror**. The opt-in `freewillwin` check observes real multi-candidate decisions and proves the winner equals a pure-law predictor fed only the captured candidates and RNG seed. Decode, skeptic corrections (comparator/cutoff polarities re-adjudicated from raw words) and maintainer adjudications: [`tools/iff-dump/r249/round.md`](tools/iff-dump/r249/round.md).

## CAS biography and preview follow-up

Desktop CAS biography now uses the recovered glyph editor, native 5px inset / 751px wrapping, original selection/navigation and capacity-preserving editing; mode/modal transitions and the final visibility boundary clear biography focus. The mobile body-carousel loop no longer overwrites the desktop preview, and its world-space position is converted to tile-space before assignment. The real create/save/read-back flow passes 93 assertions at 800x600 and 1024x768, including a six-paragraph biography. Evidence and validation: [`tools/iff-dump/cas-editor-followup/round.md`](tools/iff-dump/cas-editor-followup/round.md).

## R247 tutorial lifecycle

The tutorial is now a complete loop: the Options "Reset Tutorial" row runs the native confirm→stage flow (`Tutorial.FAM` → `Import/`, fail-if-exists); house entry can spawn the Tutorial object through the native **five-guard law** (inhibit cheat, persistent state < 3, house SIMI global 26 ≠ 0, GUID absent, selector present); generic Sim Calls 0/8/9 record completion at the decoded neighborhood fields (**+0x12a=arg+1, +0x12c=0; state 3 permanently disables the spawner; arg 0 clears global 26 live and in the house file via a lossless in-place SIMI patch**); ESC cancels a running tutorial; and the move-in refusal for tutorial houses (R197's open item: the flag origin is house-SIMI global 58) is genuinely reachable. Decode, skeptic provenance and the review/repair record:
[`tools/iff-dump/r247/round.md`](tools/iff-dump/r247/round.md), [`r247-tut-lifecycle/`](tools/iff-dump/r247-tut-lifecycle/decode.md), [`r247-fam-import/`](tools/iff-dump/r247-fam-import/decode.md).

## R246 real-flow CAS audit

The real Create-A-Sim workflow is now audited end-to-end by the opt-in
`ucasflow` check: real neighborhood entry, real panels/dialogs/typing, the
real persistence write, and fresh-from-disk re-parses pinned against the
original's created-record canon (FAMI/FAMs/NBRS/character-IFF fields,
personality slot order with Generous=0, zodiac never persisted). The audit's
headline find — the first real family save crashed on a 5→6-slot personality
mismatch — is repaired with the original's own slot mapping and pinned by a
regression assertion. Documented divergences (skin encoding, NBRS name/
PersonMode, FAMI v9, writer chunk lossiness) are recorded in
[`tools/iff-dump/r246-casflow/round.md`](tools/iff-dump/r246-casflow/round.md)
rather than silently accepted; the default suite is unchanged (142 checks,
`ucasflow` opt-in only).

CLOSED R252 (tools/iff-dump/r252-record-format/round.md): the created-record
**format** divergences are aligned to the ORIGINAL — NBRS chunk-data version
`0x3E`, record Version `0x4` (80-short PersonData), `PersonMode 5`,
`Name` = lowercase char-file stem (`userNNNNN`), skin `pd[60]` = lgt=1/drk=2/med=3,
and FAMI Version `7` with no trailing zeros (was port `0x49`/`0xA`/256/
PersonMode 9/`"iffname"`/skin 0-2/FAMI v9+16 zeros). The reader gains a
version-aware skin remap (fixes the pre-existing misread of the shipped
original-format saves; `RecordVersionFor` falls back to the `0x4` dialect since
`pd[31]` is 0 on original records), and `NBRS`/`FAMI` readers remain compatible
with the old `0x4`/`0xA` and v7/v9 forms. New opt-in `recordfmt` check asserts
the original write format + round-trip + the skin-map dialect; `ucasflow` pins
repointed to the original format. Residuals: `ImportAddNeighbor` (FAM-import)
still writes the legacy format (disclosed), value-level defaults/randomized
skills diverge from the original's 0s, and the lot-load skin is not exercised
end-to-end (map asserted only). Gates: recordfmt 28/0, ucasflow 94/0, default
142/0.

R253 follow-up
(`tools/iff-dump/r253-serialization-fix/round.md`): the audit's **two P1 NBRS
serialization defects** are fixed — `NBRS.Write` now writes
`NeighbourByID.Count` (header count == emitted live records, so a native reader
does not desync on the tail; runtime-only placeholders are not persisted,
documented), and version-0xA records' opaque 256-short person-data tail is
retained/re-emitted verbatim (`LegacyPersonDataTail`) instead of zero-filled on
round-trip, leaving the runtime 88-short `PersonData` view unchanged. The
audit-fix's `recordfmt` verdict is hardened (no false-green through zero
counters) and CAS family diagnostics now go to stdout (the signed bundle is no
longer mutated). `ImportAddNeighbor` (FAM-import) still writes the legacy
0xA/9/256 format (disclosed, record-shape pinning remains a separate R252
follow-up per the r253-import-export audit §6).

## R245 tutorial lesson-highlight and event poll

The tutorial now flashes the original TutHigh arrow over the exact control a
lesson names, and advances on real input. Opcode 34 ("UI effect") is
implemented (sub-op 0/1/2 → flash by original image id / person-panel button /
relationship panel; on/off pairs), rendering the original 3-frame 167 ms
ping-pong highlight window with the native anchor, lot-button shift, modal
hide/restore and target-destroy laws. The per-tick poll runs the owner's real
"query wait event"/"got wait event" trees against the native 16-entry
condition table (cp mode, person-panel page captions, button press/hover by
image id, shown portrait, shared rotation/scroll edge latch, page-absent),
with an explicit port-side re-entrancy guard the native lacks. Decode,
skeptic corrections (ev 9/11 captions, highlighter modal lifecycle,
re-entrancy, `FindRelButton`, opcode mask) and the review/repair record are in
[`tools/iff-dump/r245-tut-highlight/`](tools/iff-dump/r245-tut-highlight/round.md).
Residuals: poll freezes during tracked modals, ev-5 fires-early, lot-button
override awaits a neighborhood round, cursor ids/PumpMouseMoveMsg unported.

## R244 view-specific cutaway

Wall cutaway now follows the recovered native law. Per-room masks are projected
wall-extent overlap geometry in the rotated view frame (direction/sweep law,
inset screen rectangles, strict overlap, diagonal side adjustments, floor
filter, live-room gate), and the drawn mask is the dynamic composition of the
three-room hover history (behind-room probe walk, off-lot reset-and-return,
floor-change clear), the live-mode selected/tracked person's room with the
decoded outside-person rectangle, and the r26-gated cursor vicinity. The
cross-floor PIP render draws the composed mask for the target floor — walls up
except the watched sim's room — and preserves the main mask identity, content,
history and draw state through success and exceptions. The R243 open items
(direction table, projection algebra, wall extents, insertion events, strategy
vs tool virtuals, cursor algebra, mode identities) are decoded, skeptic-verified
and repaired in
[`tools/iff-dump/r244-cutaway-geom/`](tools/iff-dump/r244-cutaway-geom/decode.md),
[`tools/iff-dump/r244-cutaway-state/`](tools/iff-dump/r244-cutaway-state/decode.md)
and [`tools/iff-dump/r244-cutaway/`](tools/iff-dump/r244-cutaway/round.md).
Residuals: native tool callback unwired, packed-altitude table identity,
diagonal/strict-touch fixtures opt-in, port's own rotation mapping.


## R243 current UI restoration

PIP now uses the existing UI-effects option and the original Mac **no-capture 333 ms transition**, with whole-window drawing gated at **opacity248**. Duration, visibility and target lifetime follow their separate native routes, including hidden skipped ownership, asymmetric reversal and option-toggle behavior. The independent cutaway audit disproves the target-room shortcut: original masks use projected wall geometry and separate selected/tracked-person, room-history, pointer and strategy inputs. Runtime rendering and source-only native limits are documented in [`tools/iff-dump/r243/ui-restoration.md`](tools/iff-dump/r243/ui-restoration.md).

## R242 UI restoration

Independent review of the completed helper handoffs yielded two bounded fixes: CAS preview pixels now clear premultiplied RGB with alpha, and surviving original OBJM frames restore user-event phases only when their actual instruction is **35**. The helper's House56 saved-PIP claim was disproved by its real OBJT owner and opcode; that frame is a **negative regression case**. Review and runtime evidence are in [`tools/iff-dump/r242/ui-restoration.md`](tools/iff-dump/r242/ui-restoration.md).

## R241 UI restoration

Tutorial-owned dialogs now have the original icon, corner anchoring, closebox, outline transitions, group opacity and keyboard routes. Clicking the icon runs the original owner's named lesson-recall behavior; TS1 idle wake semantics are also restored. Review, runtime evidence and explicit scope are in [`tools/iff-dump/r241/ui-restoration.md`](tools/iff-dump/r241/ui-restoration.md). Separate bounded prompts reserve additional audits for an optional helper chat.

## R240 UI restoration

The owner-approved restoration now includes native picture-in-picture events, independent lot rendering, automatic photographs, per-frame event save state, CAS outfit memory and family-name input, and Mac clipboard/Command shortcuts. Prior Needs, camera, album, tutorial ownership and editor fixes remain in place. Independent source evidence, packaged runtime results, visual review and explicit remaining limits are maintained in [`tools/iff-dump/r240/ui-restoration.md`](tools/iff-dump/r240/ui-restoration.md). **Historical "DONE" and R211 close-out declarations below are milestones under older scopes; they do not certify complete base-game or visual equivalence.**


**Legend:** [DONE] verified · [DONE-assess] verified headlessly, honest scope stated · [DONE-partial] verified with residual · [WIP] · [TODO] · [GAP].


## Current gaps (honest — do not overclaim)

- [CLOSED R249/R250] Free-will winner selection and serving — native selection law and per-candidate tests are implemented; focused R250 run served 4/4 winners, sub-assertions 11/0, default 142/0. See `tools/iff-dump/r250/` and `r250-serving/`. Earlier unobserved-winner/serving claims are superseded.
- [GAP-partial] Generic Sims Call 13 malformed/custom-call boundary — R167 closes all two owned-corpus sites and proves both set Stack Object ID to self before calling. The original unresolved-id path reports error 10 and then invokes broad `CleanupPeople(null)` object/person teardown; ~~the port deliberately throws instead. Reproducing that destructive null-target path requires a separate full cleanup-contract decode and is not needed by the owned corpus.~~ **CLOSED (SIM-12 decode + SIM-13 subset + SIM-21 full law, 2026-10-24):** the destructive null-target path is now reproduced on the machine-verified native law (see "Deliberate divergences" above); `g13clean` in the default suite.
- ~~[GAP-partial] Superstar (Fame promote/demote + objects), Vacation (save-on-vacation, bookings, score, souvenirs), Unleashed (pet AI, shows/training), Makin' Magic (Magic Town, quests, spells), neighbourhood mgmt (bulldoze/rezone/create), eviction.~~ **SUPERSEDED (2026-10-08/09 waves — the named coverage gaps are CLOSED, see "Where the game stands"): Superstar EXP-16; Vacation TRV-05/TRV-06 incl. 06b/06c; Unleashed EXP-15 + follow-up (`ulpets` in the default suite); Makin' Magic quest coverage EXP-17; neighbourhood mgmt NBR-06 (bulldoze/rezone/evict + the community build/buy entry+save matrix, closing this row's NBR-04 entry-gate request). 2026-10-09 correction: EXP-11 is TERMINAL (every acceptance row closed or closed-by-disposition at the IFF-native controller-idle law — see "Where the game stands"); the NBR-06 bounded residuals are CLOSED by NBR-07/NBR-08 (venue variants, zone-choice strings + dialog, hide-slot pairing) except evict web-export (ENG-27 scope) and the create-lot non-existence (closed as native-absent).** ~~buy-on-community-lot~~ SUPERSEDED (MAP-02 tranche-2 sweep): NBR-04 closed the community/downtown funds-attribution hole (visited family pays via GameState.ActiveFamily fallback; UILotControl.Budget fallback so InsufficientFunds stays reachable). ~~The community build/buy *enable-matrix* decode (entry gate) remains open (NBR-04 bounded request).~~ CLOSED by NBR-06 (see the supersession note above).
- [GAP-partial] Career/school **progression loop** — R157/R158 closed the outbound commute and one-car multitile law. R163's opt-in `carreturn` trace closes the successful House 5 return/pay/promotion branch: exactly one outbound `CarJunk.iff` creation at `CarPortal.iff:4106:20`, exactly one return creation at `4121:19`, Science promotion level 0→1, and exact `CarPortal.iff:4103` budget transactions +460 bonus and +230 salary with mounted tuning[132]=2. Household net changed +680 because an unrelated `Fridges.iff` expense contributed −10; attributed career pay remained exactly 690. R165's opt-in `schoolreturn` trace closes the successful original SchoolBus day: exact creates at `CarPortal.iff:4119:4` from outbound `4115:10` and return `4116:27`, one visible bus group maximum with both legs naturally removed, Cassandra herself hidden/out-of-world with PersonData[87]=1 at school and visible/in-world with PersonData[87]=0 at home, grade 4→3. The static scan separately pins the missed-school `4117:4` +3 penalty. R166 closes Generic Sims Call 12 on the vehicle path from the original PPC law and confirms `SchoolBus.iff` BHAV 4121 instruction 9 is one of its 47 owned-corpus callers. R222 closes CHANCE CARDS live end-to-end: the fork decoded (4103 ins39-43 `MyPD[63] > min(rnd100,rnd100)`, false → ins54 CALL 4134; 4104-false also falls to ins54), the probability gate decoded (`4134 ins90 PD[24]==0` → `ins121/122 rnd(0..99) < Tuning[135]` = BCON4097[7] = 12% per pay), the cascade mapped (4134 careers 17-21 → 4124 careers 1-10 → 4133 12-16), and the opt-in `chancetrace` caught the exact chain `[4102:13][4103:54][4134:120][4124:55]` live — the Science02 card dialog answered as the player (VMNetDialogResponseCmd law), effects pinned (`4124:56=+5000`, PD[12]+100, PD[24]=1, salary `4103:9` after the welcome dialog); the no-card return ladder is live-evidenced in both variants (fine-only, demotion). Residuals: alternate career/school outcome branches beyond the card remain untraced (school has no cards — the A++/missed-school events are already pinned).
- [CLOSED R219] relationship→death runtime end-to-end. IFF-factually pinned: original death chain 20/20 (R71), closure 47/47 (R72), trigger-wiring (R73), engine-driven entry (R74/R78), serve-proofs 8305/8306/8641 (R75/R76), PersonType bridge (R79), shipped-save death persistence (R80: 8 IsGhost residents of the shipped UserData/Neighborhood.iff — ids 19,20,26,27,28,29,30,31, ages 9/27). R219 caught the live trace in-window: the R77 probe failed because it collapsed motives to +3 while the real 8299 gate is a literal Energy/Hunger/Bladder < -98; with Hunger at the floor the ORIGINAL chain executed at exact decoded instruction sites (8283 ins5 -> 8299 ins4 -> 8198 ins4 -> Global 393 'kill person for good' -> 316 'do grim reaper'), created UrnStone.iff (GUID 5A6FA529) at 393:8, and landed PersonData[68]=1 + entity.Dead. The death notice is a BLOCKING MODAL owned by the tombstone (UrnStone.iff) — the opt-in `deathtrace` check plays the player and releases the latch (r219 law doc). R220 caught the ghost in-window: `Ghost - Main Loop`(8641) + `Ghost - Wander`(8638) executing at 01:48 on the DEAD SIM'S OWN entity (person main ins38 -> 8641, the exact decoded dead-branch site) after the UrnStone night gate (main ins5, Global[0] = the live hour, night 22:00-01:59, 1-in-8 spawn roll per ~30-sim-min pass, cap 3); the grace idle is nominally 200 ticks but the engine's sleep semantics (elapsed-idle decrement + interrupts) spend ~10 sim-min in practice. Disclosed: the one-tick 4100 'generate ghost' frame itself fell between samples (the nightfall avatar join is the attributed evidence) and Ghost Scare/haunt-sound did not execute in-window.
- [CLOSED R221] Censorship blur — trigger→PreFrame→renderer pipeline verified end-to-end (IFF-literal pd30, live exec 0→3→0) AND the pixel level now pinned in the DEFAULT suite (`censorpixel`, 128 checks): the R36 GetLotThumb render pair proved EMPTY (0-pixel diff — that path never rasterizes the censor), so the check captures the LIVE composition with flags 0 vs 3 and pins the mosaic block itself (97% of the diff in the densest window at the pelvis, density 0.54, flat-run median 7 = the port's 8x8 grid law). The ORIGINAL law decoded from the binary (HouseViewer::Censor @0x1ce894 +2-symbol-offset: per-cell MEAN of underlying pixels + a ±8-per-channel jitter + clamp; RenderCensoredBlocks @0x1ce5b4: the 1x3/2x6/4x12 zoom cell ladder). DISCLOSED port approximation: a static 12-color (SUPERSEDED 2026-09-22, UI-32: the native law is ported — per-cell mean + ±8 jitter + the literal-device-pixel (1,3)/(2,6)/(4,12) ladder, re-decoded from RenderCensoredBlocks 0x1ce5b4; censorpixel re-pinned to native, medianErr 0) skin palette texture instead of the original's mean+jitter mosaic.
- [CLOSED R235 (room half)] Room-score formula — the ORIGINAL ComputeRoom law fully decoded (r228-r234) and IMPLEMENTED in VMContext.RefreshRoomScore (both score sites, FreeSO 78b2ddd1): inside rooms score clamp((min(tile56/2,60)−30) + (room108·40−20) + ((2·windows/ents)·40−40) + (ents·10/clamp(tile56/2,10,45)) + (windows&doors ? (windows/doors)·40−40 : 0), −100, +100), tile56 from the architecture room map, windows/doors from the wall pattern code sets {1,7,8,9}/{3,5,6,15,23}; DISCLOSED approximations (the R234 owner decision (a)): room80-inside = entity count (r231 bridge) and room108 = 2.05 (the light-computed BSS value fit against the unambiguous fixture); CALIBRATION: House07 Bob & Betty score 50 vs stored 50.222 (the original's own float→short truncates to 50 — the opening defect port −10 vs 50.222 is CLOSED); the Goth 86.333 fixture stays disclosed (outside-at-load); new opt-in roomlaw pin (soak 7/7); FULL GATE 128/0 on the new law. The MOOD half is CLOSED by R251 (tools/iff-dump/r251-mood-cadence/round.md): the R223 "state-dependent smoothing" was a temporal-misalignment artifact — CalcHappy (0x10b9c0) is a **pure rate-limited recompute** (`mood = Σ(wᵢ·mᵢ)/Σwᵢ`, no blend; once per 60 time-units via person[2064]); the STR#502 (adult) / STR#504 (child) Happy-Weight curves are now wired into the port's mood law (replacing the (Σ7+room)/8 approximation), with the child/adult table split (person+1536 in [1,17] → child, else adult) and the curve→motive index mapping pinned from MotiveCurveSet::LoadFromFile; the 60-unit cadence's absolute clock unit is unresolved, and the suit zero-suppression gate (person[1550] × 13.0, ORIG-02: 13.0f static, no port site) is unmodeled (negligible). The decode, skeptic, pin, and adversarial review all pass; the opt-in `moodlaw` check PASS (aggregation+no-smoothing end-to-end); full gate recorded in HANDOFF. **UPDATE 2026-10-08 (ORIG-02 + AUD-18, supersedes R251's pairing and this row's earlier wording): the Room weight is NOT a flat default — it is HappyWeightCurves entry 5 ((−100;2)(0;1)(100;2); the ORIG-02 decode), Energy is NOT a participant, and the curve↔motive pairing follows the runtime builder array {7,6,8,9,15,13,14} (R251's claimed table matches an unused static rodata block); the room80/room108 disclosed approximations above were likewise superseded by the ORIG-02 room-score constants decode (upkeep two-field predicate, −20/40 light addend, 10/45 bridge).**
- [GAP] Custom objects with custom animations.
- [GAP-partial] Sound/music/speech — infra runtime-verified (HITVM, 4479-event corpus, live HITTV thread); audible-fidelity trace not yet run.
- [GAP-partial] Personality-allocation UI — DESKTOP half CLOSED by R143 (original CAS screens with the LED-strip/pool-25 law); still open: the pet naming/rename + pet skill-string map (#20/#25, Unleashed — EXPANSION) and ~~the touch-path CAS panels (mobile by design)~~ **RETIRED (CAS-03, 2026-10-24, owner directive):** `TS1CASScreen.Original` is unconditional — the touch path composes the ORIGINAL CAS screens (software keyboard = input aid only); `cas03` in the default suite.
- [GAP-note CLOSED/SIM-11] opcode-8 random draws byte-mirrored but seed-dependent (values not pinned). ~~seed-dependent (values not pinned)~~ SUPERSEDED (MAP-02 tranche-2 sweep): SIM-11 done pins the random-draw values.

## UI-100% campaign (user directive 2026-08-28: base game first, expansions last; desktop path — touch stays mobile by design)

Scout synthesis (3 parallel agents; evidence `tools/iff-dump/r159/`): client census (r159-scout-client-census.md), original-canon gap list (r159-scout-canon-gaps.md), engine decode of the unported windows (r159-scout-engine-decode.md + raw disassemblies). Everything below is BASE-GAME unless flagged. Execution order = priority order; each item closes with an -autotest pin per SOP.

**Tier A — whole missing windows/screens:**
1. ~~Relationship subpanel on the decoded law~~ DONE R159 (r159/r159-round.md; NBRS [score,flags] value law + PersonsAge 27/9 child law discovered) — markers kRelSmiley/Heart/HeartDeep 4105-4107 (priority deep-heart > heart > smiley, dst (textX+1, rowTop+bufW+3), rows centered x=[this+28]+(w+40)/2), sort plaques 4108-4111, kRelBars 4802 + Off backdrops, ThumbBack 108, SocialInfoPopup 3750, STR# 240 tooltips, cWinDeltaMeter — replaces rel_*.png / inv_item.png / UITouchScroll (ENGINE-DECODED r159).
2. ~~Neighborhood navbar + surround~~ DONE R160 (r160/r160-round.md) — the 14-button law landed (banner-is-button, 50px grid, kNghCurrent, STR# 151 + the destination tables 169/170/173/174, kLargeMask surround Downtown/LargeBack.bmp); ngbh_*.png + ngbh_outline.png + the gradient retired. Residuals: banner credits-picker, destination-native toolbars (filter/payphone — expansion rounds), no-op buttons for absent systems.
3. ~~Budget window~~ DONE R161 (r161/r161-round.md) — cWinBudgetDlg decoded + ported (funds-readout button, 800x600 board, STR# 146 anchors/rows/tooltips, uniform max-width column law, centered OK); uibudget gate. Residuals: row-indent table values. ~~3-day window + per-category accumulators (engine House data the port doesn't track)~~ SUPERSEDED (MAP-02 tranche-2 sweep): SIM-09 (done) adds the budget ledger — TodayReport/HistoryReport per-category accumulators (JobIncome, ServiceExpense, FoodExpense, BillsExpense, MiscIncome/Expense, HouseholdExpense, ArchitectureExpense) and a 3-day HistoryReport, so the port now tracks per-category and the 3-day window is no longer "untracked".
4. ~~Help/tutorial system~~ DONE R162 for the HELP half (r162/r162-round.md), FRAME CORRECTED R195 — kHelpBtn at (5,164) + the cWinHelp modal with STR# 166's 47 topic pairs + selection persistence; uihelp gate. R195: CWinHelp::Init's LoadBuffer(0xbc0) proves the frame is the PopupInfoTiles 3008 nine-slice (ported, replacing the R162 GenDlg reading), and the R162 'TSPaint scroll thumb' reading is corrected — that draw is the frame's own 12x12 corner tile; the topic list is a ctrl-mgr string-set widget (STR# 166) whose internal scroll painting is not statically decoded (wheel/arrows keep the port's scrolling). DEFERRED: the tutorial/lesson machine (TutHigh highlights, TryTutorial BHAV, Tutorial.FAM).
5. ~~Camera panel + scrapbook~~ DONE R173 (camera half) + R190 (r190/r190-scrapbook-law.md — cWinScrapbook decoded + ported: template 1200 board 700x560, six sheet buttons at the STR# 144 anchors, photo centered (350,220), caption (90,435), filename (520,72) per [19], delete confirm [20]/[21], empty [22]; capture law decoded: mode-4 click → ComputeCameraFrame mouse-centered/clamped → 1px inset → TakeAsyncSnapshot; CPState dims SMALL 200x150 / MEDIUM 400x300 / LARGE 600x400 + custom clamp [200..600]x[150..400] from the ctor 0x212100 stw writes; CamViewAlbum wired; uiscrap gate). Residuals: PNG not JPEG persistence, read-only caption, 1px frame highlight. ~~PIP unported (disclosed)~~ SUPERSEDED (MAP-02 tranche-2 sweep): R240 (tools/iff-dump/r240-pip/primitive-law.md + caption-skeptical-review.md) independently recovered the PIP user-event/caption/placement contract (CPState::GetCPMode/SetCPMode, PIP Open 0x29d30 bottom-margin law, caption font/color/geometry).
6. ~~People pie~~ DONE R191 (r191/r191-people-pie-law.md) — the family Sim selector popped from the live-mode gauge: PieFace1 201x201 re-verified as a PURE MAGENTA KEY MASK (the disc is engine-drawn), the full decoded config ported (MaxRadius 150, InactiveRadius 35, the four colors, 8 compass slots, the 60/30 literal slots), TrackPerson selection law (vm.MyUID) + click-away/ESC close, gauge hotspot on the LiveGadget region; uipie gate. Residuals: ~~sub-pie unported~~ PORTED (UI-23, 2026-09-21 wave — two-ring law, Back affordance, ESC stepping; pet routing pending REF-01), label-ring geometry modeled at the decoded radii (disclosed).
7. ~~ReportCard~~ DONE R159, REPLACED ON NATIVE LAW R180 (r180 — the port-authored three-block child layout is gone: adult JobPerformance/skill controls torn down on child switch, engine-decoded child branch composed natively; R184 later decoded all five mutually exclusive Job-family surfaces). Grade letters still string-set-indirected — numeric shown, disclosed. ~~disclosed~~ RESOLVED (UI-27, 2026-09-21 wave — work.iff STR# 4097 GetString law; grade >= 10 flashes; GetCareer==0 blanks).
8. ~~Job subpanel composites~~ DONE R171/R184 — the crop law IS decoded: one 28x22 career cell from the 112x484 4×22 sheet (R171 compact law); R184 mapped the full five-surface router (Ordinary Job / child Report Card / Dog+Cat Skills / Fame) with native rectangles; R179 pinned that the job TITLE is string-table data with no visible child at the anchors (gate rejects extra text). Residual: none known open on the base-game Job family.
9. ~~System chrome engine-wide~~ CLOSED R195 (r195/r195-system-chrome-law.md) — the premise CORRECTED by full decode: cWinOptions never consumes the Sys sheets (its checks/radios/sliders are dedicated CPanel art: optcheckbox 120x21 / optradio 120x20 / optslideron+off 44x14, dims FAR-verified), the ITS dialog widget factories (GenDlg::AddCheck/Scrollbar/AddSlider + every Mgr::Default*) have ZERO direct bl callers (all dispatch virtual; the corrected caller-scanner is committed at r195/callers.py — the bl displacement is `w & 0x03FFFFFC` sign-extended, no shifts), and the base game's live list surfaces go through the string-set list widget family, not cTSWinScrollbar. What the family actually is: EVERY standard modal (cWinHelp + CWinPhoneBook + budget) is template 3008 = PopupInfoTiles 36x36 12px nine-slice (LoadBuffer(0xbc0) — the R162/R192 'GenDlg' frame readings corrected onto it in both ported dialogs); the phonebook's dlg+0x118 is a 540x25 string-set strip at (50,50) (the slot-11 DropDown 16x16 + slot-13/14 ListBack consumer — ported with a modeled picker popup); its columns are cTSWinText widgets at 10px pitch (row law +0xe0=10) in RGB(0,0,57) on the board (ListBack retired from the columns); its Call/Cancel are GetButtonLabel(2)/(1) ctrl-mgr push buttons (resolves the R192 'SetImage(8192)' residual). cTSWinScrollbar fully decoded (slot 4 = WinScrol 204x20 = 12 cells of 17x20, horizontal cells 0-5 / vertical 6-11 via src.left += 6*cellW, metrics +0xdc/+0xe4 law, magenta chroma key) but has NO identified live base-game consumer — WinChk/WinSlH/WinSlV/WinScrol stay mounted-but-unconsumed, CloseBox 30/MinimizeBox 31 stay RT-only (never bound by SetupWinCtrlMgr) ~~unconsumed~~ UPDATE 2026-09-21: superseded for WinScrol — the shared UIOriginalTextList CONSUMES the decoded WinScrol scrollbar law (Budget/Help/Phonebook; UI-19 closure-verified, evidence/COORD/ui19-closure-verify-20260921.md); WinChk/WinSlH/WinSlV remain unconsumed. `uisyschrome` census gate added (114 checks). Residuals: strip picker interaction modeled; column font table chosen for the 10px pitch (engine's system-font slot — RESOLVED 2026-09-25 (EXP-12): the resolver is cTSFontSys::TSAcquireFont 0x4b0100 over the 23-slot font table, font_table[11] = system slot; the per-widget binding is statically unprovable beyond that — honest partial); columns still non-scrolling (disclosed).
10. ~~Speech balloons + event chrome~~ DONE R194 (r194/r194-speech-balloon-law.md) — GenerateSpeechIcons decoded (icon groups 2000/4000, kSpeechMediumBmp 9000 16x16 + kSpeechLargeBmp 9001 32x32 loaded at 0x209de0/0x209e0c, compositor 0x236650; GetSpeechImage consumed by cXObject::DrawSpriteSlot = the IN-GAME speech path); UIOriginalSpeechBalloon nine-slice builder mounted, the money-change floater now rides the balloon chrome (off the bare text/pill); TragedyMask 9005 verified as the already-wired gzi path (R177 law, 40 shipped sites) and re-pinned; uiballoon gate. Residuals: the DrawSpriteSlot speech-slot path itself + slice metrics modeled (disclosed).
11. ~~Phonebook dialog~~ DONE R192, REFINED R195 (r192/ + r195/r195-system-chrome-law.md) — CWinPhoneBook decoded + ported: template-3008 modal 652x426 with kPhoneBookBkg 620x356 at (16,35) + phoneicon 33x23 at (75,16); the six anchor points recovered from the static initializer 0x459c60 (family list (34,77)-(270,275), member list (351,77)-(625,300), header (50,50)); STR# 180 'Call'/empty-state; the FillFamilyList/FillFamilyMembers census law; WinBtn Call/Cancel with enable-on-selection; desktop routes to the original dialog, touch keeps the mobile alert; uiphone gate. R195: frame corrected onto the actual 3008 PopupInfoTiles nine-slice; the header (50,50) identified as the 540x25 string-set strip (DropDown/ListBack consumer) and ported; columns re-pitched to the engine's 10px RGB(0,0,57) cTSWinText law; the button identity residual RESOLVED (GetButtonLabel WinBtn push buttons). Residuals: modeled picker interaction, column font choice, non-scrolling clipped lists, portrait-swap icons (disclosed).
12. ~~Small mounts~~ DONE R193, ViewMenu half CORRECTED R193→R217, then WIRED R218 (r193/ + r217/r217-viewmenu-pie-law.md + r218/r218-view-pie-wiring.md) — kTallSubpanel 4918 (cpanel\Backgrounds\TallSubPanel.TGA 600x150, loaded by cWinCPanel::Init 0x270240 guarded by CPState::IsDoubleByteUI) mounted as the desktop people-window backdrop at (0,-50) MainPanel-local, LIVE-only; the ViewMenu sheets DECODED as cTSPieMenu ITEMS (InsertString into the view+0x13c pie; ViewMenuBackground 17x17 = member 825, the popup chip; UpdateViewMenu @0x215b42 = the item-frame ladder law; the panel buttons' own law = DoSpeedTransitionSound, no flyout) — the R193 hold-gated plaque mounts RETIRED and asserted absent (uismall re-pinned R217), then the pie itself PORTED as `UIOriginalViewPie` on the tear-off model (press-drag ≥10px on a small button opens the radial; plain clicks step; uiviewpie gate, R218). Residuals: the pop gesture + exact cell arithmetic + tag strings + transition sound id are TOC/vtable-unrecoverable (models disclosed in the r218 law doc).

# R237 UI assessment — visible parity gaps reopened (2026-09-04)

> **SUPERSEDED IN PART (2026-09-21, UI-19 + coordinator):** the Budget, Help and Phonebook composition failures named below were **REAL but are now CLOSED** — repaired by the R238-era restorations (r238-budget / help layout laws, r238-phonebook corrected law + the shared UIOriginalTextList consuming the decoded WinScrol scrollbar law) — independently verified with a live gate PASS 9/0 (coordination/tasks/UI-19.md; coordination/evidence/COORD/ui19-closure-verify-20260921.md). The **TALL BACKDROP** item is NOT stale: it remains **user-gated (Lane U; scoped IsDoubleByteUI patch authored, not applied)**. The unchanged R236 package still passes 129/0: several checks assert the port's incorrect geometry, while `uitotal` tests art provenance rather than visual equivalence. See `tools/iff-dump/r237/ui-restoration-review.md` for decoded evidence; the standing default check list is now **132 entries** (superseding R236's 129); opt-in `uiaudit` adds missing dialog/options captures.

# THE SIMS (2000) MAC PORT — BASE-GAME 1.0 DECLARATION (R211, 2026-09-01)

**Declared complete under the close-out charter** (user decision, R204): the enumerated Tier B/C inventory is closed — every item is ported, ledger-retired as already-done, canon-recovered-and-pinned, or formally disclosed; the Tier C `uitotal` enforcement gate is live in the default suite; and the R211 final hunt round (r211/r211-hunt-round.md: the 74-table UIText census, the art sweep, the residual audit) **found nothing above disclosed-grade**. The delta document (below) is the official disclosure list; the standing gate counts are superseded (see the round-history ledger and the IFF-literalism table for the current default-suite count).

## The delta document (the disclosed residuals, indexed by round row)

> Disposition note (MAP-04, 2026-09-15): UI-10's disposal matrix (done 2026-09-09) is the
> disposition authority for the covered standing items — msgbox152, STR-149 code-7/Error
> sound, Vita sway/pick, pause style-3000 — superseding any awaiting-decision label here.
> Still standing outside that matrix: the R169 RampGenerator curve approximation and the
> r135-class engine BSS undecodables.

The per-round PARITY rows below ARE the delta document. The standing entries:

- **Engines of disclosure** (statically unrecoverable, need runtime TOC/data):
  the msgbox 152 read-site (virtual ctrl-mgr factory, R204); the STR-149
  code-7 static-string case (R203); the Vita sway amplitude (BSS floats) and
  the Vita pick virtual (CFM-glued vtable, R209) ~~(BSS floats)~~ — both DECODED
  r258 (UI-24, 2026-09-21): the amplitudes are STATIC code-section pools
  (π/4 Init / π/2 ctor, the solo-sway branch gated near-dead cat&&dog) and the
  "pick virtual" is cTSWin::InvalidateSelf (a repaint flag, not a picker);
  port pending UI-31; the dirt-tool Error sound — **DECODED 2026-09-25
  (EXP-12): cTool::DoDeniedSound 0x192070 → HIT event [TOC-22024]+90 =
  0x4D35A (the shared denied-sound table: slide+0, explode+41, refund+60,
  kaching+73), 400-tick rate-limited; the port never plays it because the
  terrain tools route errors through their own tooltip path — wiring card
  owed.** Original note: (undecoded, R203).
- **Port-approximation disclosures**: the Paused label's style-3000 face
  (palette-kinship used, R204) with the blink re-show cadence DECODED and the
  steady-indicator law PORTED (R214 — no symmetric blink exists; the timer is
  a one-shot hide on BUY/BUILD/CAMERA entry); the friend-count set-builder
  0x1417e0 internals (the port's standing engine predicate, R205); the
  friend-readout rect now anchors to the FIXED worst-case money gadget
  (178 − "$9,999,999") ON THE MONEY LINE — the full creation-block law
  PORTED (R215 fixed the X, R216 corrected the Y: SetArea(moneyLeft−w,
  moneyTop, moneyLeft, moneyTop+fontH); R144's "moneyBottom" prose was a
  misread of the friend gadget's own bottom); the cheat bar's
  careted input renders modern with the help window PORTED on the engine
  roster (R213; "SetMode(1)" corrected to SetInvalEveryFrame — no edit mode
  exists); the Vita
  idle sway amplitude is a ±0.05 rad disclosed substitute and the sequential
  cycle is the only statically visible engine consumer (playback PORTED,
  R212); the scrapbook captions source sibling table 144 (R211).
- **Absent-by-scope systems** (one-neighborhood port scope, buttons mounted
  engine-exact): import, web export, credits screen, rezone (r143/uinav).
- **Conventions**: touch/mobile surfaces stay mobile-by-design (the charter);
  edit boxes render modern text (standing convention); uitotal walks the live
  game-screen state and its PAUSED fold (mode panels pinned by their own
  checks; R214).
- **Engine-side pre-existing quirks preserved**: e.g. r144's +0x1a8
  attribution CORRECTED by R205 (friend readout, not clock); R202's PPX fix.

## Post-1.0 hooks

The registry+walker machine (R210) and the pinned canon (all *Law classes) make every future change self-enforcing. Expansions remain an explicit go/no-go decision, per the charter.

---

**Tier B — in-place corrections (compact pointer index; each fully documented at its evidence dir):** R201→R202 sim-visibility RESOLVED & CORRECTED (R201's "Vitaboy draws zero fragments" was wrong — the real defect was `PPXDepthEngine.RenderPPXDepth`'s non-MRT branch leaving the DEPTH target bound, so sims drew into the depth buffer; FIX = one symmetric line + `simvis` rewritten framing-correct and PROMOTED into the default suite). **R196** UIBigButton→WinBtn on the desktop path (shared\sys\WinBtn.bmp 260x33, the R142 law). **R174+R197** lot-query chrome CLOSED (diagonal stripes out + direct dispatch + native hover plaque + STR 133 empty-lot refusal; desktop MOVE-IN ported on recovered `cWinNeighborhoodVC::MoveInModeLotHandler`; the 45x68 member strip belongs to the PAF cards (R143), never the lot query). **R198** tooltip box + error-color unification (ONE shared cDefaultTTWindow singleton; NORMAL = default BLACK, ERROR = palette(255,0,0) RED; R145 green is catalog context; BLACK is the live-mode law; five TSO disabled-object branches route to the R129 reason ladder). **R187** motive trend arrows (cAverageHistory exact record law + cWinDeltaMeter five-slot state machine + five arrow masks). **R200** follow-sim visibility (TrackPerson/StopTracking: the kTrackingTarget crosshair marks the TRACKED sim's WEBCAM PORTRAIT, not a gauge button). **R204/R203/R205** STR# 148 Pause / 152 DefaultDialogButtons / 149 DirtToolErrs DONE; R205 162 FriendCountDlg + 138[20] tooltip (r144's '+0x1a8 = clock' attribution CORRECTED — +0x1a8 is the FRIEND READOUT; the clock digits paint at TSPaint 0x2b5978). **R206** lot-entry permanent control panel (the panel composes from frame one; R199 residual CLOSED). **R207** cheat bar law (cTSWinTextEdit2: (20,20) 200x21, opaque RGB(64,93,95), WHITE, cap 255, chord ctrl+shift+**C or B**). **R208** screen-switch law (base-game switches are DIRECT constructions — no wipe; the only engine transit art is the expansion taxi/cWinPictureSplashDialog; UITransDialog deleted). **R210** Tier C 'uitotal' enforcement gate (UIArtProvenance + live-tree walker). R160/R161/R162 (navbar/budget/help) and R199/R202 remain covered under the UI-100% campaign and the current-truth section.

**Tier C — enforcement:** final 'ui-total' gate — walk the live desktop UI tree per state and fail on any texture sourced from modern Content paths (whitelist = the disclosed no-canon items below).

**Out of scope (honest):** touch-path mobile composition (by design, documented); all EXPANSION families (scout-canon-gaps §B — Magic book, Superstar, Unleashed filters, Hot Date/Vacation/Magicland phones+banners, balloon shadow — deferred with expansion gameplay); absent-from-disk members (BuyBack/OptionsBack/BuildBack/LiveBack/CamBack/DisposeBack/IconTemp/MaxisLogo — no canon exists; port chrome is the lawful state); no-corpus strings (STR# 160 [14..19], search/floor-ordinal strings — negative space proven r124/r115); engine-internal undecodables stay disclosed models (BSS tables per the r135 signature).


## What is verified — IFF-literalism (do not regress)

The engine mounts/exposes **exactly the original IFF data**; every permanent -autotest check below FAILs on any drift. Canon comes from engine-independent raw IFF decoders in tools/.

| Check | Pins (must equal raw IFF canon, else FAIL) |
|---|---|
| corpus | module BHAV counts: Global 251, PersonGlobals 473, SocialGlobals 101, CatGlobals 179, DogGlobals 177, PetSitGlobals 22, PhoneGlobals 149, VacationDirectorGlobals 85 |
| catalog | served buy catalog == raw OBJD canon: 1114 GUIDs, price/category, sort bytes, names, behavioral entry ids, full RawData, order, defaults (A–H) |
| career | engine JobData == raw CARR canon (20 tracks) + loop IFF-mount + closure + serve-proofs |
| freewill | brain mounted at IFF ids 8303/8301/8229/8275/8219; dispatch chain 1-op-call; closure |
| freewillvar | production action aggregation preserves two `Change Action String` variants by identity, order, Param0 `[111,222]`, score, and callee (R163) |
| persondata | persisted OBJM/NBRS prefixes zero-extend to the complete 101-word address space; live words 21/70/74/80/81/84/100 round-trip with TS1 raw semantics, word 98 stays zero in the House 5 fixture, original Global 488 and PersonGlobals 8645 execute, and `InheritNeighbor` normalizes (R163) |
| travelinv | original PhoneGlobals call sites pinned; production modes 5/6 replace GUID 10/11 companion tokens with the latest Temp 0 neighbor id instead of appending stale tokens (R163) |
| savedthreads | original `House05.iff` import only: exact repaired object ids `10,114,115,116,117,123,131,133,134,173,174,191,192,194,226`; each pinned owner GUID restarts its current entry-point main as a self-owned, non-action root frame at IP 0 with an empty queue, inactive queue block, and cleared interrupt; every ordinary live frame has a code owner, non-empty routine, and in-range instruction pointer (R164) |
| brainlive | original PersonGlobals brain EXECUTES in live VM (R70): >=1 PG-scoped avatar frame AND >=1 canonical brain BHAV observed (measured 9,013 frames / 16 ids) |
| deathchain | original death/motive-failure chain IFF-literal 20/20 + closure(47) + trigger-wiring + engine-driven entry (8342/8193 zero IFF callers) + dispatch surface + PersonType bridge + shipped-save death (8 IsGhost) + serve-proofs 8340/8299/8305/8306/8641 (all live-exec completed on avatar obj=16) |
| savesim | shipped-save sim-state (R81): JobLvl>0 exactly 7 residents, user00000 Cooking 300/Mech 200, rel matrix 22 rows; motives runtime-only |
| carseek | original carpool portal mounts and executes its morning commute path: 4096/280/4100/4125 plus portal-owned 4105/4102, one `CarJunk.iff` multitile group created at 4106:20, and the worker departs (R84/R157/R158) |
| carreturn (opt-in) | natural House 5 full-shift trace: exactly one outbound create at 4106:20 and one return create at 4121:19, Science level 0→1, exact +460/+230 pay callbacks, and mounted bonus tuning 132=2 (R163) |
| personality | slots 2..7 IFF-literal; playable avatars carry varied IFF personality; gate census == 1 |
| motiveinit | init-motives(8325) writes only MyMotives {5..9,14,15}; BCON 8195 base/amount |
| skills | Easel pd15@ins3/4/5, ChessTable pd18@ins5/6/7, ExerciseMachine pd17@ins1/2/3, Mirrors pd11@ins1/2/3 |
| motiveact | Beds sleep→[5]@ins0, ChairsLR1Tile comfort→[6]@ins2, ShowerC hygiene→[8]@ins1, Bars drink→[9]@ins53 |
| relact | Baby DNA 3@ins1/6@ins4/7@ins5; Fridges 34@ins10/32@ins11; Beds tuck-in 58@ins5 |
| money | CarPortal pd[24]@49; MailBox pd[1]@21; Easel pd[1]@12; Newspaper pd[80]@0 |
| ttab | Beds Sleep→4106/4107, Easel Sell→4100/4101, Fridges Serve→4107/4112 |
| ttas | Beds 'Sleep', Easel 'Sell Painting', Fridges 'Serve Meal' |
| opcodes | PersonGlobals whole-brain fingerprint: 6124 instr; op2=3562 op8=73 op3=3 op13=304 op44=375 op42=159 op28=176 op26=80 op31=73 op41=63 |
| callgraph | 20 fixture objects: 747 BHAVs / 7767 instr; op histogram |
| globalcalls | global-call histogram 1053/98 distinct, 0 dangling, checksum 338605 |
| snd | .wav 284 / .mp3 0 / .xa 17438 / .utk 0 |
| iff | FAR1Provider mount table == 46,591 member rows / 35 extensions |
| objd | TS1ObjectProvider distinct-GUID corpus == 5120 |
| ctss | CTSS 1895 chunks / 3769 blocks |
| strs | STR# 5430/155482 + TTAs 1568/11286 |
| consts | BCON 3663/29952 + TPRP 206/434 + GLOB 478 |
| bhvi | BHAV instruction corpus 388503 |
| bhop | BHAV opcode histogram 713 distinct / 388503 |
| dgrp | DGRP 20522/514716 + PALT 5238/1340928 |
| slot | SLOT 1472/6969 + SPR# 758/3120 + SPR2 17140/138948 |
| operand / opmx | BHAV operand bytes 3108024 (256 buckets; per-opcode matrix 713/4571) |
| chunks | registered-chunk inventory 28 types (BCON 3663, BHAV 27852, OBJD 5120, …) |
| uipal | ORIGINAL-UI palette (13 anchors: #000029/#000052/#00004A/#080852/#73739C/#636394/#5A5A8C/#00186B/#00106B/#001063/#72729E/#7474A0/#00FFFF) + UIStyle aligned navy/cyan. **UI-33 UPGRADE: BYTE-PINNED** — the gate now runtime-verifies the five anchor members 1:1 (raw FAR name+len+sha256, uicur idiom; canon generated by tools/iff-dump/make_uipal_canon.py) and 14/14 anchor colors are re-derived from the members' own BMP pixels (incl. RLE8); R67's "motive-bar backing (PersBkg)" attribution of #72729E/#7474A0 is CORRECTED — the re-derivation locates both colors only in cpanel\Catalog\BuySTDining.bmp (all 13 array values verified) |
| uidump | IFF backdrop widget-tree pin: `OriginalPanelBack` present, visible, and sized `min(804, ScreenWidth-220) × 100` per the R142 correction; headless live-UI render to PNG |
| loadscreen | ORIGINAL start + loading ('Go') screens IFF-literal (R83): 19 UIGraphics.far members (Nbhd\NScreen.BMP, Community/Nbhd Bus_loadscreen 800x600/1024x768, Downtown Taxi, Magicland, Studiotown, VIsland) mount 1:1 by member name + byte length + BMP header dims; client loading screen IFF-mounts kNeighborhoodGo (ResolveOriginal) |
| uichrome | ORIGINAL live-toolbar chrome IFF-literal (R85): 5 category buttons (Mood.bmp 128x39 / Job / Personality / Relationship.BMP / people.bmp 200x50) + Greenbars.BMP/Redbars.bmp motive fills 27x25 mount 1:1 by member name + byte length + BMP header dims (7/7); rendered-control TREE gate: category button 128x39 + motive bar 27x25 mounted live (IFF-first in UICategorySwitcher/UIMotiveBar) |
| uitoolbar | ORIGINAL live-toolbar chrome IFF-literal (R86): LIVE house tab (House.bmp 108x30) / OPTIONS tab (options.bmp 92x23) / PAUSE (pause.bmp 60x30) / EXIT (OptExit.bmp 200x40) / OBJECTS (objects.bmp 180x47) / LiveGadget gauge (LiveGadget.bmp 108x100) mount 1:1 by member name + byte length + BMP header dims (6/6); rendered-control TREE gate: live tab 108x30 + gauge 108x100 mounted live (IFF-first in UIMotiveBar/UILiveButton+UIDesktopUCP) |
| uicur | ORIGINAL UIGraphics.far .cur cursors IFF-literal (R87): all 51 cursor members (Shared\cursors\*.cur, the ORIGINAL game cursor paths) mount 1:1 by raw FAR stored name + byte length + sha256 (byte-verbatim); PLUS rendered-control gate: the IFF-first live mount loaded the ORIGINAL LivePerson.cur (326 bytes) into the runtime CursorManager on every platform (macOS GameFacade.Linux skip removed) |
| uiglyph | all 45 original UIGraphics.far `.ffn` tables mount byte-verbatim; R96/R119–R140 parse and render them live engine-wide with corrected atlas offset, premultiplied ink, Windows-1252 mapping, and native-size ladder behavior |
| uiboot | ORIGINAL boot/logo screen IFF-literal + LIVE (R88): kSimsLogo (Res_Other.RT id 9003) = Other\setup.bmp + 17 language variants (each 800x600x24, plumbob+wordmark art baked in) pin 1:1 by raw FAR stored name + byte length + sha256 + BMP dims; the boot loading screen IFF-mounts the original kSimsLogo live — Simitone logo/diagonal stripes/elastic bar hidden, engine-drawn original-style progress bar active (original templates declare NO bar/logo-control asset -> original bar is engine-drawn) |
| uilogo | ORIGINAL "The Sims" logo stamps IFF-literal (R88): kNghSimsLogoEn/SP/FR/GR/JP (NghUI\TheSimsLogo*.bmp) + kDowntownCredits + kStudiotownCredits pin 1:1 by raw FAR stored name + byte length + sha256 + BMP dims (70x52 / jp 99x52) |
| uianim | ORIGINAL animated frame families IFF-literal (R88): Unleashed neighborhood waves kNghFrameDelta1-6_UL + NonUS (12) + TS1.0 frame-deltas DiffN1-N2/N3/N4 + Loc (6) + Downtown water dscreen00-05 (6) + Magicland water dscreen_waves1-6 (6) + Magicland fog mistA/B/C tga (3) + Magicland balloon 1-16 (16) + Studiotown car 0-19 (20) + Vacation water visland_waves001-005 (5) = 74 frames pin 1:1 (name+bytes+sha256+dims) |
| uinbhd | original neighborhood animations are live: water families, decoded six-lane Studiotown cars, 21-cloud Magicland fog, wind-state balloon, and the Nessie cheat; engine-derived positions/timing and movement invariants are pinned (R89–R95). Residuals are limited to the disclosed unresolved binary tables/MathLib amplitude and remaining hotspot/layout work |
| relation/rel-key/rel-mode | NBRS loads original 22 records; UseNeighbour keyed by NEIGHBOUR id; modes 0–3 resolve 6/6 |
| censor | pd30 triggers {0,1,3,128}; live exec 0→3→0; sites ins19/23 + ins10/14; render pair |
| jobs | original career table loads 1:1 (NumJobs=20; Mortimer jobType=9 → 'Test Subject') |
| npcinfo | MakePersonData mirrors init-NPC: 25/50/−1,4,0 + Cheats 1 + PersonType 2 + skills [0,1000) |
| names | in-lot avatars keep saved names (Cassandra/Bella/Mortimer) |
| audio | HITVM init; 4479-event corpus; PlaySoundEvent creates live HITTV thread |
| motive/mood/load/lot | Goth house 5 loads (303 entities, 3 avatars, 0 errors); motive decay directional; mood (s7+room)/8 ±3 |

## What is verified — engine fixes (each IFF-grounded, committed in FreeSO mac-port-rel)

- UseNeighbor matrix-key — resolve stack avatar to NEIGHBOUR id before NBRS lookup (cc63db95).
- vsync-park — disable SynchronizeWithVerticalRetrace for headless/remote sessions.
- exit-hang — OnExiting re-arms Exit() when quit-confirm defers (95db2d4).
- new-sim motive init — IFF-literal init chain instead of blanket 100 (b695a209 + 11bb46ff).
- NPC personality — PersonalityPoints 500→1000, pd[4]=Generous (609044e0).
- NPC job/autonomy/priority — 25/50/−1,4,0 (e7cd5a36).
- NPC skills+Cheats+PersonType — op8 spreads [0,1000) + 1/2 (1b305904).
- OBJD v138 read — numFields 106 so all sort bytes parse from IFF.
- IFF ListAll per-chunk try/catch — no chunk dropped when one body Read throws (c8141d7d).
- FAR order — FAR1Provider.Init sorts by IFF path (Ordinal) (ce53d276).
- lowercase glob chunk registered so NPC semi-globals mount.
- R163 PersonData — persisted TS1 prefixes normalize to 101 signed words while TS1 aliases remain raw; renderer word 74 is retained after its side effect.
- R163 travel inventory — modes 5/6 replace the GUID 10/11 token they create and store the current Temp 0 neighbor id.
- R163 autonomy — `VMFindBestAction` retains every dynamic action-string variant rather than duplicating the first candidate.
- R163 scopes — TreeAd JIT scopes 12/28/29 delegate to the interpreter contract and advance the JIT cache to version 2; TS1 NeighborhoodData scope 34 reads signed NGBH words 1..15 and stays read-only.
- R163 observation hooks — successful object creation and committed budget mutations expose exception-isolated, read-only callbacks used to attribute the natural career trace without changing primitive outcomes.
- R164 saved continuations — the TS1 OBJM importer validates every saved frame against the currently mounted code owner, routine, and instruction range; one stale frame rejects the whole continuation and its interaction state. After entities, groups, and missing controllers exist, the importer resets each rejected owner once onto its current main before the first simulation tick and clears abandoned avatar motive deltas. Repaired ids are exposed only through a transient, non-serialized TS1 lot-state report; generic TSO/network `VMThread.Load` semantics are unchanged.

## RE corpus facts (verified — this repo, tools/)

- FAR1 mount = 16-byte header + filename; GameData/Global/Global.far = 24 IFFs. IFF container banner "IFF FILE 2.5:…"; 76-byte chunk header (container BE, payloads LE). BHAV: header 0x8001/0x8002 (TS1) or 0x8003; instr = opcode u16 + true/false ptrs + 8-byte operand; opcode <256 primitives, 256–4095 global, 4096–8191 private, ≥8192 semi-global. No numeric operand stack (Args/Locals/TempRegisters[20]).
- Chunks present but ignored (correct): rsmp, XXXX, POSI, TMPL, pers, CATS, Optn, FAMh.
- Per-object call-graph audit: 0 dangling global/private/semi-global calls across all 126 object IFFs; global-call map 125 named subroutines, all resolve.
- Legal/REF: Doornbos "Object file format in The Sims" (2000); SimsTek wiki; simantics.primitives; FreeSO/Simitone MPL-2.0; IffPencil (Apache-2.0); ffmpeg EA-ADPCM/XA.
## Round history (evidence ledger — one line each)

| R# | verdict | one-liner | evidence |
|---|---|---|---|
| 236 | DONE | roomlaw PROMOTED to default suite; House7 pins the R235 law EXACTLY (avatar room == 50 — reproduces stored 50.222 through the original float->short truncation); House5 pins liveness (decoded law's furnished rooms clamp at 100; Goth 86.333 stays DISCLOSED, outside-at-load); FULL GATE 129/0. | tools/iff-dump/r236/ |
| 235 | DONE | the ORIGINAL ComputeRoom law SHIPPED (decision (a)) at BOTH score sites in VMContext (FreeSO 78b2ddd1): inside rooms closed form + tile56 from architecture map, windows/doors from wall PATTERN code sets; DISCLOSED room80-inside=entity count (r231 bridge), room108=2.05 (BSS); House07 50 vs stored 50.222 (opening defect CLOSED); Goth 86.333 disclosed; roomlaw pin soak 7/7; FULL GATE 128/0. | tools/iff-dump/r235/ |
| 234 | DONE | the aggregation loops decoded (0x1270d0+ = UNROLLED continuation of R233's per-tile seeding; NO float aggregation/additive term in ComputeRoom tail); the ~26.7/+23 gap can only be the TRUE room108 — fixtures require 1.667 (House07) / 1.575 (House05), light-subsystem BSS values; decision point (a) ship calibrated approximation or (b) hold for runtime. | tools/iff-dump/r234/ |
| 233 | DONE | outside tail decoded and missing-term hypothesis CORRECTED: 0x126ba0-0x126dec computes int(1+255·room108) and SEEDS per-tile byte maps (24-byte strides via room+0x78 pointer tables) — per-tile score/lighting init, NOT an additive term; gap localized to 0x1270d0+ aggregation. | tools/iff-dump/r233/ |
| 232 | DONE | [world+12] candidate: world struct (TOC-29216 → BSS 0x90850) only stfs +12 = LightLayer init 0x1dc1f4 (f23=1.0); with room108=1.0 the fixtures land at 23.53/63.33 vs 50.222/86.333 = consistent ~26.7/~23 gap (one missing additive); R231's 1.6 explained as 1.0 + ~0.6. | tools/iff-dump/r232/ |
| 231 | DONE | shadow calibration — fixtures REPRODUCED for the first time under hypC + room108≈1.6 (House07 needs 1.667=5/3, House05 1.575); consistent with a single [world+12]^2·type-factor world constant; pin honestly blocked on decoding [world+12] and the Goth outside-at-load path. | tools/iff-dump/r231/ |
| 230 | DONE | attribution pass DEFINITIVE: CollectObjectStats (0x125df0-0x1261d0) has NO float stores into the room struct; room[80]/[108] are aggregates over the LIGHT SUBSYSTEM's per-position values; remaining item = a DESIGN bridge (export per-room light aggregates or port LightLayer position math at 0x1dd6f0 + aggregation). | tools/iff-dump/r230/ |
| 229 | DONE | field-mapping refinements: obj[88] branch law (CollectObjectStats 0x126160-0x1261a0: OUTSIDE rooms sum v<0 into room[80], INSIDE count v>=0 into room[84], room[76]+=obj[152] wear); CORRECTION — inside t1=room[60]/room[80] cannot read the obj88 accumulator (stays 0 → div-by-zero); inside room[80] fed by the collector's LIGHT/GEOMETRY block (0x125fe0-0x126080, r30 +8=0.1875/+12=3/+16=5). | tools/iff-dump/r229/ |
| 228 | DONE | ComputeRoom law assembled IN CLOSED FORM: INSIDE = clamp((min(tile56/2,60)−30)+(room108·40−20)+((room60/room80)·40−40)+(room84·10/clamp(tile56/2,10,45))+(room96?(room96/room100)·40−40:0),−100,+100); OUTSIDE = 2·room80+1·room84; every constant from the R225 22-float table; full field-semantics table. | tools/iff-dump/r228/ |
| 227 | DONE | window/door classifier MECHANISM decoded (0x1264e0-0x12652c): room+0x3c += 2 per wall whose WALL RESOURCE style-pair is nonzero — the lookup is DATA (.wll catalog per-style records: [style*4+32]→rec, rec+12→u16 subtable, [styleX*4+styleY*2] flag); port must parse it for implementation. | tools/iff-dump/r227/ |
| 226 | DONE | tile-collector ported as permanent roomlaw-tiles diagnostic (r134 CollectTileStats over VMArchitecture: tile56=wall+1/open+2 + wall style histogram); House07 50.222 room = tile56 39/17 wall tiles with styles where the original code sets sit; House05 Goths load OUTSIDE; two prerequisites enumerated (window/door classifier, phase-1/collector stat semantics). | tools/iff-dump/r226/ |
| 225 | DONE | room-score constants RECOVERED: capstone proved R223's decoder had frB/frC swapped (real A-form frD/frA/frB/frC/5-bit XO; ground truth fsubs f5,f1,f2 @0x126894); 'impossible' pipeline = 0x4330 int→double + fcmpu/fmr min-max ladders; 695-instr ComputeRoom disasm banked; constants chain RoomManager::ComputeRooms→RoomScoreConstants::UpdateConstants→FloatConstants::Set; 22-float provider table @file 0x5a3538 recovered verbatim (0.8,255,0.1875,3,5,0,1,0.5,100,0.01,−100,0.0625,30,60,−30,−20,20,10,45,2,−50,−40). | tools/iff-dump/r225/ |
| 224 | DONE | room-score decode round — ground truth + constants + named blocker: Room::ComputeRoom @0x1267d0 mapped on clean disasm (IsOutside split; INSIDE type-selects r30[+20/+24/+28]=0.0/1.0/5.0; OUTSIDE int→double-collector stats normalized against r30+32=100); the 0x126890-0x1268c0 float pipeline renders with a division-by-a-just-zeroed register (idiom/data island). | tools/iff-dump/r224/ |
| 223 | DONE | mood/room formula: STR#502 seven mood-weight curves recovered verbatim (MotiveCurveSet::LoadFromFile); CalcHappy @0x10b9c2 decoded — per-motive curve eval (mood=Σ(w·m)/Σ(w)); FALSIFICATION via 5 original save pairs: stored mood is STATE-DEPENDENT (smoothing/slower cadence); room-score half quantified (port −9 vs 86.333/50.222). | tools/iff-dump/r223/ |
| 222 | DONE | CHANCE CARDS traced live end-to-end: 4103 ins39-43 rolls min(rnd,rnd) forks on MyPD[63]>min → FALSE ins54 CALL 4134; probability gate decoded (4134 ins90 PD[24]==0 → rnd(0..99)<Tuning[135]=BCON4097[7]=12%); cascade 4134(17-21)→4124(1-10)→4133(12-16); chancetrace caught [4102:13][4103:54][4134:120][4124:55] live, answered via VMNetDialogResponseCmd; effects pinned (4124:56=+5000, PD[12]+100, PD[24]=1); no-card ladder live-evidenced (fine-only, demotion). | tools/iff-dump/r222/ |
| 221 | DONE | censor blur PIXEL level pinned (censorpixel promoted 127→128): R36 render pair proved EMPTY (GetLotThumb never rasterizes the censor); live-capture pins the block (97% diff in densest window, density 0.54, flat-run median 7 = the port's 8x8 grid law); ORIGINAL law decoded (per-cell MEAN + ±8 jitter + clamp; RenderCensoredBlocks 0x1ce5b4 1x3/2x6/4x12 zoom ladder) — ported UI-32 (SUPERSEDED the static 12-color palette). | r221/r221-censor-pixel-law.md |
| 220 | DONE | ghost night-haunt traced LIVE (R219 residual): UrnStone.iff law decoded (night gate Global[0]>=22, 1-in-8 roll per ~30-sim-min, 4100 'generate ghost', cap 3); loop CAUGHT at 8641 'Ghost - Main Loop' + 8638 'Ghost - Wander' on the dead sim's own entity at 01:48 (person main ins38 → 8641); ghosttrace opt-in rides deathtrace. | r220/r220-ghost-law.md |
| 219 | DONE | natural death traced LIVE end-to-end (last full non-expansion GAP): R77 miss decoded (gate is < -98, not +3); deathtrace collapses Hunger to the floor and catches 8299→8198→Global 393→316 at exact sites + UrnStone.iff tombstone (5A6FA529 at 393:8) + PersonData[68]=1/Dead; death notice = tombstone's blocking modal; untouched soak 6/6. | r219/r219-death-trace-law.md |
| 1–10 | DONE | harness; Goth lot loads (303/3/0); vsync-park, exit-hang fixed. | autotest-goth-house5 / exfix logs |
| 11–18 | DONE | RELATION→NBRS store; RelVar 0/1; write round-trip + clamp; UseNeighbour matrix-key FIX; NAMES; AUDIO functional; REL-MODE 1–3. | relation/names/relmode logs |
| 19–24 | DONE | careers DATA 1:1; NPC motive-init, personality 1000, job 25/50/−1,4,0, skills+PersonType fixes. | r21–r24 logs |
| 25–26 | DONE | career check (CARR canon + loop IFF-mount + reach-serve). | r25/r26 logs |
| 27–28 | DONE | freewill mounted 8303/8301/8229/8275/8219; career-loop live-exec. | r27/r28 logs |
| 29–45 | DONE | personality, motiveinit, closure, censor census, live render, skills, motiveact, relact, money, ttab, ttas, opcodes, callgraph, globalcalls. | r29–r45 logs |
| 46–53 | DONE | catalog OBJD surface (1114 GUIDs) + v138 fix + names/defaults/order (A–H); honest FAIL R47 run1 30/31 (DowntownSort served=0 vs canon, falsified my mirror, fixed not weakened). | r46–r53 logs (r47 run1) |
| 54–66 | DONE | snd, iff, objd, ctss, strs, consts, bhvi, bhop, dgrp, slot, operand, chunks, opmx. | r54–r66 logs |
| 67–69 | DONE | UI: UIGraphics.far audit (794 assets, 13 palette anchors), uipal, original toolbar backdrop mounted + tree-pinned (uidump), IFF speed strip. | r67–r69 logs |
| 70 | DONE | brainlive: original free-will brain EXECUTES in live VM (9,013 frames / 16 ids); suite 47/47. | r70/ |
| 71–81 | DONE | deathchain R71 pin→R72 closure→R73 wiring→R74/R78 entry→R75/R76 serve-proofs→R79 PersonType→R80 shipped-save death (8 IsGhost) + savesim R81 shipped-save career/skill/rel. | r71/–r81/ |
| 82–83 | DONE | R82 docs housekeeping; R83 loadscreen original start/loading screens IFF-literal (19 UIGraphics.far members mount 1:1); UI LOOK re-skin residual. | r83/ |
| 84 | DONE | carseek: ORIGINAL carpool-portal HEAD live-exec (CarPortal.iff BHAVs run live; morning paper-carrier NPC); residual: commute-CAR tail not yet observed. | r84/ |
| 85 | DONE | uichrome: ORIGINAL live-toolbar chrome (5 category buttons + Greenbars/Redbars fills) IFF-literal + rendered-control tree gate. | r85/ |
| 86 | DONE | uitoolbar: ORIGINAL live-toolbar chrome (LIVE house tab / OPTIONS / PAUSE / EXIT / OBJECTS + LiveGadget gauge) IFF-literal + tree gate. | r86/ |
| 87 | DONE | uicur/uiglyph: ORIGINAL UIGraphics.far .cur cursors (51) + .ffn glyph-tables (45) IFF-literal byte-verbatim; IFF-first live cursor mount on EVERY platform (removed GameFacade.Linux skip); UPDATE 2026-10-09: make_r87_canon.py retired by ORIG-02 (r87/*.cs + r87-resource-canon.json remain). | r87/ |
| 88 | DONE | uiboot/uilogo/uianim: ORIGINAL boot/logo screen LIVE (kSimsLogo Res_Other.RT id 9003 = Other\setup.bmp, 18 locale members byte-verbatim) + logo stamps (7) + animated frame families (74 frames) 1:1; UPDATE 2026-10-09: make_r88_canon.py retired by ORIG-02. | r88/ |
| 89 | DONE | uinbhd: ORIGINAL neighborhood screens LIVE-ANIMATED (12 families/71 cycled frames through UINeighborhoodAnimationLayer); 11 screen members pinned 1:1; sprite positions provisional (engine code, not IFF data). | r89/ |
| 90 | DONE | engine-RE: stereo anchor scan (scan_pef_ppc.py, 356 anchors/256 strong); nessie is a CHEAT (DoNessie, CheatCallback spawns Downtown 531,438 / Vacation 200,530 / UL 118,519); Magicland waves (0,62) ENGINE-CONFIRMED; fog=21 CloudInfo clouds; Studiotown car=SIX carLaneInfo lanes; full tables in r90/RE-ENGINE-CONSTANTS.md. | r90/RE-ENGINE-CONSTANTS.md / r90/ |
| 91 | DONE | uinbhd car-lane port live (UINeighborhoodCarLaneLayer, 6 lanes/54 cars at engine-literal coords; 20 frames are STATIC variants, Random()%18 picks; movement rare-event). | r91/ |
| 92 | DONE | uinbhd cloud/balloon port live (21 Magicland clouds + wind-walk balloon from decoded engine logic; ppc_decode.py built, two dump quirks encoded); R90's 'seed*35 LCG' corrected to /35 magic. | r92/ |
| 93 | DONE | uinbhd nessie cheat live (CheatCallback__18cWinNeighborhoodUL typed-command match; spawn 118,519; state machine 0-5; once-only refusal; end-state decoded). | r93/ |
| 95 | DONE | uinbhd water positions: Downtown + Vacation waves draw at ENGINE-LITERAL (0,0) (ctor li/this+308+312 / this+316+320 zeroed); UL/TS1.0 self-drawing SpriteSlots disclosed-unresolved. | r95/ |
| 96 | DONE | uiglyph text render: ORIGINAL .ffn tables decoded (FNTF header + 11-byte char records + one 4-bit grayscale atlas) and rendered live (OriginalGlyphFont + UIOriginalText on neighborhood titles). | r96/ |
| 97 | DONE | uiglyph second surface: neighborhood hover lot captions in original glyphs (variablesans_10). | r97/ |
| 98 | DONE | uiglyph third surface: lot-screen money readouts in original glyphs (variablesans_12_bs bold; § glyph). | r98/ |
| 99 | DONE | uiglyph fourth surface: all remaining lot-toolbar readouts (time/friends/floor) in original glyphs (variablesans_11). | r99/ |
| 100 | DONE | uiglyph fifth surface: cheat-bar RESPONSE in original glyphs with the ENGINE'S OWN string ('Sorry only one Nessie at a time.', variablesans_09). | r100/ |
| 101 | DONE | car-hotspot residual CLOSED BY REFUTATION (the '+3/+4' are record field offsets in the car anim function, not pixel offsets; the final draw passes r6=r7=0). | r101/ |
| 102 | DONE | layout montage delivered as a verification artifact + PEF section map (3 sections; cloud-table lwz bases are section-relative, loader-relocated). | r102/ |
| 103 | DONE | sway instruction-decode + PEF unpacker (two MathLib calls; amplitude = named TOC slots behind sec1 unpack). | r103/ |
| 104 | DONE | PEF unpacker COMPLETE (Ghidra corrections; sec1 unpacks to exactly 0x7bf80); sway angle constants ENGINE-RECOVERED (0.785/180.0/wind gates 1.5/3/4.5/5/6); cloud tables need sec2 relocations. | r104/ |
| 105 | DONE | cloud-table trace STATICALLY EXHAUSTED (relocation simulator; target region remains zero); 142/262 disclosed model remains; SUPERSEDED r258/UI-24: the 'lwz tables' are subfic ladders and the engine-exact cloud-seeding law is recovered. | r105/ |
| 106 | DONE | uiglyph sixth surface: options-panel caption in original glyphs (variablesans_08). | r106/ |
| 107 | DONE | UL/TS1.0 water positions RESOLVED AS SPRITE-DATA (position = the sprite RESOURCE's stored origin, inside the activate path 0x47b350; no engine-code constants). | r107/ |
| 108 | DONE | nessie_sfx ENGINE-RECOVERED (TOC entry sec1+0x38fc = 0x6c6e0 '%d\0nessie_sfx\0UI_Nhood_click...' — +3 skips the prefix, engine plays the sound NAMED 'nessie_sfx'). | r108/ |
| 109 | DONE | per-font advance semantics RESOLVED (advance = width + t0 UNIFORM across all seven English tables; the R97-era '_07 t0-as-advance' note was a MISREADING). | r109/ |
| 110 | DONE | sway multiply structure engine-decoded (sway=(int)(x·trig(wind·pi/720)); no dedicated amplitude constant; 64.0 the corpus's one unaccounted float); SUPERSEDED r258/UI-24: sway=(int)(m·sin(0.785·m/180)), m=seed%1440, constants are DOUBLES, '64.0' never existed. | r110/ |
| 111 | DONE | ORIGINAL SPLASH-PROGRESS TEXT restored (GameData/UIText.iff STR# 'splashprogress' chunk 155, 340 entries = 20 lang × 17, whole-chunk sha 39fdf8af; English title-screen scroll; original strings carry NO trailing dots); `uisplash` gate (59→60). | r111/ |
| 112 | DONE | DIALOG BASE CLASSES in original .ffn type (UIMobileDialog/UIMobileAlert + UIBigButton via variablesans_09; STR# 'ObjDialogs' chunk 142 = OK/Cancel/Yes/No); `uidialog` gate (60→61). | r112/ |
| 113 | DONE | LOT-QUERY PANEL in original .ffn type (UIHouseSelectPanel; NghRollover chunk 134 + MoveInModeStrs chunk 132 [15] with EMBEDDED \r\n wrapper); `uilotq` gate (61→62). | r113/ |
| 114 | DONE | LIVE-MODE SUBPANEL NAMES in original strings + glyphs, with PROVENANCE FIX (Live.iff STR# 'Motives' chunk 130 + 'Personality' chunk 131, replacing a hardcoded FSO _f102_motivestrings.cst); `uilive` gate (62→63). | r114/ |
| 115 | DONE | JOB SUBPANEL original strings + MONEY FLOATERS original glyphs + alert VALUE fixes (Live.iff STR# 136 'JobSubpanelLabels' + 137 'JobSubpanelPopupText'); `uijob` gate (63→64). | r115/ |
| 116 | DONE | CAS SCREENS in original strings + glyphs (UIText.iff 130 'DesignCharStrs', 128 'PickFamilyStrs', 129 'DesignFamilyStrs'); `uicas` gate (64→65). | r116/ |
| 117 | DONE | UCP TOOLTIPS in original strings (UIText.iff STR# 138 'VCtlTips', 24 English × langs; eyedropper stays port-authored); `uidtips` gate (65→66). | r117/ |
| 118 | DONE | THE REAL LOADING SCREEN ONLY, FROM FRAME ONE (MountBootOriginalDirect in the ctor — synchronous FAR1Archive read, setup.bmp byte-verbatim; modern loader plumbing DELETED; window title 'Simitone'→'The Sims'). | r118/ |
| 119 | DONE | THE FONT ISSUE CLOSED ENGINE-WIDE (OriginalVectorFont = MSDFFont subclass family over the R87-pinned tables; TextStyles scale==1.0 exactly on ladder sizes; every label/button/dialog/tooltip renders original .ffn glyphs); `uivfont` gate (66→67). | r119/ |
| 120 | DONE | BOTTOM-BAR COMPOSITION FIXED (MainPanel no longer overlaps DesktopUCP — panel placed at UCP right + 8; Window_ClientSizeChanged rewritten — GraphicsDevice.Viewport is single source of truth; black-band class fixed); `uimpanel` gate (67→68). | r120/ |
| 121 | DONE | THE ORIGINAL OPTIONS SCREEN (STR# 145 'optionstrs' carries the original's own (x;y) layout directives — Save(29;6)/Neighborhood(88;6)/Quit(159;6)/Graphics(29;58)/Sound(89;58)/Play(151;58) + anchors; full art kit from Res_CPanel.h/res_cpanel.rt); `uiopts` gate (68→69). | r121/ |
| 122 | DONE | THE ORIGINAL BUY-MODE CATALOG (STR# 150 'BuyModeCatalogSortTips' = six 8-entry sort blocks; subsorts STR# 200-207; art 80 members byte-verbatim; DISCOVERY: original RoomFlags bit order = Kitchen/Bedroom/Bathroom/Living/Misc/Dining/Outside/Study, CanonRoomToFlagBit={3,5,1,7,0,2,6,4}; two latent engine NREs fixed); `uibuy` gate (69→70). | r122/ |
| 123 | DONE | ODD BACKGROUND BEHIND THE STATUS BARS REMOVED (UIMotiveBar draw of kLiveModeGauge LiveGadget.bmp behind the 25px fills was wrong; negative-space proof: no track member for motive bars; fills drawn straight on panel). | r123/ |
| 124 | DONE | THE ORIGINAL CATALOG ITEM DESCRIPTION PANEL (UIQueryPanel rebuilt; kCatalogPopupBack PopupInfo.BMP 559x127 + PopupInfoTiles + STR# 160 'CatalogRatings'; ratings/flags derived from the corpus, field 80-86 signed; negative-space: [14..19] unencodable); `uidesc` gate (70→71). | r124/ |
| 125 | DONE | ORIGINAL LIVE-MODE OBJECT HOVER TOOLTIPS (STR# 159 'ObjectTTs' 9 entries; classification is pure TTAB data — flags TS1NoChild 0x10/TS1NoAdult 0x40/TS1AllowCats 0x200/TS1AllowDogs 0x400; ObjectTooltipReason walks raw tree tables); `uitt` gate (71→72). | r125/ |
| 126 | DONE | ORIGINAL BUILD-MODE CATALOG SUBCATEGORY BUTTONS (STR# 139 'BldTips' + 11 plaques toolBtn*.bmp; toolBtnTerrain loads full frame); `uibuild` 73-check. | r126/ |
| 127 | DONE | ORIGINAL BUILD-MODE TERRAIN TOOL ITEMS (the 4 cell plaques + 4 popup thumbs; raise/lower SPLIT restored; FreeSO submodule 0c1b1ebf); `uibldt` 74-check. | r127/ |
| 128 | DONE | ENGINE DECODE: STR# 159 'ObjectTTs' consumer recovered (cObjPickerTool ctor 0x186450 bulk-loads engine indices 1..9; TOOLTIP LADDER in TryClick 0x1850f0: adults(112)→kids(116)→dogs(124)→cats(128)→pets(132)→people(136); [0] vs [8] SETTLED STRUCTURALLY — [0] is the TERMINAL DEFAULT, [8] a narrow pet-only case). | r128/ |
| 129 | DONE | THE ENGINE LADDER PORTED (R128 finished/corrected): rung helpers = ObjSelector::{People,Pets,Cats,Dogs,Children,Adults}Only (0x103870-0x1041b0); ppc_decode 24-bit LI×4 fix + branch-label polarity fix; the full consumer map — cObjPickerTool::Release 0x1850c4 runs ONLY at mode this+104==31; [8] pet-only, [0] human default, [3]=pet self-hover; uitt extended, engine-exact classifier. | r129/ |
| 130 | DONE | THE ORIGINAL ROOF PANEL (7 UIGraphics.far members + GameData/Roofs corpus + STR# 147 'roofpanelstrs' with layout directives; cWinRoofPanel::TSOnCommand → cRoofLayer::SetPitch float table 0x5a3bbc: flat 0.0/shallow pi/5/medium 3pi/20/steep pi/10; RenderRoofPolys divides by pitch — the engine's SHALLOW is the largest); `uiroof` 75-check. | r130/ |
| 131 | DONE | THE LIVE-TAB MOOD GAUGE + ROOF PAGES (cWinPeople cluster mapped; kLiveModeGauge 4911 LiveGadget.bmp 108x100; TSPaint float-eased value chain; sign-flipped mapping: positive→GREEN/negative→RED; the engine's gauge caption is STR# 154 [15]); `uigauge` 76-check. | r131/ |
| 132 | DONE | THE PEOPLE PANEL'S REMAINING SURFACES (follow-sim + ratings bars + original HOUSE TAB; HouseStats class found at 0x8bba8 — score computation is the R133 target; House=0 until HouseStats decodes; Friend = FAMI.FamilyFriends raw count); `uirate` (76→77). | r132/ |
| 133 | DONE | THE HOUSESTATS COMPUTATION (HouseStats class full map: UpkeepScore 0x8bcd0, YardScore 0x8bda0, FurnishingsScore 0x8be30 piecewise ladder, SizeScore 0x8bf90; WRITER House::GetHouseStats 0x8c190 — room scan, LOT LADDER dimension <40 Small/<50 Medium/>=50 Large; ObjectModule::FillInObjectStats 0xe2630; corpus correction — House subpanel has its own tables 133/135/138); `uihouse` (77→78). | r133/ |
| 134 | DONE | THE OBJECT-SIDE STATS + ladder-hunt CLOSED (FillInObjectStats 0xe2630 line-by-line: upkeep dividend per-object condition weight, value routing indoor/outdoor via GetCurrentValue; Room::CollectObjectStats 0x125df0 + CollectTileStats 0x126220 + ComputeStats 0xe2820; DEFINITIVE NEGATIVE — no {key,val} ladder run exists in the image, so the five House score VALUES are permanently BSS). | r134/ |
| 135 | DONE | THE TOC-RESOLUTION HARVEST (resolve_toc.py; FOUR const pools recovered — [TOC-20376] gauge pool, [TOC-20380], [TOC-23512] FS clamp/scale, [TOC-23516]; THE EXACT GAUGE LAW: rating=clamp(symround(11*(100+mood)/201−5),−5,+5); height=&#124;rating&#124;·5 px; THE UPKEEP LAW: int(100·clamp(dividend/2^31,0,1)) guard objectCount). | r135/ |
| 136 | DONE | LAYOUTSCORE — the route-history decode and LAYOUT BAR LIVE (symbolic interpreter machine-derived the formula; House+48..92 generations; AddLayoutTick 0x8c4f0 gated tick%10==0; THE EXACT TAIL LAW at 0x8c2ec-0x8c3d8: score=(int)(100.0·clamp(1−2·flagged/max(1,total),0,1)); seed [TOC-29460] BSS-proven). | r136/ |
| 137 | DONE | THE LIVE HOUSE VALUE + LAYOUT FLAG CORRECTION (cFixedWorld::ComputeArchValue 0x15ea70-0x15eee0: two-story x lot-dim walk; WALL STRUCTURE + FLOORING at HALF, wallpaper FULL, roof=100 const; OriginalRouteHistory v2 corrected flag channel — LayoutScore = 100×(1−2× route occupancy); `uivalue` (78→79)). | r137/ |
| 2026-10-12 | DONE | WAVE-3 layout-history closure (post-release, main e50c4dc16+) — the r136 disclosed single-generation model is replaced by the decoded SIX-GENERATION retention: OriginalRouteHistory now keeps five seeded snapshot pairs (N0=200 from ORIG-02 layout-seed-n0.md) + the live accumulator pair, with the EnterLiveMode case-220 rotation (RotateLive) and the AUD-18-F ClearRouteHistory seed (snapshot pairs seeded, accumulator zeroed). **LayoutScore = (int)(100.0·clamp((SA−2·SB)/max(SA,1),0,1)) over all six generations.** The post-renovation transient decays over the rolling window; fresh-house outcome (0) and long-run ratio unchanged. uihouse math/live blocks and eng03flag sampler re-pinned to the multi-generation law; battery PASS 160/0 from the dist. | — |
| 138 | DONE | THE TEXT-RENDERING ROUND — texture upload premultiply bug fixed (OriginalGlyphFont.FromFFN uploaded white/straight-alpha while every draw path blends AlphaBlend premultiplied → glyphs saturated to solid blocks; FIX = premultiply ink v=nib·17, Color(v,v,v,v)); `uitext` (79→80). | r138/ |
| 139 | DONE | TEXT RESIDUALS RESOLVED by engine decode (cTSFont subsystem mapped: nibble law — NO per-font input, nibble used as-is, bold ceiling 221/255 = CANON; CP1252 boundary — engine only sees bytes, MapChar maps 27 Unicode equivalents; native scale — engine NEVER scales bitmap glyphs). | r139/ |
| 139b | DONE | USER FOLLOW-UP garble — root cause variablesans_16.ffn TABLE inconsistency (Internally inconsistent table); FIX = LadderPx drops 16 (15/16→_14, 17→_18). | r139/ |
| 140 | DONE | THE .FFN LOAD LAW ENGINE-DECODED + ATLAS-STREAM CORRECTION (MacGIMEX_open FNTF branch reads charTable=header+header[20]; shape record=header+header[28] 16-byte header; the port had read the atlas stream from shapeOff+12 since R96 — 4 bytes early; CORRECTION: +16; variablesans_16 was NEVER broken and 16 RESTORED to LadderPx). | r140/ |
| 141 | DONE | MAIN-GAME UI/UX RESTORATION — the control panel on ORIGINAL ENGINE ART (Res_CPanel.RT/.h gadget-id→art map recovered; kPeople/kObjects/kArch/kOptions sheets, mode patches 4803-06, kUniversalCPBkg 4910 PanelBack.bmp; SYMBOL INDEX 7,717 fns; cWinViewControl::Init 0x2b5f30; 'House.bmp live-button' codified misuse corrected). | r141/ |
| 142 | DONE | UI/UX IN ITS ENTIRETY — ROUND 2: DIALOGS, PIE MENU, AND BOTTOM-BAR COMPOSITION ON DECODED ENGINE LAW (4 RT/.h pairs; MESSAGE-BOX LAW: cSimsApp::MessageDialog 0x253cf0→cTSWinMsgBox, fill RGB(0,0,82), content-sized min-width 200, **PositionButtons 0x5234b0 row law** (maxW=widest, spacing=(w−32−n·maxW)/(n+1) clamped ≥16); PIE LAW: cTSPieMenu radius 90, label RGB(187,187,187), **SLICE QUANTIZER (Layout 0x5290c0): 0→1, 1-2→+2, 3-4→+4, ≥5→+8 slices**; the R141 'TSUI template DB' premise BUSTED (0x591440 = MSL operator new) — the real layout law is the hardcoded {x,y} anchor table (UCP re-anchored engine-exact: mode cascade 10,6/69,20/117,45/193,125; wall row y77-110; pause 15x15)). | r142/ |
| 143 | DONE | **THE ORIGINAL CREATE-A-SIM FLOW — ALL THREE SCREENS ON DECODED ENGINE LAW.** THE RLE8 UNLOCK (FreeSO submodule 8beea8e3): 237 of 282 UIGraphics.far .bmp members are BI_RLE8; Simitone.Desktop ImageSharp rendered them opaque — real transparency is **MAGIC-PINK PALETTE ENTRIES** (AddPersonBtn idx1=19048px, ScrollBar idx2), while PALETTE INDEX 0 is an ordinary navy fill (PickBkg 66% idx0, opaque); new FSO.Files.BmpRLE8 exact decoder runs first in WinDataFromStreamP. **CAS 13-button anchor table** with SetImage(4,1) per-state dims (Adult 57x100 (497,101), Child 92x90 (406,111), Female 57x121 (499,274), Male 98x111 (400,274), skin row 57/46/55x71 at 499/452/396,202, body/head arrows); Done/Cancel = cTSSystemButtons on NbhdTileBtn 200-wide at (111,529)/(400,529); **PERSONALITY LED strips**: 5 rows at (228,131+32i), LIT WIDTH = value·6 clamped 59 (10th pip 1px short), **CLICK ZONES split at value·5 (draw/click pitch mismatch = engine quirk)**, POOL 25 down-counter with deny-sound at 0, trait offsets {7,6,3,5,2}; **ZODIAC**: ComputeZodiacSign @0x172170 = **nearest-archetype squared-Euclidean**, table read from data 0x4c144 (Aries 5,4,8,5,3 … Pisces ZERO row = unassigned sims are Pisces), names STR# 'signs' cat 7; FAMILY/PAF anchors (Add 139x103 (593,103)/Delete 149x99 (594,208)/Edit 137x99 (594,311); 8 slots 85x105 vertical-first grid (111,121)-(551,481); PickBkg CENTERED offX=max(0,(winW−800))/2; cards 3x 690x68, member strip 45x68 (110+i·48,14) wrap 494). `uicasorig` 85 checks ALL PASS. | r143/ |
| 218 | DONE | THE VIEW PIE WIRED (UIOriginalViewPie tear-off radial on the decoded cDDDSimsView+0x13c cTSPieMenu; `uiviewpie` 126→127). | r218/ |
| 217 | DONE | THE PHANTOM VIEW-MENU FLYOUT — R193's model CORRECTED AND RETIRED (the four ViewMenu sheets are cTSPieMenu ITEMS, rendered only inside the pie popup). | r217/ |
| 216 | DONE | THE R215 RESIDUAL ROUND — three corrected laws (slider trough 11..119; friend digit ON THE MONEY LINE SetArea(moneyLeft−w, moneyTop, moneyLeft, moneyTop+fontH); two-layer floor rim). | r216/ |
| 215 | DONE | THE USER-DEFECT ROUND — four UI defects on decoded law (slider stretch /2 via SpriteBatch scale; friend digit on the fixed worst-case money gadget; floor preview edge flag-table law). | r215/ |
| 214 | DONE | THE PAUSED LABEL RE-SHOW CADENCE (DrawPause machine decoded — the symmetric blink NEVER existed; LIVE=steady, BUY/BUILD/CAMERA=one 3000ms flash, OPTIONS=steady; R204 disclosure retired). | r214/ |
| 213 | DONE | THE CHEAT HELP WINDOW (cTSWinCheatHelp 0x50e8c0; roster 92 registered commands verbatim; SetMode(1) corrected to SetInvalEveryFrame; `uicheathelp` 125→126). | r213/ |
| 212 | DONE | THE VITA IDLE PLAYBACK (cWinVitaBtnSolo::AnimatePet sequential-cycle machine ported; `uivitaplay` 124→125). | r212/ |
| 211 | DONE | THE FINAL HUNT ROUND — BASE-GAME 1.0 DECLARED (74-table UIText census + art sweep + residual audit; NOTHING found above disclosed-grade). | r211/ |
| 210 | DONE | TIER C 'ui-total' ENFORCEMENT GATE — THE CLOSER IS LIVE (UIArtProvenance + live-tree walker; PanelBack re-routed; three real gaps fixed on bring-up). | r210/ |
| 209 | DONE | VITA IDLE CANON RECOVERED (the four cWinVitaBtnSolo lists byte-verbatim — ORDER IS THE WEIGHT; adult 10/child 13/cat 12/dog 18; sway = SineGenerator 10000ms, amplitude BSS floats). | r209/ |
| 208 | DONE | SCREEN-SWITCH LAW (base-game switches are DIRECT constructions — no wipe; the ONLY engine transit art is the expansion taxi/cWinPictureSplashDialog; UITransDialog deleted). | r208/ |
| 207 | DONE | CHEAT BAR ON THE ENGINE LAW + two ledger retirements (cTSWinTextEdit2: (20,20) 200x21, opaque RGB(64,93,95), WHITE, cap 255, chord ctrl+shift+C **or B**). | r207/ |
| 206 | DONE | LOT-ENTRY PERMANENT CONTROL PANEL — the R199 residual CLOSED (cSimsApp::RebuildControlPanel 0x24de40 builds the panel from frame one; no hidden-then-reveal). | r206/ |
| 205 | DONE | FAMILY FRIEND COUNT READOUT + ABOUT DIALOG — Tier B STR# 162 CLOSED (readout = PostChildDraw 0x2b57c0, count via GetFamilyFriendCount 0x20c770, gadget directly BELOW the money readout; r144 '+0x1a8 = clock' CORRECTED — +0x1a8 IS the friend readout; `uifriend` 118→119). | r205/ |
| 204 | DONE | SYSTEM STRING FAMILIES + CHARTER SWITCH TO CLOSE-OUT (STR# 148 'Pause' + 152 'DefaultDialogButtons'; the pause label = a BLINK — timer 3000ms one-shot; R205 ported the friend count). | r204/ |
| 203 | DONE | DIRT-TOOL ERROR LAW (STR# 149 'DirtToolErrs'; cDirtTool::UpdateErrorMessage @0x18d230; the engine cost readout is BLACK; unaffordable = [5] 'Insufficient funds'; port-invented DarkRed RETIRED). | r203/ |
| 202 | DONE | **SIM VISIBILITY RESOLVED** — R201's diagnosis CORRECTED; the real defect = PPXDepthEngine.RenderPPXDepth non-MRT branch leaving the DEPTH target bound (sims drew into the depth buffer); FIX = one symmetric line (restore color target) + `simvis` rewritten framing-correct and PROMOTED into the default suite (115→116 checks). | r202/ |
| 201 | DONE | SIM-INVISIBILITY DIAGNOSIS (R201's 'Vitaboy draws zero fragments' — the test camera never framed the sims; they project below the viewport). | r201/ |
| 200 | DONE | FOLLOW-SIM LAW (cWinPeople::TrackPerson @0x28bf00/StopTracking @0x288740 — the kTrackingTarget crosshair marks the TRACKED sim's WEBCAM PORTRAIT, not a gauge button). | r200/ |
| 199 | DONE | SAME-MODE CLICK LAW / DESKTOP LIVE DUAL-MOUNT (CPState::SetMode @0x210790 no-ops a same-mode force-0 click). | r199/ |
| 198 | DONE | TOOLTIP BOX + ERROR-COLOR UNIFICATION (ONE cDefaultTTWindow singleton — NORMAL=BLACK, ERROR=RED; R145 green is catalog context; five TSO branches route to R129 ladder). | r198/ |
| 197 | DONE | NATIVE MOVE-IN LOT HANDLER — lot-query chrome CLOSED (cWinNeighborhoodVC::MoveInModeLotHandler recovered; STR# 132 captions; the 45x68 member strip belongs to the PAF cards (R143)). | r197/ |
| 196 | DONE | UIBIGBUTTON → WINBTN ON THE DESKTOP PATH (WinBtn.bmp 260x33 = 4x65x33, the UIButton 3-slice 4-state stretch, the R142 law). | r196/ |
| 195 | DONE | SYSTEM CHROME ENGINE-WIDE (cWinOptions never consumes Sys sheets; every standard modal = template 3008 PopupInfoTiles 36x36 12px nine-slice; cTSWinScrollbar fully decoded but NO live base-game consumer — later consumed by UIOriginalTextList; `uisyschrome` 114 checks). | r195/ |
| 194 | DONE | SPEECH BALLOONS + EVENT CHROME (GenerateSpeechIcons groups 2000/4000, kSpeechMediumBmp 9000 16x16 + kSpeechLargeBmp 9001 32x32; TragedyMask 9005 = gzi path). | r194/ |
| 193 | DONE | SMALL MOUNTS (kTallSubpanel 4918 TallSubPanel.TGA 600x150 mounted at (0,-50) LIVE-only; ViewMenu sheets DECODED as cTSPieMenu ITEMS by R217). | r193/ |
| 192 | DONE | PHONEBOOK DIALOG (CWinPhoneBook: template-3008 modal 652x426, kPhoneBookBkg 620x356 at (16,35); STR# 180; desktop routes to original). | r192/ |
| 191 | DONE | PEOPLE PIE (PieFace1 800 byte-verified as a PURE MAGENTA KEY MASK; MaxRadius 150/InactiveRadius 35/four colors/8 compass slots; TrackPerson law). | r191/ |
| 190 | DONE | SCRAPBOOK + SNAPSHOT CAPTURE (cWinScrapbook decoded + ported: template 1200 board 700x560, six sheet buttons at STR# 144 anchors, capture law; PIP contract recovered R240). | r190/ |
| 189 | DONE | ROOF FLOW + ARROW + NATIVE-SIZE PINS (two-row flow; next-page arrow 800/non-800 coordinates). | r189/ |
| 188 | DONE | HOUSE FURNISHINGS/YARD END-TO-END (Furnishings closed on the Build.iff data law; Yard verified no correction needed). | r188/ |
| 187 | DONE | MOTIVE/RELATIONSHIP DELTA-HISTORY LAW (cAverageHistory exact record law + cWinDeltaMeter five-slot state machine + five arrow masks). | r187/ |
| 186 | DONE | HOUSE SCORE CURVES (House::Initialize 0x08d3e0 builds the two 8-slot PiecewiseFns from Global.iff STR# 505 strings 1/2). | r186/ |
| 185 | DONE | PEOPLE-PANEL ROUTE GUARDS (native cWinPeople::SetPanel route guards ported; relationship comparator decoded). | r185/ |
| 184 | DONE | LIVE JOB-FAMILY NATIVE LAW (the five mutually exclusive surfaces behind the desktop Live Job tab with exact rectangles). | r184/ |
| 183 | DONE | CAS SYSTEM-BUTTON LAW (cTSSystemButton state-sheet law: 45/1/45 composition + caption geometry). | r183/ |
| 182 | DONE | STUDIO CATEGORY RECHECK (the unopened Studio Town BUY main row; non-sequential resource table honored). | r182/ |
| 181 | DONE | VISIBILITY CONTRACT (UIElement.Visible not enforced by the framework; static coverage fixed + runtime uivis gate). | r181/ |
| 180 | DONE | CHILD REPORT CARD LAW (the port-authored three-block layout replaced by the engine-decoded child branch). | r180/ |
| 179 | DONE | JOB TITLE IS DATA, NOT A CHILD (cWinSubpanelJob::Init distinguishes string-table data from attached controls). | r179/ |
| 177 | DONE | OBJECTDIALOG ICON-SELECTOR LAW (selector dispatch + common image composition decoded). | r177/ |
| 176 | DONE | STUDIO/MAGIC BUY-CATEGORY CORRECTION (resource-ID arrays are NOT sequential). | r176/ |
| 175 | DONE | OBJECTDIALOG HEADLINE + PORTRAIT LAW (the Easel dialog 'Congratulations!' headline on the native picture/avatar dialog class). | r175/ |
| 174 | DONE | NATIVE OPTIONS + LOT SELECTION (desktop lot dispatch directly; empty-lot refusal via STR# 133). | r174/ |
| 173 | DONE | NEIGHBORHOOD + CAMERA PARITY (camera.bmp 116x29 4-state at (175,95), CameraPatch.bmp 55x100 + F4 routing; cWinCamPanel 804x100 band). | r173/ |
| 172 | DONE | TOOLTIP + CATALOG DESCRIPTION COMPOSITION REPAIR (original cDefaultTTWindow law: face 1 size 7, box textWidth+6 × charHeight+2, paint x=3/y=1). | r172/ |
| 171 | DONE | REMAINING PRIMARY UI PANEL REPAIR (desktop child replacement follows the atomic cWinCPanel/cWinPeople law). | r171/ |
| 170 | DONE | PRIMARY UI COMPOSITION REPAIR (the gray neighborhood frame was case-sensitivity — requested 'LargeBack.bmp' vs stored 'largeback.bmp'). | r170/ |
| 169 | DONE | DESKTOP UI GEOMETRY REPAIR (exact cWinViewControl patch offsets Live 7,30/Buy 64,35/Build 113,51/Options 183,83; Pause 4x2 rows). | r169/ |
| 168 | DONE | GENERIC SIMS CALL 14 CERTIFIED EXACT (jump-table 14 → 0xf273c; 'HouseRadioStationEqualsTemp0' performs no comparison). | r168/ |
| 167 | DONE | GENERIC SIMS CALL 13 VALID OWNED-CORPUS LAW RECOVERED (jump-table 13 → 0xf26d8; CleanupPeople visits every object except target; ~~the port deliberately throws on the destructive path~~ superseded by SIM-21: the destructive path now runs on the native law). | r167/ |
| 166 | DONE | GENERIC SIMS CALL 12 RECOVERED AND IMPLEMENTED (TryGenericSimCall jump-table 12 → file 0xf2608; BuildRotationLookup fixed-64 law r0=(x,y), r1=(63−y,x), r2=(63−x,63−y)). | r166/ |
| 165 | DONE | ORIGINAL SCHOOL-BUS DAY TRACED END-TO-END (CarPortal BCON 4097 school hours; Cassandra obj16/GUID c1207913; grade 4→3). | r165/ |
| 164 | DONE | ATOMIC TS1 SAVED-CONTINUATION REPAIR (OBJM stacks no longer match Complete Collection replacement BHAVs; 245 instances/522 frames scanned). | r164/ |
| 163 | DONE | ENGINE SEMANTICS SWEEP + CAREER RETURN (PersonData [0..100] normalization + TS1 raw-word semantics; travel inventory GUID 10/11 replacement; distinct dynamic free-will variants; TreeAd JIT scopes 12/28/29). | r163/ |
| 159 | DONE | THE PEOPLE-PANEL COMPLETION — UI-100% round 1 (relationship subpanel rewritten on original art + decoded law: sort plaques 4108-4111, marker priority deep-heart>heart>smiley, kRelBars, STR# 240, cWinDeltaMeter). | r159/ |
| 160 | DONE | THE NEIGHBORHOOD NAVBAR — UI-100% round 2 (kNghBarBkg banner 800x52; the 14-button law + STR# 151 + destination tables 169/170/173/174; kLargeMask surround). | r160/ |
| 161 | DONE | THE BUDGET WINDOW — UI-100% round 3 (cWinBudgetDlg decoded + ported: the funds readout IS the button, STR# 146 anchors/rows/tooltips, uniform max-width column law). | r161/ |
| 162 | DONE | THE HELP WINDOW + kHelpBtn — UI-100% round 4 (cWinHelp + kHelpBtn 4993 mounted at the r144 anchor (5,164); FRAME CORRECTED R195). | r162/ |
| 159s | DONE | UI-100% SCOUT PHASE (3 parallel Hermes agents; client census + original-canon gap list + engine decode of unported windows). | r159/ |
| audit | DONE | EVIDENCE-TREE AUDIT (no code change): tools/iff-dump pruned 275MB→28MB (kept every round doc/errata/decode/canon table + last four rounds' logs). | iff-dump/README.md |
| 158 | DONE | THE CARPOOL COUNT LAW CLOSED — '18 CARS' WAS ONE CAR (4106 has exactly ONE caller — 4100). | r158/ |
| 157 | DONE | THE COMMUTE-TAIL ROOT CAUSE NAMED + FIXED — THE CARPOOL IS LIVE (instrumented 'process' 4100 per-person). | r157/ |
| 156 | DONE | THE COMMUTE WINDOW FIXED + NATIVE BRANDING (4100 triggers 4106 at Global[0]==Attr[0]−2). | r156/ |
| 155 | DONE | THE CAREER COMMUTE TAIL: MISATTRIBUTION CORRECTED — the r151-r154 '4102 At Work + 4106 Create Car observed live' record was FALSE (probe attributed frames by owning IFF filename). | r155/ |
| 154 | DONE | **THE INTEREST-SCALE SAGA RESOLVED — the Hot Date migration pipeline is REAL and WIRED** (the R153 recommendation; the research agents' 'dead script' verdict overturned by gate decode). | r154/ |
| 153 | DONE | THE ALL-10 SENTINEL: VERDICT = IT NEVER EXISTED (gate-polarity correction; two parallel decode agents). | r153/ |
| 152 | DONE | THE ENGINE CREATION DIALECT — cXPerson::RandomizeAllInterests PORTED (r151 loop-2 '{1,2,3}' correction). | r152/ |
| 151 | DONE | EXPANSION INTERESTS — ALL 19 WIDGETS ON THE DECODED LOW-INDEX WORDS (the R150 premise BUSTED: the 'expansion-era runtime build' was the original). | r151/ |
| 150 | DONE | INTEREST VALUES LIVE — THE 'init traits' TREE LAW (the R145 'SDSC import' premise BUSTED by data: house IFFs carry NO sim data chunks — House05.iff walk). | r150/ |
| 149 | DONE | PLAQUE ENABLE MATRICES — EMPTY CATEGORIES GRAY OUT (cWinCatalog::Init 0x26bd3c-0x26c374: seven 8x8 byte matrices). | r149/ |
| 148 | DONE | UI 'SOMETHING IS MISSING' DEEP-DIVE — skeptic audit + buy-band interaction chrome. | r148/ |
| 147 | DONE | FAR-ZOOM BLACK ROAD/GROUND — ROOT CAUSE + FIX (the terrain render was NEVER broken; R146's harness forced states the live game never enters). | r147/ |
| 146 | DONE | EXPANSION BUY BANDS + FAR-ZOOM TERRAIN follow-up (BuyCOne..; the 'road/ground black at zoom' root causes). | r146/ |
| 145 | DONE | BUY/BUILD TOOLBARS + INTEREST/GIFT PANELS on decoded engine law (three R144 corrections: the 12 build tools NEVER flow statically; interest/gift laws). | r145/ |
| 144 | DONE | MAIN-UI ACCURACY PASS — the live toolbar on decoded engine law (cWinViewControl patch offsets; 'LargeBack.bmp' case-sensitivity; the money/friend gadget +0x1a8 attribution corrected relation). | r144/ |
| 94 | DONE | Evidence dir tools/iff-dump/r94/ retains 5 files (r94-dt-tspaint.txt, r94-dt-tspaint-full.txt, r94-initcars-full.txt, r94-percar.txt, r94-tspaint-map.txt) but NO ledger row was ever written for a 'round 94' — the number is skipped between 93 and 95; nothing to recover. | r94/ |
| 178 | DONE | (round number skipped — no evidence dir, no ledger row; nothing to recover; MAP-04 note). | — |
## Rules of engagement

- Label honestly; correct past claims when data disproves them.
- Every claim needs a source: web, engine source, or staged-data scan under tools/.
- Evidence logs live in tools/iff-dump/ (tracked — commit normally, no git add -f needed).
- Never weaken a pin; extend it and keep the -autotest suite green.

(Relocated here from mid-table by MAP-04 2026-09-15 — the header sat between round rows
128 and 129, breaking the round-history table. Moved to the file tail in the
2026-10-09 PARITY sweep, with the doc-only rows 94/178 re-joined to the table end
and the table's stray blank lines removed so the ledger renders as one table.)
