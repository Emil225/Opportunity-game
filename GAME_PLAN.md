# Opportunity — game plan

Hackathon: NASA Space Apps Challenge 2026 (14–15 November). Submission closes Sunday 15 November, 11:59 PM local time.
Challenge: tell the story of hardware NASA left on the Moon, Mars or in deep space, for school-age space enthusiasts. Accuracy matters.

## Concept

A slow, quiet 3D game about the Opportunity rover. The player is a rover driver at mission control. They do not steer live: they queue a few commands, send them, wait, and watch the rover carry them out. The goal in each level is to photograph a real science target. Each photo reveals the real NASA image and a short explanation from a scientist.

- Audience: roughly ages 10–14. Lonely but warm in tone, never scary.
- Pacing reference: Iron Lung (slow movement, navigating by instruments, taking photos), without the horror.
- Look: low-detail 3D with a pixelated filter, butterscotch Martian sky.
- Framing: open in the present day with the rover silent and dusty in Perseverance Valley, then flash back to the landing.

## Project setup

- Unity 6.6 (6000.6.8f1), Universal Render Pipeline, Universal 3D template.
- Uses the new Input System package (`Assets/InputSystem_Actions.inputactions`). Do not use the old `Input` class.
- UI: uGUI with TextMeshPro.
- Target build: WebGL (playable from a link), so no threads, no file system access, keep textures small.
- Currently only the default `Assets/Scenes/SampleScene.unity` exists. No game scripts or models yet.
- Put code in `Assets/Scripts`, models in `Assets/Models`, NASA photos in `Assets/Photos`, levels in `Assets/Scenes`.
- Scripts are edited as files; a person does the editor work (attaching components, building scenes). For every script, list the editor steps needed: which object it goes on and which fields to fill in.

## Core loop

1. The console shows a map, a power meter, and a list of science targets.
2. The player queues commands with buttons: turn left, turn right, drive forward, aim camera, take photo.
3. The player presses Send. A "transmitting…" indicator runs for a few seconds (the real one-way delay to Mars is about 3 to 22 minutes).
4. The rover carries out the commands slowly, one at a time.
5. If a science target is in view when the photo is taken, the real NASA image appears line by line, followed by scientist dialogue.
6. When all targets in a level are photographed, the next level loads.

## Systems to build

### Must have
- **RoverController**: executes one command at a time. Turns on the spot, drives in a straight line at a slow fixed speed, spins the wheels while moving.
- **CommandQueue**: stores queued commands, shows them in the UI, sends them after the transmit delay, lets the player clear the queue before sending.
- **Camera mast**: pans and tilts on command; the photo is taken from this camera.
- **PhotoCapture**: checks that a `ScienceTarget` is inside the camera view, within range, and not blocked. Reports success or "nothing of interest".
- **ScienceTarget**: data per target: name, real NASA image, image ID and credit, caption, dialogue lines.
- **Photo result screen**: reveals the image line by line, then shows the caption and credit.
- **DialogueBox**: shows scientist lines two or three at a time, advance with a click. All dialogue is on-screen text.
- **Level flow**: start screen, level loading, ending screen, credits and sources screen.
- **Level 1: Eagle crater**, complete and playable from start to finish.

### Should have
- **Map**: top-down view with the rover position, heading, and target markers.
- **Power meter**: drops as dust builds on the solar panels; an occasional wind gust cleans them and restores power.
- **Levels 2–4** (see below).
- **Pixel filter**: render the main camera to a low-resolution texture and upscale with point filtering.
- **Sound**: wind, radio beeps, shutter click.

### Only if there is time
- Launch and airbag landing intro, as three or four still images with text.
- Microscope close-up as a second tool.

## Levels

Each level is one small area with one or two targets.

1. **Eagle crater, 2004.** The rover rolls off its lander platform. Target: the "blueberries", small hematite spherules in the outcrop. Hematite usually forms in water, so this was evidence of past water on Mars.
2. **The heat shield, 2005.** The rover visits its own discarded heat shield. Target: a meteorite lying beside it (Heat Shield Rock), the first meteorite identified on another planet.
3. **Purgatory Dune, 2005.** The rover is stuck in soft sand for about five weeks. Drive commands move it only a tiny distance; wheels spin almost in place. The player has to keep sending commands to get free.
4. **Perseverance Valley, 2018.** A planet-wide dust storm. The fog thickens, the light fades, the power meter falls, and commands stop getting replies. Ends back in the present day with the silent rover.

## Facts to use (check each against a NASA source before it goes in the game)

- Landed 25 January 2004 (UTC) in Eagle crater, Meridiani Planum.
- Planned mission: 90 Martian days (sols). Actual: more than 14 years, over 5,000 sols.
- Drove about 45 km in total, farther than any other vehicle on another world.
- Top speed about 5 cm per second.
- Twin rover: Spirit.
- Last contact: 10 June 2018. NASA declared the mission over on 13 February 2019.
- "My battery is low and it's getting dark" is a journalist's paraphrase of the final data, not a message the rover sent. Present it that way or leave it out.

## Assets

- **Rover**: NASA 3D Resources, "Spirit & Opportunity" (GLB) or the lower-detail "MER Opportunity". Unity needs a glTF importer package for GLB files, or convert to FBX in Blender.
- **Terrain**: one small Unity terrain per level, shallow crater, reddish sand texture.
- **Rocks and outcrop**: three or four simple meshes, reused and rescaled.
- **Blueberries**: tiny grey spheres scattered on the ground.
- **Lander platform**: flat base with open petals.
- **Heat shield wreck and meteorite**: crumpled cone and a dark pitted rock.
- **Photos**: JPL Photojournal, Mars Exploration Rover section. Record the image ID (PIA number) and credit line for every image.

## Animations

- Wheels spinning and turning on the spot (must).
- Camera mast pan and tilt (must).
- Photo downlink reveal, line by line (must).
- "Transmitting…" indicator (must).
- Dust storm: thickening fog, fading light, falling power (should).
- Wheels slipping in Purgatory Dune (should).

## Build order

1. Rover drives from queued commands on a flat plane.
2. Photo capture, result screen and dialogue working with one target.
3. Eagle crater level complete.
4. Map, power meter, pixel filter.
5. Levels 2–4 by copying level 1.
6. Ending, sound, credits and sources.
7. WebGL build, tested in a browser.

Stop adding features three hours before the deadline. The remaining time is for the demo video, slides and submission.

## Rules to keep

- Every fact and image in the game needs a source listed on the credits screen.
- Third-party assets must be free to use or open source.
- No in-app purchases or analytics packages.
