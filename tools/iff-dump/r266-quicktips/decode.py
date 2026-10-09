#!/usr/bin/env python3
"""UI-39: decode the Quick Tips option + presenter family (read-only)."""
from pathlib import Path
import runpy
ROOT = Path(__file__).resolve().parents[3]
helper = runpy.run_path(str(ROOT / 'tools/iff-dump/r145/cdis.py'))
helper['md'].skipdata = True
OUT = Path(__file__).parent

# (name, symbol addr, len) — symbol addr is file offset + 2 (r145 convention)
FUNCS = [
    ('optionsmgr-setquicktips', 0x239ce2, 60),
    ('wincampanel-setquicktips', 0x265b82, 292),
    ('winoptions-setquicktips', 0x2800c2, 900),
    ('winpeople-setquicktips', 0x287eb2, 792),
    ('winsubpanelhouse-setquicktips', 0x2a0082, 144),
    ('winsubpaneljob-setquicktips', 0x2a1b02, 248),
    ('winsubpanelreportcard-setquicktips', 0x2a6232, 76),
    ('winsubpanelcatskills-setquicktips', 0x45c072, 144),
]
for name, sym, ln in FUNCS:
    (OUT / (name + '.txt')).write_text(
        helper['dis'](sym - 2, sym - 2 + ln) + '\n')
    print('wrote', name)
