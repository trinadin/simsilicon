#!/usr/bin/env python3
"""UI-39: dump STR# 140 'cammodestrs' (the camera panel quick-tip strings)."""
import struct, hashlib, sys

DEFAULT = ("/Users/nathannoom/Developer/Games/The Sims/worktrees/ui39/"
           "game-data/The Sims/GameData/UIText.iff")
TARGET_ID = 140
TARGET_LABEL = "cammodestrs"


def walk_chunks(data):
    chunks = []
    off = 0x40
    while off + 76 <= len(data):
        cc = data[off:off + 4]
        (sz,) = struct.unpack(">I", data[off + 4:off + 8])
        if sz < 76 or off + sz > len(data):
            break
        cid, flags = struct.unpack(">HH", data[off + 8:off + 12])
        label = data[off + 12:off + 76].split(b"\0")[0].decode("mac_roman")
        chunks.append((cc, sz, cid, flags, label, off))
        off += sz
    return chunks


def main(path):
    data = open(path, "rb").read()
    tgt = None
    for cc, sz, cid, flags, label, off in walk_chunks(data):
        if cid == TARGET_ID:
            tgt = (cc, sz, cid, flags, label, off)
            break
    if tgt is None:
        print("FATAL: STR# %d not found" % TARGET_ID)
        return 1
    cc, sz, cid, flags, label, off = tgt
    body = data[off:off + sz]
    print("=== STR# %d '%s' chunkSize=%d full-chunk sha256=%s ===" % (
        cid, label, sz, hashlib.sha256(body).hexdigest()))
    d = body[76:]
    (fmt,) = struct.unpack_from("<h", d, 0)
    print("format", fmt)
    if fmt == -3:
        (count,) = struct.unpack_from("<H", d, 2)
        p = 4
        idx = 0
        shown = 0
        langs = {}
        while shown < count and p < len(d):
            lang = d[p]; p += 1
            e = d.index(b"\0", p); val = d[p:e].decode("mac_roman"); p = e + 1
            e = d.index(b"\0", p); com = d[p:e].decode("mac_roman"); p = e + 1
            langs[lang] = langs.get(lang, 0) + 1
            if lang == 1:
                print("  %3d (%4d B)  %r  comment=%r sha256=%s" % (
                    idx, len(val.encode('mac_roman')), val, com,
                    hashlib.sha256(val.encode('mac_roman')).hexdigest()))
                idx += 1
            shown += 1
        print("total", count, "langs", sorted(langs.items()), "english", idx)
    else:
        (count,) = struct.unpack_from("<H", d, 2)
        print("count", count, "(pascal format)")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1] if len(sys.argv) > 1 else DEFAULT))
