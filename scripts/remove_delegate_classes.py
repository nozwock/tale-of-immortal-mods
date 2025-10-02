#!/usr/bin/env python
"""
This will filter out compiler-generated delegate classes from ILSpy disassembled C# files for the Il2CppDumper's dummy dlls that honestly aren't
really useful and just add a lot of noise.
"""

import argparse
import re
from pathlib import Path
from typing import cast


def remove_delegate_classes_from_file(filepath: Path):
    """
    Relies on being properly formatted and starting/closing bracket always having same indentation level.
    """

    def should_start_removal(line: str):
        return re.search(r"\bprivate sealed class\b", line)

    def get_brace_indent(line: str):
        if re.search(r"{|}", line.strip()):
            return len(line) - len(line.lstrip())

    # I learned a while ago that utf-8 is not the default...
    with open(filepath, encoding="utf-8") as f:
        lines = f.readlines()

    result = []
    skip_mode = False
    brace_indent_level = None

    i = 0
    while i < len(lines):
        line = lines[i]

        if not skip_mode:
            # Check for attribute lines
            attr_lines = []
            while re.match(r"^\s*\[.*\]\s*$", line):
                attr_lines.append(line)
                i += 1
                if i >= len(lines):
                    break
                line = lines[i]

            if i < len(lines) and should_start_removal(line):
                brace_indent_level = get_brace_indent(line)
                skip_mode = True
                i += 1
                continue

            # No match, keep attribute lines + current line
            result.extend(attr_lines)
            result.append(line)
            i += 1

        else:
            if brace_indent_level is None:
                brace_indent_level = get_brace_indent(line)
            if get_brace_indent(line) == brace_indent_level and line.strip() == "}":
                skip_mode = False
                brace_indent_level = None
            i += 1

    with open(filepath, "w", encoding="utf-8") as f:
        f.writelines(result)


def process_folder(root_dir: Path):
    for root, _, files in root_dir.walk():
        for file in files:
            if file.endswith(".cs"):
                filepath = root.joinpath(file)
                remove_delegate_classes_from_file(filepath)
                print(f"Processed: {filepath.relative_to(root_dir)}")


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("-f", "--file", type=Path)
    parser.add_argument("-d", "--dir", type=Path, help="Directory with .cs files.")

    args = parser.parse_args()

    if (file := cast(Path, args.file)) and file.name.endswith(".cs"):
        remove_delegate_classes_from_file(file)
        print(f"Processed: {file}")
    if args.dir:
        process_folder(args.dir)
