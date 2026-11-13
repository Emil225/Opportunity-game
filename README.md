# NASA history mini-games

A collection of small, quiet 3D mini-games about real NASA hardware and the people who ran it. Made for the NASA Space Apps Challenge 2026 (submission closes **Sunday 15 November, 11:59 PM local time**).

Each mini-game tells one true story. The first one is **Opportunity**: you drive the Mars rover by queueing commands and photographing real science targets. Its design is in [GAME_PLAN.md](GAME_PLAN.md). The next one being worked on is a story about the **Hubble Space Telescope**: it launched with a flawed mirror, and astronauts later fixed it.

Audience: roughly ages 10 to 14. Accuracy matters: every fact and image needs a NASA source on the credits screen.

## Getting started

1. Ask Emil to add you as a collaborator on GitHub, then clone the repository:
   ```bash
   git clone https://github.com/Emil225/Opportunity-game.git
   ```
2. Install **Unity 6000.6.8f1** through Unity Hub, with **WebGL Build Support**. Use exactly this version: a different one rewrites lots of project files and causes conflicts.
3. In Unity Hub, choose **Add > Add project from disk** and pick the cloned folder. The first open takes a few minutes while Unity builds its `Library` folder.
4. Open `Assets/Scenes/StartScreen.unity` and press Play.

### What the project uses

- Universal Render Pipeline (URP).
- The **new Input System** (`Assets/InputSystem_Actions.inputactions`). Do not use the old `Input` class.
- UI is uGUI with TextMeshPro.
- glTFast for importing `.glb` models.
- The final build is **WebGL**, played from a link. That means no threads, no file system access, and small textures.

## Who works where

The main rule: **each person works in their own folder and their own scenes.** Unity scene and prefab files do not merge well, so two people editing the same one almost always means someone loses work.

| Area | Owner | Files |
|---|---|---|
| Opportunity mini-game | Emil | `Assets/Scripts/`, `Assets/Scenes/Level1_EagleCrater.unity`, `Assets/Scenes/RoverTest.unity`, `Assets/Models/`, `Assets/Materials/`, `Assets/Photos/`, `Assets/Terrain/` |
| Hubble mini-game | _(your name)_ | `Assets/MiniGames/Hubble/` (everything for it goes in here) |
| Shared | Ask first | See the list below |

### Your folder

Make a folder for your mini-game and keep **everything** for it inside:

```
Assets/MiniGames/Hubble/
  Scenes/      your scenes, including a test scene of your own
  Scripts/
  Models/
  Materials/
  Photos/      NASA images (write down the image ID and credit for each)
  Audio/
  Data/        your own Credits asset
```

Future mini-games follow the same pattern: `Assets/MiniGames/<GameName>/`.

### Shared files: ask before changing

These are used by everyone. Before changing one, say so in the group chat, keep the change in a commit of its own, and get it merged quickly so the other person can pull it.

- `Assets/Scenes/StartScreen.unity` and `Assets/Scenes/Ending.unity`
- `ProjectSettings/`, especially `EditorBuildSettings.asset` (the scene list)
- `Packages/manifest.json` and `Packages/packages-lock.json` (adding a package changes these)
- `Assets/InputSystem_Actions.inputactions`
- `Assets/Settings/` (render pipeline settings)
- `Assets/Data/Credits.asset`
- `README.md`

### Reusing Opportunity's scripts

You are welcome to **use** the existing scripts in your own scenes. These are the most useful:

- `DialogueBox`: on-screen dialogue, click to continue.
- `PhotoResultScreen`: reveals a NASA image line by line with a caption and credit.
- `CreditsScreen` and `CreditsData`: a scrolling credits and sources panel.
- `PixelFilter`: the low-resolution pixel look.

Please **don't edit** them. If one needs to work differently, either ask Emil, or copy it into your folder under a new name and change the copy.

## Writing code

- **Put your scripts in a namespace.** Opportunity's scripts have no namespace, so a class of yours called `LevelManager` would clash with Emil's and break the build for both of you:
  ```csharp
  namespace Hubble
  {
      public class MirrorController : MonoBehaviour { ... }
  }
  ```
- Use the new Input System (`Keyboard.current`, `Pointer.current`, or input actions), never `Input.GetKey`.
- Start each script with a comment listing its **editor steps**: which object it goes on and which fields to fill in. The existing scripts show the format.
- Remember WebGL: no `System.Threading`, no `System.IO` file access.

## Git workflow

1. **Work on a branch, never directly on `main`.** Name it after your mini-game, for example `hubble/mirror-scene`.
   ```bash
   git switch main
   git pull
   git switch -c hubble/mirror-scene
   ```
2. **Pull `main` into your branch often**, at least every few hours during the hackathon:
   ```bash
   git pull origin main
   ```
3. Commit small and often. **Always commit the `.meta` file** that sits next to each asset: Unity uses it to keep references working.
4. **Move and rename files inside Unity**, not in Finder or Explorer, so the `.meta` file moves with them.
5. When something works, push your branch and open a pull request into `main`. The other person gives it a quick look and merges it.
6. If git reports a conflict in a `.unity` or `.prefab` file, don't try to merge it by hand. Keep one version (`git checkout --ours` or `--theirs`), then redo the smaller change in Unity.

## Adding your mini-game to the game

For now the start screen goes straight into Opportunity. The plan is to turn it into a menu that lists every mini-game.

Until then, test your mini-game from your own scene. When it's playable, tell Emil the name of its first scene. Emil will add it to the menu and the build scene list, so those shared files only ever have one person editing them.

## Facts, images and credits

- Every fact in dialogue or captions needs a NASA source (science.nasa.gov, jpl.nasa.gov, nasa.gov, hubblesite.org and similar). Check it before it goes in the game.
- For every NASA image, record its image ID and credit line exactly as NASA gives it.
- Make your own credits asset: in your `Data` folder, right-click and choose **Create > Opportunity > Credits**. Add your sources there, not to the shared `Assets/Data/Credits.asset`. They'll be combined on the final credits screen.
- Third-party assets must be free to use or open source. No in-app purchases or analytics packages.

## Deadline

Stop adding features **three hours before the deadline**. That time is for the WebGL build, testing it in a browser, the demo video, slides and the submission.
