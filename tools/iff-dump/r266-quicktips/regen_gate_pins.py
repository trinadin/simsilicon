#!/usr/bin/env python3
"""UI-39: regenerate the binary law pins for the ui39 gate (never hand-typed).

Ranges (raw file offsets in 'The Sims Complete', symbol addr = file + 2):
  setquicktips : cOptionsMgr::SetQuickTips body (60 B) - the 250/750 dwell law
  tspaint-tail : cTSWinBtn::TSPaint quick-tip pump (0x50cff0..0x50d040)
  mousenter    : cTSWinBtn::TSOnMouseEnter arm tail (0x50b8a0..0x50b8c8)
  mouseexit    : cTSWinBtn::TSOnMouseExit 0x14 send (0x50b460..0x50b48c)
  cmd13-block  : cWinOptions TSOnCommand popup block (0x282b04..0x282b8c)
"""
import hashlib, sys

BIN = ("/Users/nathannoom/Developer/Games/The Sims/worktrees/ui39/"
       "game-data/The Sims/The Sims Complete")

RANGES = [
    ("SetQuickTipsLawSha", 0x239ce0, 60),
    ("TSPaintTipPumpSha", 0x50cff0, 0x50d040 - 0x50cff0),
    ("MouseEnterArmSha", 0x50b8a0, 0x50b8c8 - 0x50b8a0),
    ("MouseExitSendSha", 0x50b460, 0x50b48c - 0x50b460),
    ("Cmd13PopupBlockSha", 0x282b04, 0x282b8c - 0x282b04),
]


def main():
    data = open(BIN, "rb").read()
    for name, off, ln in RANGES:
        print('private const string Ui39%s = "%s";  // [%#x +%d]' % (
            name, hashlib.sha256(data[off:off + ln]).hexdigest(), off, ln))


if __name__ == "__main__":
    sys.exit(main())
