@_default:
    just --list

build-gamelibs game_path version="":
    # https://github.com/nozwock/unity-gamelibs-builder
    unity-gamelibs build-package "{{ game_path }}" \
        --name TaleOfImmortal \
        --version="{{ version }}" \
        --version-prefix toi \
        --framework net472 \
        --strip-only \
        --keep-unity \
        --system-include "System.Web*.dll"
