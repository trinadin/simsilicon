#!/usr/bin/env python3
"""UI-41 — parse the PEF loader EXPORT symbol table of `The Sims Complete`.

The binary is unstripped: the loader section's export area carries
function/data symbols with names (this is where r217's
`cTSPieMenu::InsertString @0x5283f2` ids came from, via Ghidra). This parser
reads them directly (linear parse of the export symbol array; the export
hash table is skipped).

Export symbol entry (PEF spec, validated empirically below):
  class byte (0=abs,1=code,2=data,3=TOC,4=glue,5=TOC-glue)
  nameLength varint (7-bit chunks, high bit = continuation)
  name bytes
  value varint
  [class==1 code: one more varint follows (class-name-table count)]

Usage:
  python3 pef_exports.py            # summary + sanity report
  python3 pef_exports.py <regex>    # print matching symbols
"""

import os
import re
import struct
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, os.path.join(HERE, "..", "iff-dump"))

import pef_load  # noqa: E402

BIN = "game-data/The Sims/The Sims Complete"


def read_varint(buf, pos):
    val = 0
    while True:
        b = buf[pos]
        pos += 1
        val = (val << 7) | (b & 0x7F)
        if not (b & 0x80):
            return val, pos


def parse_exports():
    data = open(BIN, "rb").read()
    pef = pef_load.load_pef(path=BIN, verbose=False)
    sections = pef["sections"]
    ld = next(s for s in sections if s["kind"] == 4)
    lb = ld["containerOffset"]
    (mainSec, mainOff, initSec, initOff, termSec, termOff, impLibCount,
     impSymCount, relocSecCount, relocInstrOff, stringsOff, exportHashOff,
     exportHashPower, exportSymCount) = struct.unpack_from(">14I", data, lb)

    hash_size = (1 << exportHashPower) * 2
    pos = lb + exportHashOff + hash_size
    end = lb + ld["containerLength"]

    syms = []
    for n in range(exportSymCount):
        cls = data[pos]; pos += 1
        nlen, pos = read_varint(data, pos)
        name = data[pos:pos + nlen].decode("mac-roman", "replace")
        pos += nlen
        value, pos = read_varint(data, pos)
        extra = None
        if cls == 1:  # code symbols carry one further varint
            extra, pos = read_varint(data, pos)
        syms.append((cls, name, value, extra))
        if pos > end:
            raise ValueError(f"overrun at symbol {n}")
    return syms, pos, end


def main():
    syms, pos, end = parse_exports()
    names_ok = sum(1 for c, n, v, e in syms if n and all(32 <= ord(ch) < 127 for ch in n))
    print(f"# parsed {len(syms)} export symbols (consumed to {pos:#x}, area end {end:#x}); "
          f"{names_ok} clean names")
    by_cls = {}
    for c, n, v, e in syms:
        by_cls[c] = by_cls.get(c, 0) + 1
    print("# by class:", by_cls)

    if len(sys.argv) > 1:
        pat = re.compile(sys.argv[1])
        for c, n, v, e in syms:
            if pat.search(n):
                print(f"  cls={c} {v:#010x} extra={e} {n}")


if __name__ == "__main__":
    main()
