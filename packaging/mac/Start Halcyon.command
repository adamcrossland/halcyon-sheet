#!/bin/bash
# Starts Halcyon Runner's Files on macOS.
# The first time, macOS may refuse to open this file: right-click it and choose Open.
cd "$(dirname "$0")" || exit 1

# Files downloaded from the internet are quarantined; clear that for this folder only.
xattr -dr com.apple.quarantine . 2>/dev/null

# Apple Silicon Macs only run signed programs. The app is built on Windows, where it can't be signed,
# so give it a local ("ad-hoc") signature the first time.
if ! codesign --verify ./Halcyon 2>/dev/null; then
    echo "Preparing Halcyon for first run…"
    codesign --force --sign - ./Halcyon || { echo "Couldn't sign the app. See READ ME.txt."; read -r -p "Press Return to close."; exit 1; }
fi

chmod +x ./Halcyon
exec ./Halcyon
