#!/usr/bin/env python3
"""UI-41 — resolve the view-pie vtable (sec1+0x75d00) through the PEF
relocation stream and disassemble the true handler targets.

Both prior "vtable-unrecoverable" verdicts stopped at UNAPPLIED PEF
relocations:
  - r218 law doc: "the PEF vtable relocations needed to trace the dispatch
    were not preserved in the r142 evidence";
  - UI-37 re-proof: "pie vtable at sec1+0x75d00 unrelocated".
tools/iff-dump/pef_load.py already contains a validated relocation simulator
(12,941 blocks consumed exactly; ImportRun/SmByImport consume exactly the 535
imported symbols). This script runs the pie-region words through it, prints
the relocated entries, and disassembles every resolved code target so the
window-manager virtual dispatch (the pop gesture's caller) can finally be
traced.

Address model (per pef_load.py): fragment-relative — sectionC (code) base 0,
sectionD (data) base = code totalLength. A patched word's prepared value =
fileWord + sectionValueAtPatchTime. Known function ids quoted in evidence
(UpdateViewMenu @0x215b42, cDDDSimsView::Init @0x218c64) are fragment
offsets; the code section container starts at file 0x8E90.
"""

import os
import struct
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, os.path.join(HERE, "..", "iff-dump"))

import pef_load  # noqa: E402
import ppc_decode  # noqa: E402

BASE = 0x75D00           # sec1 (data) offset of the pie vtable per UI-37
REGION_BEFORE = 0x40
REGION_AFTER = 0x180
DISASM_WORDS = 96        # words per resolved target


def main():
    pef = pef_load.load_pef(verbose=False)
    image = pef["image"]
    sections = pef["sections"]
    code_sec = sections[0]
    code_container = code_sec["containerOffset"]
    code_len = pef["codeLen"]

    patches = {}
    for kind, off, file_word, val in pef["events"]:
        if off is not None and off != -1:
            patches[off] = (kind, file_word, val)

    print(f"# code section: container {code_container:#x} len {code_len:#x}; "
          f"data image {len(image):#x} bytes; {len(patches)} reloc patches")

    targets = []
    for off in range(BASE - REGION_BEFORE, BASE + REGION_AFTER, 4):
        if off + 4 > len(image):
            break
        raw = struct.unpack_from(">I", image, off)[0]
        kind = file_word = val = None
        if off in patches:
            kind, file_word, val = patches[off]
        note = ""
        resolved = None
        if kind in ("C", "D", "bySection"):
            resolved = file_word + val
            note = f" +{val:#x} => {resolved:#x}"
            if 0 < resolved < code_len:
                note += " (code)"
                targets.append((off, resolved))
        elif kind is not None:
            note = f" kind={kind} val={val}"
        print(f"data[{off:#07x}] {raw:08x}  raw{note}")

    print(f"\n# {len(targets)} resolved code targets in region")
    for data_off, target in targets:
        print(f"\n== data[{data_off:#x}] -> code {target:#x} "
              f"(file {code_container + target:#x}) ==")
        blob = pef["data"][code_container + target:
                           code_container + target + DISASM_WORDS * 4]
        for i in range(0, len(blob), 4):
            w = struct.unpack_from(">I", blob, i)[0]
            addr = target + i
            try:
                text = ppc_decode.decode(w, addr)
            except Exception as err:  # noqa: BLE001
                text = f"<decode err {err}>"
            print(f"  {addr:08x}: {w:08x}  {text}")
            # stop at a blr return past a few instructions
            if i >= 16 and text.strip().startswith("blr"):
                break


if __name__ == "__main__":
    main()
