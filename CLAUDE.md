# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.


# DO MUST
対話は日本語で行ってください。

## 1. タスク管理・準備段階
1. タスクの確認 - `.claude/TODO.md`でタスクを管理してください
2. タスク作成 - 必ず作業前にタスクはステップバイステップに分割して`.claude/TODO.md`に追加してから作業を開始してください
3. タスク詳細確認 - タスクに詳細が書いてある事があるので必ず確認してください

## 2. 実装段階
1. 優先度順に実装 - `.claude/TODO.md`に記載されているタスクは優先度が高い順に実装していってください
2. ステータス更新 - 作業中のタスクはステータスをIn Progressにし、作業が完了したものはDoneにしてください
3. コミット - タスクが完了したら、作業内容をコメントに入れてコミットしてください
4. 記録 - 実装の際に発生した問題や特記事項などは、そのタスク内のコメント欄に残しておいてください

## 3. 最終確認段階
1. 追加タスク確認 - タスクが終わったら、追加のタスクがないか確認してください
2. 継続作業 - 追加タスクがある場合は同様に進めてください




## Project Overview

Unity TimeMachine is a Timeline extension for Unity that enables complex operations like Pause and Repeat on Unity's Timeline. It's designed for live entertainment and virtual production scenarios.

## Core Architecture

### Package Structure
- **jp.iridescent.timemachine/**: Main package directory containing the TimeMachine Unity package
  - **Runtime/**: Core runtime components for Timeline control
  - **Editor/**: Custom Unity Editor extensions for TimeMachine
  - **Resources/**: Package resources and assets

- **Unity_TimemachineDemoProject~/**: Unity demo project showcasing TimeMachine functionality

### Key Components

1. **TimeMachinePlayer** (`Runtime/TimeMachinePlayer.cs`): Main controller that manages timeline assets and playback
2. **TimeMachineTrackManager** (`Runtime/TimeMachineTrackManager.cs`): Manages timeline tracks and clip events
3. **TimeMachineControlTrack** (`Runtime/TimeMachineControlTrack.cs`): Custom Timeline track for TimeMachine control
4. **TimeMachineControlClip** (`Runtime/TimeMachineControlClip.cs`): Timeline clip implementation
5. **TimeMachineControlMixer** (`Runtime/TimeMachineControlMixer.cs`): Mixer for blending timeline clips

### OSC Integration
- **TimeMachineOscReceiver**: Handles OSC (Open Sound Control) messages for external control
- **TimeMachineExtOscReceiver**: Extended OSC receiver functionality
- **TimeMachineOscDebugUI**: Debug UI for OSC communication

## Development Commands

### Unity Compilation
```bash
# Refresh Unity assets and compile (must be done through Unity MCP tools)
mcp__unity-natural-mcp__refresh_assets

# Check compilation errors
mcp__unity-natural-mcp__get_compile_logs
```

### Running Tests
```bash
# Run Edit Mode tests
mcp__unity-natural-mcp__run_edit_mode_tests

# Run Play Mode tests  
mcp__unity-natural-mcp__run_play_mode_tests
```

### Console Management
```bash
# Clear Unity console logs
mcp__unity-natural-mcp__clear_console_logs

# Get current console logs
mcp__unity-natural-mcp__get_current_console_logs
```

## Dependencies

Key Unity packages and dependencies (from `Unity_TimemachineDemoProject~/Packages/manifest.json`):
- Unity Timeline (com.unity.timeline: 1.8.7)
- uOSC for OSC communication (com.hecomi.uosc)
- UniTask for async operations (com.cysharp.unitask: 2.1.0)
- Universal Render Pipeline (com.unity.render-pipelines.universal: 17.0.4)
- Unity Test Framework (com.unity.test-framework: 1.5.1)

## Unity Version Requirements
- Minimum Unity version: 2021.2.0f1 (as specified in package.json)
- Demo project uses Unity 6000.0.33f1 (from ProjectVersion.txt)

## Assembly Definitions
- **TimeMachine.Runtime**: Runtime assembly containing core functionality
- **TimeMachine.Editor**: Editor-only assembly for custom inspectors and tools

## Working with Timeline
TimeMachine extends Unity's Timeline system. When modifying timeline-related code:
1. Understand Unity's PlayableDirector and TimelineAsset APIs
2. TimeMachine clips inherit from PlayableAsset/PlayableBehaviour patterns
3. Custom tracks extend from TrackAsset base class

## Git Workflow
- Main branch: `main`
- Development branch: `develop`
- The repository uses pull requests for feature integration

## Package Installation
Users install the package via Unity Package Manager using:
```
https://github.com/murasaqi/Unity_TimeMachine.git?path=/jp.iridescent.timemachine#v1.0.1
```

## Important Notes
- This is a Unity package project with a demo Unity project included
- The package is designed for live entertainment and virtual production use cases
- OSC integration allows external control of timeline playback
- The project follows Unity's standard package structure and conventions