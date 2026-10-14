# Cobbox - Golet Games

A hypercasual mobile endless runner, inspired by **ZigZag** (Ketchapp).

## Concept
A ball moves forward on its own along a narrow, randomly generated zigzag path. Each tap switches its direction between left-diagonal and right-diagonal. If the ball runs off the path, the game is over. Tiles fall away behind the player, gems appear along the path, and the speed slowly increases.

- **Controls:** one tap, anywhere on the screen
- **Goal:** survive as long as possible and beat your best score
- **Session length:** about 30 seconds to a few minutes

## Tech
| | |
|---|---|
| Engine | Unity 6 LTS (latest `6000.x` LTS) |
| Render pipeline | URP, "Universal 3D" template |
| View | 3D with an orthographic, isometric-angle camera |
| Art style | Simple shapes (cubes and a sphere), flat colours, low-poly |
| Platforms | Android (primary), iOS (later) |
| Orientation | Portrait |
| Language | C# |
| Version control | Git + Git LFS for binary assets |

## Core features (MVP)
- [ ] Ball moves on its own; a tap switches its direction
- [ ] Path generated ahead of the ball and reused tile objects (pooling)
- [ ] Tiles fall after the ball passes
- [ ] Game over when the ball leaves the path
- [ ] Score (+1 per tile) and saved best score
- [ ] Collectible gems
- [ ] Start, game-over and retry screens
- [ ] Speed that increases over time
- [ ] Camera that follows the ball smoothly

## Roadmap
1. **Setup:** Unity project, folder structure, runs on a real phone
2. **Core prototype:** movement, path generation, falling tiles, game over (no art)
3. **Game loop:** score, best score, gems, game UI
4. **Feel and difficulty:** speed curve, camera, animations, sound, phone vibration
5. **Art and polish:** colour themes, particle effects, menus
6. **Mobile setup:** support for different screen sizes, steady 60 FPS, small download size
7. **Ads and tracking:** rewarded "continue" ad, full-screen ad every few runs, analytics
8. **Release:** Google Play internal testing, then a soft launch

## Out of scope (for now)
- Multiplayer and leaderboards
- In-app purchases (beyond an optional "remove ads")
- Skins shop and level progression
- Custom 3D models or characters

## Project structure
```
Assets/
  Scenes/      # single Game scene
  Scripts/     # C# gameplay code
  Prefabs/     # tile, gem, player
  Materials/
  Audio/
  UI/
```

## Getting started
1. Install the latest Unity 6 LTS from Unity Hub, with Android Build Support (OpenJDK, Android SDK & NDK).
2. Clone the repo and run `git lfs install`.
3. Open the repo folder in Unity Hub (**Add → Add project from disk**).
