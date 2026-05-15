# Restart.Intune

A cross-platform desktop notification application that prompts end-users to restart their managed Windows or macOS device when required by Microsoft Intune policy. Built with [Avalonia UI](https://avaloniaui.net/) and .NET 10, it runs fully self-contained — no runtime installation is required on target machines.

---

## Table of Contents

- [Overview](#overview)
- [Solution Structure](#solution-structure)
- [Project Reference Map](#project-reference-map)
- [Project Details](#project-details)
- [Key Concepts](#key-concepts)
- [Environment Variables](#environment-variables)
- [Registry Usage (Windows)](#registry-usage-windows)
- [Prerequisites](#prerequisites)
- [Build Instructions — Windows](#build-instructions--windows)
- [Build Instructions — macOS](#build-instructions--macos)
- [Publish Profiles](#publish-profiles)
- [Artifact Output Locations](#artifact-output-locations)
- [Deployment Notes](#deployment-notes)
- [Known Limitations and Future Work](#known-limitations-and-future-work)

---

## Overview

When Microsoft Intune detects that a managed device requires a restart (e.g. after an update is installed), it launches this application. A notification window appears in the bottom-right corner of the primary monitor — always on top, with no taskbar icon — giving the user options to:

- **Restart Now** — triggers an immediate forced graceful restart after confirmation
- **Postpone** — defers the notification by a user-selected time slot (up to 5 times)
- **Remind Me Later** — closes the current notification session

The application supports two branded window variants (CBA and Bankwest) and a generic `RestartRequiredWindow`, all sharing the same business logic layer.

---

## Solution Structure

```
Restart.Intune/
├── src/
│   ├── Restart.Intune.App/               # Entry point — platform-agnostic bootstrapper
│   ├── Restart.Intune.Core/              # Shared business logic, models, interfaces
│   ├── Restart.Intune.UI/                # Avalonia UI layer — AXAML, ViewModels, Helpers
│   ├── Restart.Intune.Platform.Windows/  # Windows-specific service implementations
│   └── Restart.Intune.Platform.macOS/    # macOS-specific service implementations
├── tests/
│   └── Restart.Intune.Core.Tests/        # xUnit unit tests for Core logic
├── artifacts/
│   ├── windows/                          # Published Windows executable output
│   └── macos/
│       ├── osx-x64/                      # Published macOS Intel output
│       └── osx-arm64/                    # Published macOS Apple Silicon output
└── README.md
```

---

## Project Reference Map

```
Restart.Intune.App (net10.0-windows / net10.0-macos)
├── Restart.Intune.Core          (net10.0)
├── Restart.Intune.UI            (net10.0)
│   └── Restart.Intune.Core
├── Restart.Intune.Platform.Windows  (net10.0-windows)  ← Windows builds only
│   └── Restart.Intune.Core
└── Restart.Intune.Platform.macOS    (net10.0-macos)    ← macOS builds only
    └── Restart.Intune.Core

Restart.Intune.Core.Tests (net10.0)
└── Restart.Intune.Core
```

Platform project references in `Restart.Intune.App` are **conditional** — each OS only pulls its own implementation at build time, controlled by MSBuild `GetTargetPlatformIdentifier` conditions.

---

## Project Details

### Restart.Intune.App

| Property | Value |
|---|---|
| **Target Frameworks** | `net10.0-windows`, `net10.0-macos` |
| **Output Type** | `WinExe` (Windows) / `Exe` (macOS) |
| **Role** | Application entry point — bootstraps Avalonia and selects the main window |

Configures the Avalonia `AppBuilder` with platform detection, Inter font, and trace logging. No business logic resides here.

| Package | Version |
|---|---|
| `Avalonia` | 11.3.15 |
| `Avalonia.Desktop` | 11.3.15 |
| `Avalonia.Themes.Fluent` | 11.3.15 |
| `Avalonia.Fonts.Inter` | 11.3.15 |

---

### Restart.Intune.Core

| Property | Value |
|---|---|
| **Target Framework** | `net10.0` |
| **Role** | Platform-neutral shared library — ViewModels, service interfaces, models, configuration |

| Type | Purpose |
|---|---|
| `MainWindowViewModel` | `INotifyPropertyChanged` ViewModel shared by all windows |
| `ControlState` (enum) | `Default`, `PostponeMode`, `RestartMode` — drives button row visibility |
| `InstalledUpdate` | Model for a single update shown in the "About this update" tab |
| `WindowConfig` | Per-window config model (footer text, footer image path) |
| `WindowConfigLoader` | Loads per-window config from disk/registry (stub in current phase) |
| `RestartWindowBase` | Static class — all postpone business logic |

**Postpone rules:** max 5, 30-min increments, range now+30min to now+4hrs, button disabled at limit.

---

### Restart.Intune.UI

| Property | Value |
|---|---|
| **Target Framework** | `net10.0` |
| **Role** | Avalonia UI layer — AXAML windows, styles, converters, helpers |

| Window | Size | Description |
|---|---|---|
| `RestartRequiredWindow` | 530×480 px | Generic restart notification |
| `CBANotificationWindow` | 460 px wide | CBA-branded notification |
| `BankwestNotificationWindow` | 460 px wide | Bankwest-branded notification |
| `ConfirmRestartDialog` | Modal | Restart confirmation |

All windows: `ShowInTaskbar="False"`, `Topmost="True"`, `SystemDecorations="None"`, DPI-aware bottom-right positioning.

| Helper | Purpose |
|---|---|
| `MonitorHelper` | DPI-aware `PixelPoint` via `Window.RenderScaling` |
| `SystemOperations` | `shutdown /r /f /t 0` (Windows) / `shutdown -r now` (macOS) |
| `RegistryHelper` | Writes postpone time to `HKCU\SOFTWARE\RestartIntune\PostponeTime`; no-op on non-Windows |
| `HyperlinkNavigationHandler` | Opens help URLs via `IBrowserLauncher` |
| `IBrowserLauncher` / `DefaultBrowserLauncher` | Browser launch abstraction — injectable for tests |

---

### Restart.Intune.Platform.Windows

`net10.0-windows` — Windows-only service implementations. Linked to Windows builds only via MSBuild conditional `ProjectReference`.

### Restart.Intune.Platform.macOS

`net10.0-macos` — macOS-only service implementations. Requires the Apple SDK .NET workload. Linked to macOS builds only.

### Restart.Intune.Core.Tests

`net10.0` — xUnit 2.9.2 + Moq 4.20.72. Platform-neutral; runs on any OS.

---

## Key Concepts

### DPI-Aware Positioning

`Window.Bounds` is in logical DIPs; `WorkingArea` is in physical pixels. At 125% scaling a 530-DIP window is 662px wide. `MonitorHelper.GetBottomRightPosition()` multiplies by `Window.RenderScaling` to produce a correct `PixelPoint` at any DPI.

### `#if DEBUG` Guards

`SystemOperations.RestartMachine()` and `RegistryHelper.WritePostponeValue()` are behind `#if DEBUG / #else`. Debug builds write to `Debug.WriteLine` only — no real restart, no registry write.

### Postpone State Machine

`_postponesRemaining` starts at 5 per window. Each confirm decrements it; `RestartWindowBase.EvaluatePostponeAction()` returns the warning/disable state. Logic is independently unit-tested.

---

## Environment Variables

| Variable | Value | Effect |
|---|---|---|
| `RESTARTINTUNE_UI` | `avalonia` | Routes to Avalonia `RestartRequiredWindow` |
| `RESTARTINTUNE_UI` | _(absent)_ | Default (currently also Avalonia) |

```powershell
# Windows
$env:RESTARTINTUNE_UI = "avalonia"; .\Restart.Intune.App.exe
```

```bash
# macOS
RESTARTINTUNE_UI=avalonia ./Restart.Intune.App
```

---

## Registry Usage (Windows)

`HKCU\SOFTWARE\RestartIntune` → `PostponeTime` (`REG_SZ`)  
Written on postpone confirm in Release builds. Standard user permissions sufficient.

---

## Prerequisites

### Windows Development

| Requirement | Minimum | Notes |
|---|---|---|
| OS | Windows 10 x64 build 1607 | Windows 11 recommended |
| .NET SDK | 10.0.203 | `winget install Microsoft.DotNet.SDK.10` |
| IDE | Visual Studio 2026 (18.5+) | .NET desktop development workload |
| Git | Any | |

```powershell
dotnet --version   # 10.0.203 or higher
```

### macOS Development

| Requirement | Minimum | Notes |
|---|---|---|
| OS | macOS 12 (Monterey) | macOS 14 recommended |
| .NET SDK | 10.0.203 | `brew install dotnet` |
| Apple workload | — | `sudo dotnet workload install macos` |
| Xcode CLI Tools | 14+ | `xcode-select --install` |
| IDE | JetBrains Rider or VS Code + C# Dev Kit | VS for Mac is end-of-life |

```bash
dotnet --version && dotnet workload list && xcode-select -p
```

---

## Build Instructions — Windows

All commands from `C:\DEV\Restart.Intune\`

### Debug Build

```powershell
dotnet build Restart.Intune.sln -c Debug

# Windows only
dotnet build src\Restart.Intune.App\Restart.Intune.App.csproj `
  -f net10.0-windows -c Debug

# Run
.\src\Restart.Intune.App\bin\Debug\net10.0-windows\Restart.Intune.App.exe
```

> No restart or registry write in Debug builds.

### Release Publish (self-contained exe)

```powershell
dotnet publish src\Restart.Intune.App\Restart.Intune.App.csproj `
  -f net10.0-windows `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -p:PublishSingleFile=true `
  -p:PublishTrimmed=false `
  -p:IncludeNativeLibrariesForSelfExtract=true `
  -o artifacts\windows
```

### Using the Publish Profile

```powershell
dotnet publish src\Restart.Intune.App\Restart.Intune.App.csproj `
  /p:PublishProfile=win-x64 -f net10.0-windows
```

Profile: `src\Restart.Intune.App\Properties\PublishProfiles\win-x64.pubxml`

### Running Tests (Windows)

```powershell
dotnet test tests\Restart.Intune.Core.Tests\Restart.Intune.Core.Tests.csproj -c Release
dotnet test tests\Restart.Intune.Core.Tests\Restart.Intune.Core.Tests.csproj `
  -c Release --logger "console;verbosity=detailed"
```

From Visual Studio: **Test > Run All Tests** or **View > Test Explorer**.

---

## Build Instructions — macOS

> **Important:** `net10.0-macos` cannot be cross-compiled from Windows. Run all commands on a macOS host.

### Install the Apple Workload (one-time)

```bash
sudo dotnet workload install macos
dotnet workload list   # verify "macos" is listed
```

### Debug Build (macOS)

```bash
dotnet build Restart.Intune.sln -c Debug

# macOS only
dotnet build src/Restart.Intune.App/Restart.Intune.App.csproj \
  -f net10.0-macos -c Debug

# Run
./src/Restart.Intune.App/bin/Debug/net10.0-macos/Restart.Intune.App
```

### Release Publish — Intel (osx-x64)

```bash
dotnet publish src/Restart.Intune.App/Restart.Intune.App.csproj \
  -f net10.0-macos -c Release -r osx-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=false \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  -o artifacts/macos/osx-x64

# Via profile
dotnet publish src/Restart.Intune.App/Restart.Intune.App.csproj \
  /p:PublishProfile=osx-x64 -f net10.0-macos
```

### Release Publish — Apple Silicon (osx-arm64)

```bash
dotnet publish src/Restart.Intune.App/Restart.Intune.App.csproj \
  -f net10.0-macos -c Release -r osx-arm64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=false \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  -o artifacts/macos/osx-arm64

# Via profile
dotnet publish src/Restart.Intune.App/Restart.Intune.App.csproj \
  /p:PublishProfile=osx-arm64 -f net10.0-macos
```

### Running Tests (macOS)

```bash
dotnet test tests/Restart.Intune.Core.Tests/Restart.Intune.Core.Tests.csproj \
  -c Release --logger "console;verbosity=normal"
```

---

## Publish Profiles

```
src/Restart.Intune.App/Properties/PublishProfiles/
├── win-x64.pubxml       # Windows x64 — any host
├── osx-x64.pubxml       # macOS Intel — macOS host only
└── osx-arm64.pubxml     # macOS Apple Silicon — macOS host only
```

All profiles: `Release`, `SelfContained=true`, `PublishSingleFile=true`, `PublishTrimmed=false`.

---

## Artifact Output Locations

| Platform | Path | Binary |
|---|---|---|
| Windows x64 | `artifacts\windows\` | `Restart.Intune.App.exe` (~95 MB) |
| macOS Intel | `artifacts/macos/osx-x64/` | `Restart.Intune.App` |
| macOS Apple Silicon | `artifacts/macos/osx-arm64/` | `Restart.Intune.App` |

PDB files are co-located — keep for crash analysis, do not deploy to end-users.

---

## Deployment Notes

### Windows (Intune Win32 App)

- Wrap as `.intunewin` using the [Win32 Content Prep Tool](https://github.com/microsoft/Microsoft-Win32-Content-Prep-Tool)
- Deploy in **user context** — UI is per-session, writes go to `HKCU`
- **Do not use SYSTEM context** — UI will not be visible
- Binary is **unsigned** — sign with `signtool.exe` or whitelist via WDAC/Intune before deployment to suppress SmartScreen
- Minimum: Windows 10 x64 build 1607

### macOS (Intune Shell Script / PKG)

- Binary is **unsigned and unnotarised** — sign and notarise before distribution
- Deploy via Intune shell script or `.pkg`
- Must run in user session context

---

## Known Limitations and Future Work

| Item | Detail |
|---|---|
| `WindowConfigLoader` is a stub | Returns `null`; real JSON/registry implementation planned |
| `FooterLogoSource` binding incomplete | Typed `object?`; requires `IImage` for logo to render |
| No DI container | Direct instantiation; DI wiring is a future phase |
| WPF entrypoint not wired | Both `WindowLauncher` paths currently resolve to Avalonia |
| macOS platform services are stubs | `Platform.macOS` compiles but has no implemented logic |
| No code signing | Both artifacts unsigned; signing required for production |
| Trimming disabled | Avalonia 11.x trimming compatibility unverified |