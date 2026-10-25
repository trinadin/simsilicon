#!/usr/bin/env python3
"""UI-41 — CodeWarrior traceback-table scanner: name every function.

The Sims Complete's PPC code carries CW traceback tables immediately before
each function entry (mangled name, e.g. "3.UpdateViewFromCPState__
15cWinViewControlFP7CPState"). This scanner walks the code section,
extracts (entry, name) pairs, and serves lookups:

  python3 tb_names.py <regex>      # list matching functions
  python3 tb_names.py --at 0xADDR  # name the function containing ADDR
"""

import os
import re
import struct
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, os.path.join(HERE, "..", "iff-dump"))

import pef_load  # noqa: E402


def scan():
    pef = pef_load.load_pef(verbose=False)
    data = pef["data"]
    cbase = pef["sections"][0]["containerOffset"]
    code = data[cbase:cbase + pef["codeLen"]]
    # Traceback tables end flush at the entry. CW table layout (what we need):
    # ... [len byte] name ... \0 [pad to 4] ENTRY. The name is preceded by a
    # one-byte count of LEADING dots/underscores trimmed (e.g. "3." = 3 chars
    # of "__" prefix elided). Scan every printable run ending in \0 with 4-byte
    # padding before a plausible entry (word-aligned, next word is an
    # instruction, not a string).
    funcs = []
    n = len(code)
    i = 0
    pat = re.compile(rb"[\x20-\x7e]{6,120}\x00")
    for m in pat.finditer(code):
        end = m.end()
        # entry = next 4-byte boundary after the NUL (0-3 pad)
        entry = (end + 3) & ~3
        if entry + 4 > n:
            continue
        name = m.group()[:-1].decode("latin1")
        # mangled names contain __ and a class length tag like __15cWin...
        if "__" not in name and not re.match(r"^[a-zA-Z_]\w*$", name):
            continue
        w = struct.unpack_from(">I", code, entry)[0]
        # plausible first instruction: not another ascii-ish word and not zero
        if w == 0 or (0x20202020 <= w <= 0x7f7f7f7f and all(32 <= b < 127 for b in struct.pack(">I", w))):
            continue
        # avoid duplicates at same entry (keep first)
        if funcs and funcs[-1][0] == entry:
            continue
        funcs.append((entry, name))
    return funcs


def main():
    funcs = scan()
    print(f"# {len(funcs)} named functions", file=sys.stderr)
    if len(sys.argv) > 2 and sys.argv[1] == "--at":
        addr = int(sys.argv[2], 0)
        best = None
        for entry, name in funcs:
            if entry <= addr:
                best = (entry, name)
            else:
                break
        print(f"{best[1]}  (entry {best[0]:#x}, +{addr - best[0]:#x})" if best else "unknown")
        return
    pat = re.compile(sys.argv[1] if len(sys.argv) > 1 else ".")
    for entry, name in funcs:
        if pat.search(name):
            print(f"{entry:#08x}  {name}")


if __name__ == "__main__":
    main()
