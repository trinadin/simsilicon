#!/usr/bin/env python3
"""UI-41 — CodeWarrior traceback-table scanner: name every function.

The Sims Complete's PPC code carries CW traceback tables that FOLLOW the
function they name: the layout is [function][its table][function][its
table]... (review-verified semantically: the table naming a getter sits
after the getter's `blr`, and 0x20cfe0's `TSOnCommand__12cDDDSimsView`
string follows the (cmd, sender) dispatcher it names). This scanner
extracts (entry, name) pairs accordingly:

  python3 tb_names.py <regex>      # list matching functions
  python3 tb_names.py --at 0xADDR  # name the function containing ADDR

Pairing rule: each table's name belongs to the function that STARTED at the
first plausible instruction word after the PREVIOUS table's end. Entries
whose first word is 0x00000000 (alignment padding before the next function)
are accepted; the padding is skipped when locating that next entry.
"""

import os
import re
import struct
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, os.path.join(HERE, "..", "iff-dump"))

import pef_load  # noqa: E402

NAME_PAT = re.compile(rb"[\x20-\x7e]{6,120}\x00")


def plausible_entry_word(w):
    """Not ascii-text-like (tables/strings) — good enough as a function start."""
    b = struct.pack(">I", w)
    if all(32 <= c < 127 for c in b):
        return False
    return w != 0


def scan():
    pef = pef_load.load_pef(verbose=False)
    data = pef["data"]
    cbase = pef["sections"][0]["containerOffset"]
    code = data[cbase:cbase + pef["codeLen"]]
    n = len(code)
    tables = []  # (start, end_nul, name)
    for m in NAME_PAT.finditer(code):
        name = m.group()[:-1].decode("latin1")
        # mangled names contain __ plus a length tag, or are plain identifiers
        if "__" not in name and not re.match(r"^[A-Za-z_]\w*$", name):
            continue
        tables.append((m.start(), m.end(), name))
    funcs = []
    # FOLLOW semantics: table i's name labels the function that ENDS at
    # table i — i.e. the function whose ENTRY is the first plausible word
    # after table (i-1)'s end. entry_0 = the code start (or after padding).
    prev_end = 0
    for i, (s, e, name) in enumerate(tables):
        entry = (prev_end + 3) & ~3
        while entry + 4 <= s and struct.unpack_from(">I", code, entry)[0] == 0:
            entry += 4
        prev_end = e
        if entry + 4 > s:
            continue
        if not plausible_entry_word(struct.unpack_from(">I", code, entry)[0]):
            continue
        if funcs and funcs[-1][0] == entry:
            continue
        funcs.append((entry, name))
    funcs.sort()
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
