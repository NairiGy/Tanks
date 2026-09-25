# Tanks

A multiplayer top-down tank battle game built in Unity with [Mirror](https://mirror-networking.com/) networking. Two teams fight over team-deathmatch and base-capture objectives, with server-authoritative bots that can fill either team.

## Gameplay

- Host or join a match.
- Two win conditions run per match: team deathmatch (eliminate the other team) and base capture (hold the enemy base's zone).
- Bots drive, aim, fire and navigate on the server. Each picks a role (Capturer, Defender or all-rounder), and every team gets at least one Capturer and one Defender.
- Tanks have armor, several ammo types, disguise/spotting (bushes reduce visibility) and destructible tracks.

## Architecture

Layered assemblies, so dependencies only point one way:

```
Tanks.Core   shared gameplay: vehicles, projectiles, player, spotting, combat events
Tanks.Match  match lifecycle and win conditions        -> Core
Tanks.Game   composition root (NetworkSessionManager)  -> Core, Match
Tanks.AI     bot behaviors and steering                -> Core, Match, Game
Tanks.UI     all UI                                    -> Core, Match, Game
```

Each assembly declares its own references, so a wrong-direction or circular dependency fails to compile.

Pure game logic (spotting and disguise math, deathmatch tracking, AI steering) lives in small engine-independent classes covered by EditMode tests in `Assets/_Project/Tests/EditMode`.

```
Assets/
  _Project/    scripts, prefabs, scene, data, settings, audio, sprites, textures, tests
  Mirror/      networking library (MIT)
  ThirdParty/  the third-party art the game uses (see below)
```

## Getting started

Requires Unity 6000.3.11f1 (or a compatible 6000.3.x).

1. Clone the repo and open it in Unity Hub.
2. Open `Assets/_Project/Scenes/SampleScene.unity` and press Play to host.
3. To run the unit tests: **Window → General → Test Runner → EditMode**.

## Third-party assets

`Assets/ThirdParty/` contains only the files this game uses. They belong to their publishers, are used under the licenses below, and are not covered by this repository's MIT license. If a publisher objects to their content being included here, it will be removed.

| Asset | Publisher | Source | License |
|---|---|---|---|
| Tank 3D Model | Isle of Assets | [Unity Asset Store](https://assetstore.unity.com/packages/3d/vehicles/land/tank-3d-model-225955) | Standard Unity Asset Store EULA |
| Tank Leopard2 | Kucher | [Unity Asset Store](https://assetstore.unity.com/packages/3d/vehicles/land/tank-leopard2-264329) | Standard Unity Asset Store EULA |
| Gridbox Prototype Materials | Ciathyza | [Unity Asset Store](https://assetstore.unity.com/packages/2d/textures-materials/gridbox-prototype-materials-129127) | Standard Unity Asset Store EULA |
| Unity Particle Pack | Unity Technologies | Unity Asset Store | Standard Unity Asset Store EULA |
| Free Bushes | Yughues | [OpenGameArt](https://opengameart.org/content/bushes) | CC0 |

Asset Store items are under the standard EULA unless their listing says otherwise. Mirror (`Assets/Mirror/`) is MIT licensed; see `Assets/Mirror/LICENSE`.

## License

Code in `Assets/_Project/` is MIT licensed, see [LICENSE](LICENSE).
