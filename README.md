# Tanks

A multiplayer top-down tank battle game built in Unity with [Mirror](https://mirror-networking.com/) networking. Two teams fight over team-deathmatch and base-capture objectives, with server-authoritative bots that can fill either team.

## Gameplay

- Host or join a match over LAN/direct IP.
- Two win conditions run per match: team deathmatch (eliminate the other team) and base capture (hold the enemy base's zone).
- AI bots drive, aim, fire and navigate on the server; each bot picks a role (Capturer, Defender, or a default all-rounder) and every team is guaranteed at least one Capturer and one Defender.
- Tanks have armor, aim/fire mechanics with multiple ammo types, disguise/spotting (bushes reduce visibility), and destructible track modules.

*(Gameplay video coming soon.)*

## Architecture

The codebase is split into layered assemblies so dependencies only point one way:

```
Tanks.Core   -> shared gameplay: vehicles, projectiles, player, spotting, combat events
Tanks.Match  -> match lifecycle and win conditions (depends on Core)
Tanks.Game   -> composition root: NetworkSessionManager (depends on Core, Match)
Tanks.AI     -> bot behaviors and steering (depends on Core, Match, Game)
Tanks.UI     -> all UI (depends on Core, Match, Game)
```

Each assembly enforces its own reference rules, so an accidental circular or wrong-direction dependency fails to compile rather than being a convention someone has to remember.

Pure game-logic (spotting math, disguise math, deathmatch tracking, AI steering) is extracted into small, engine-independent classes and covered by EditMode unit tests under `Assets/Tests/EditMode`.

Project layout:
```
Assets/
  _Project/   your own content: Scripts, Prefabs, Scenes, Data, Settings, Sounds, Sprites, Textures, Tests
  Mirror/     networking library (MIT, included)
  ThirdParty/ only the third-party art/audio files the game actually uses (see below)
```

## Requirements

- Unity 6000.3.11f1 (or compatible 6000.3.x)
- Git

## Getting started

1. Clone the repo.
2. Open the project in Unity Hub (Unity resolves the packages itself on first open).
3. Open `Assets/_Project/Scenes/SampleScene.unity` and press Play to host, or open the Test Runner (**Window → General → Test Runner → EditMode**) to run the unit tests.

Everything the game needs is in the repository; nothing has to be downloaded separately.

## Third-party assets

Only the third-party files the game actually uses are kept in `Assets/ThirdParty/`, so the project works right after cloning. They remain the property of their publishers and are used under the licenses below; they are not covered by this repository's MIT license. If a publisher objects to their content being included here, it will be removed.

| Asset | Publisher | Source | License |
|---|---|---|---|
| Tank 3D Model | Isle of Assets | [Unity Asset Store](https://assetstore.unity.com/packages/3d/vehicles/land/tank-3d-model-225955) | Standard Unity Asset Store EULA |
| Tank Leopard2 | Kucher | [Unity Asset Store](https://assetstore.unity.com/packages/3d/vehicles/land/tank-leopard2-264329) | Standard Unity Asset Store EULA |
| Gridbox Prototype Materials | Ciathyza | [Unity Asset Store](https://assetstore.unity.com/packages/2d/textures-materials/gridbox-prototype-materials-129127) | Standard Unity Asset Store EULA |
| Particle Pack | Unity Technologies | bundled Unity example content | Unity Companion License |
| Free Bushes | Yughues | [OpenGameArt](https://opengameart.org/content/bushes) ([Asset Store mirror](https://assetstore.unity.com/packages/3d/vegetation/plants/yughues-free-bushes-13168)) | CC0 |

The three Unity Asset Store entries are listed under the Standard Unity Asset Store EULA, which is the default unless a listing states otherwise — check the current listing before any commercial use, and for the full packages get them from the links above (only the files this game uses are included here).

**Mirror** is included (`Assets/Mirror/`) under the MIT License (`Assets/Mirror/LICENSE`).

## License

The code under `Assets/_Project/` is licensed under the MIT License — see [LICENSE](LICENSE). Third-party assets keep their own licenses as listed above.
