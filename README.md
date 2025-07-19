# <img src="https://raw.githubusercontent.com/IcEarthlight/aurela-game/refs/heads/master/aurela_logo.svg" height="120"/>

Aurela is a dimentional rhythm game built with Unity for PC platforms. Notes descend through 3D space synchronized to music, and players must time their inputs precisely to hit them. Combining dynamic visuals, audio feedback, and stat tracking, it's designed to challenge reflexes and rhythm perception alike.

## Features

- **3D Note Drop System** immersive vertical descent visuals for each beat.
- **Judgment Categories** Perfect / Great / Good / Bad / Miss, with Early / Late timing indicators.
- **Hold Note Support** long-press detection and failure feedback.
- **Chart Parsing** partly supports .osu for mania mode
- **Cross-platform Build Support** compatible with Windows & Mac & Linux

## Development Environment

| Item | Detail |
|------|--------|
| Engine | Unity 2022.3.62f1 LTS |
| Language | C# |
| Editor | VS Code |
| Platforms | Arch Linux x86_64 6.15.6 |
| Chart Formats | currently only `.osu` |

## Quick Start

```bash
# Clone the repository
git clone https://github.com/IcEarthlight/aurela-game.git

# Open the folder in Unity and press Play
```

## Controls

| Key | Action |
|-----|--------|
| <kbd>w</kbd><kbd>e</kbd><kbd>f</kbd><kbd>␣</kbd><kbd>k</kbd><kbd>o</kbd><kbd>p</kbd> | Default mapping for keys |
| mouse scrolling / <kbd>↑</kbd><kbd>↓</kbd> | Select songs / charts |
| <kbd>Esc</kbd> | Quit / Pause |

## Disclaimer

The `Charts/` directory in this project contains beatmaps sourced from the game [osu!](https://osu.ppy.sh), including associated audio files and background artwork. These assets are **not owned or licensed by this project**, and are included **solely for testing and development purposes**. They will not be distributed or included in any public or official release. All rights remain with their respective creators.

Absolutely! Here's a friendly and welcoming closing section you can add to your `README.md`:

## To-Do List

This project is under active development. Here's what's planned and what's already been implemented:

- [x] Save and display highest score records
- [ ] Settings menu (volume, note speed, latency calibration, key remapping)
- [ ] Song selection UI (track info & play history display)
- [ ] Rank system and progression design
- [ ] Scene transition loading animations
- [ ] Speed variant support for charts
- [ ] Special hit sounds (custom per note/chart)
- [ ] Mobile adaptation (touch input and UI scaling)
- [ ] Built-in hart editor

## Get Involved

Feel free to playtest the game, explore its mechanics, and experiment with custom charts! Your feedback is invaluable - whether it's a bug report, feature suggestion, or gameplay tweak. You're warmly invited to submit issues or open pull requests to help improve the project.
