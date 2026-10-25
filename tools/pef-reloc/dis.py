#!/usr/bin/env python3
"""UI-41 disassembler wrapper with CORRECT bc polarity.

tools/iff-dump/ppc_decode.py prints swapped eq/ne for B-form branches (the
known SIM-12 pitfall: '40 82' = bne cr0, '41 82' = beq cr0). This wrapper
decodes branch conditions straight from BO/BI so the pop-gesture dispatch
chain reads correctly.
"""

import os
import struct
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, os.path.join(HERE, "..", "iff-dump"))

import pef_load  # noqa: E402
import ppc_decode  # noqa: E402

COND = {0: "lt", 1: "gt", 2: "eq", 3: "so"}


def bc_str(w, addr):
    """Decode a branch-conditional word into (taken_addr_string, note)."""
    bo = (w >> 21) & 31
    bi = (w >> 16) & 31
    crf = bi >> 2
    cond = COND.get(bi & 3, "?")
    aa = (w >> 1) & 1
    lk = w & 1
    target = (w & 0xFFFC) if aa else (addr + (w & 0xFFFC if not (w & 2) else 0))
    # sign-fix for relative
    if not aa:
        off = w & 0xFFFC
        if off & 0x8000:
            off -= 0x10000
        target = addr + off
    if bo in (12, 15):       # branch TRUE
        name = f"b{cond}"
        if crf:
            name += f" cr{crf}"
    elif bo in (4, 6, 7):    # branch FALSE
        name = f"bn{cond}"
        if crf:
            name += f" cr{crf}"
    elif bo in (16, 18, 19):  # dec-and-branch-always variants
        name = "bdz/bdnz"
    elif bo == 20:
        name = "bdz"
    else:
        name = f"bc bo={bo}"
    if lk:
        name += "l"
    return name, (f"0x{target:x}" if not (w & 2) else "ctr/lr")


def dis(start, end, annotate_toc=True):
    pef = pef_load.load_pef(verbose=False)
    data = pef["data"]
    img = pef["image"]
    cbase = pef["sections"][0]["containerOffset"]
    words = struct.unpack_from(f'>{pef["codeLen"]//4}I', data, cbase)
    patches = {o: (k, fw, v) for k, o, fw, v in pef["events"] if o not in (None, -1)}

    def patched(off):
        if off in patches:
            k, fw, v = patches[off]
            if k in ("C", "D", "bySection"):
                return fw + v
        return None

    out = []
    for a in range(start, end, 4):
        w = words[a // 4]
        t = ppc_decode.decode(w, a) or ".long"
        note = ""
        op = w >> 26
        if op == 16:  # bc — replace with correct polarity
            name, tgt = bc_str(w, a)
            t = f"{name} {tgt}"
        elif op == 18 and not (w & 2):  # b/bl
            off = w & 0xFFFC
            if off & 0x8000:
                off -= 0x10000
            t = f"{'bl' if (w & 1) else 'b'} 0x{a + off:x}"
        elif op == 32 and ((w >> 16) & 31) == 2 and annotate_toc:  # lwz rX, off(r2)
            off = w & 0xFFFF
            if off & 0x8000:
                off -= 0x10000
            doff = 0x8000 + off
            if 0 <= doff < len(img) - 4:
                v = patched(doff)
                if v is None:
                    v = struct.unpack_from(">I", img, doff)[0]
                note = f"   <== TOC[{off:#x}] -> {v:#x}"
        out.append(f"{a:08x}: {w:08x}  {t}{note}")
    return "\n".join(out)


if __name__ == "__main__":
    start = int(sys.argv[1], 0)
    end = int(sys.argv[2], 0)
    print(dis(start, end))
