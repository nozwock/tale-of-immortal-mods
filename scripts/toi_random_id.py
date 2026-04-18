#!/usr/bin/env python
# Can alias this to something like `toi-randid` and make it available in your global PATH.
# Use `toi localize` from https://github.com/nozwock/tale-of-immortal-tool instead

import random
from argparse import ArgumentParser
from ctypes import c_int
from pathlib import Path
from textwrap import dedent

import rapidjson  # python-rapidjson
from rapidjson import PM_COMMENTS, PM_TRAILING_COMMAS

json_loads = rapidjson.Decoder(parse_mode=PM_COMMENTS | PM_TRAILING_COMMAS)


def random_id():
    """
    ModTool$$RandomID
    """
    v = random.randint(1, 0x2A)
    if v > 0x14:
        # Overflowed negative values
        return random.randint(c_int(0x80000000).value, c_int(0xFFF0BDBF).value)
    else:
        return random.randint(100_000_001, 0x7FFFFFFF)


def main():
    parser = ArgumentParser(
        description=dedent(
            """Set random `id` to each json list entry.
            Creates `id` if not present in the entry, otherwise if `id` is 0, randomize it."""
        )
    )
    parser.add_argument("json", nargs="+", type=Path)

    args = parser.parse_args()

    for filepath in args.json:
        with open(filepath, encoding="utf-8") as f:
            obj = json_loads(f.read())

        for it in obj:
            if (id_ := it.get("id")) is None or str(id_) == "0":
                it["id"] = random_id()

        with open(filepath, "w") as f:
            rapidjson.dump(obj, f, ensure_ascii=False, indent=2)


if __name__ == "__main__":
    main()
