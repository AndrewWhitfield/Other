#!/usr/bin/env bash
# build-macos.sh — Build and publish Restart.Intune for macOS
# Run this script on a macOS host after copying the solution folder.
#
# Prerequisites:
#   1. .NET 10 SDK:  https://dotnet.microsoft.com/download/dotnet/10.0
#   2. macOS workload: dotnet workload install macos
#
# Usage:
#   chmod +x build-macos.sh
#   ./build-macos.sh             # builds both arm64 and x64
#   ./build-macos.sh arm64       # arm64 only  (Apple Silicon)
#   ./build-macos.sh x64         # x64 only    (Intel)

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
APP_CSPROJ="$SCRIPT_DIR/src/Restart.Intune.App/Restart.Intune.App.csproj"

TARGET="${1:-all}"

check_prerequisites() {
    if ! command -v dotnet &>/dev/null; then
        echo "❌  dotnet SDK not found. Install .NET 10 from https://dotnet.microsoft.com/download/dotnet/10.0"
        exit 1
    fi

    DOTNET_VERSION=$(dotnet --version)
    echo "✅  .NET SDK: $DOTNET_VERSION"

    if ! dotnet workload list 2>/dev/null | grep -q "^macos"; then
        echo "⚠️   macOS workload not detected. Installing..."
        dotnet workload install macos
    else
        echo "✅  macOS workload installed"
    fi
}

restore() {
    echo ""
    echo "📦  Restoring packages..."
    dotnet restore "$APP_CSPROJ" -f net10.0-macos
}

publish_arch() {
    local ARCH="$1"
    local PROFILE="osx-$ARCH"
    echo ""
    echo "🔨  Publishing $PROFILE..."
    dotnet publish "$APP_CSPROJ" \
        /p:PublishProfile="$PROFILE" \
        -f net10.0-macos \
        --nologo
    echo "✅  Published → artifacts/macos/$PROFILE/"
}

check_prerequisites
restore

case "$TARGET" in
    arm64) publish_arch arm64 ;;
    x64)   publish_arch x64 ;;
    all)
        publish_arch arm64
        publish_arch x64
        ;;
    *)
        echo "Unknown target '$TARGET'. Use: arm64 | x64 | all"
        exit 1
        ;;
esac

echo ""
echo "🎉  Done. Artifacts:"
find "$SCRIPT_DIR/artifacts/macos" -type f 2>/dev/null | sort
