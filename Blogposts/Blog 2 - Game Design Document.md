# Game Design Document

## Project Overview
- **Game Title          :** Spearhead: Nereus Protocol
- **Genre               :** Singleplayer/Co-op Arcade Action Sci-Fi Shooter
- **Platform            :** Windows-based arcade machine (local single / local 2-player split-screen)
- **Target Audience     :** Players seeking challenging, fast-paced, narrative-driven co-op arcade sessions (5–10 minute runs)

## Game Concept
Spearhead is a fast, tense 3D arcade escort shooter where 1–2 players each operate a single 360° upper-bridge turret mounted above their ship’s armored bridge. The player(s) track a moving convoy from a first-person turret camera, fight airborne alien techno-organic horrors, and coordinate railgun target designation and other shared systems. The experience should feel epic, cinematic, and hopelessly tense while remaining immediately accessible and arcade-friendly.

<img width="1568" height="968" alt="SHIP_Terran_Frigate_Spearhead_v01_1" src="https://github.com/user-attachments/assets/e18f0279-386a-4a90-9a33-a54cfb46cdf7" />


## Target Aesthetics
- **Fellowship           :** Players work together to protect the fleet. Special actions can only be performed after cooperating on activating them.
- **Challenge            :** Survival against increasing wave difficulties of attacking foes, combined with high-scoring between players so players  might challenge each other in friendly competition.
- **Submission           :** Players are part of the fleet. Their survival depends on that of the fleet. They do not control the ships, they only control the weapons systems against overwhelming enemies.
- **Sensation            :** World-shaking railgun discharges, ocean spray, thunderous impact VFX + low-frequency audio that sells scale.

## World Foundation
**Planet                :** Nereus-9 (oceanic super-world, 92% surface water), bioluminescent seas, thin horizon mist, exotic weather (two suns by day, multiple moons by night).
- **Key Resource        :** Aether Crystal — an exotic crystal essential for FTL cores, energy weapons and advanced power production on Terra.
- **World State         :** Humanity arrived peacefully to extract resources; drilling operations awakened an ancient unknown race of aliens that became hellbent on wiping out humanity.
- **Threat              :** Semi-biological, semi-mechanical alien entities in various sizes posing different kinds of threats to humanitys operation on the planet.

## Player Experience & Game POV
First person turret view in either split screen (2 players) or single screen (1 player).
- **Primary POV         :** POV camera placed on the upper part of the turret, showing the guns as well as providing a wide-angle view of the area around the turret / ships.
- **2P Split-Screen     :** Vertical split screen, with each player having half of the screen available for their own POV turrets and HUD.
- **Player Feelings     :** Teamwork, tension (managing ammo, heat, hull health, etc.), competing for high-scores over short (4-8 minutes sessions).
- **Cooperative options :** Multiple ship variants to choose. Either dual turret ships (require 2 players), or single turret ship(s). In 2P mode with single turret ships, each player will be placed on turrets on seperate escort ships.

## Gameplay Mechanics
- **Core Mechanic — Railgun Designation:** Players maintain laser lock on capital targets while surviving enemy fire; successful locks trigger massive spinal railgun strikes on designated target.
- **Enemy Encounters    :** Wave-based enemy attacks (Details to be decided, but perhaps Harassers → Bombers → Snipers → Capital Beasts → Leviathans).
- **Modes               :** 1 player or 2 player escort Mission (High-score).
- **Cooperation Focus   :** Both players must coordinate to maintain targeting windows, communicate with each other to divide targets between each other and protect convoy assets from attacks from multiple flanks.

## Characters & Narrative
- **Protagonist         :** Player(s) are the forward defence of a deep-sea mining fleet on a hostile alien ocean world.
- **Tone                :** Earnest military propaganda like "Democracy Floats," "Extraction Is Freedom," "Peace Through Superior Firepower"
- **Enemy Philosophy    :** An alien hive-like hiearchical society structure means Leviathans are the largest most fierce alien entities, and these control the rest. On the outer perimeter the least dangerous aliens reside. These act as observers and alarms. When they are disturbed, they immediately begin attacking and alert the next tier of enemies. This continues until the Leviathan appears.

## Art and Audio
- **Visual Style        :** TBD
- **Color Palette       :** Deep ocean blues, neon bioluminescence, industrial grays, white railgun flashes
- **Audio               :** Authoritative propaganda narration, world-shaking railgun impacts

## Initial Scope
- 1 Escort Mission
- 1 and 2 player split-screen support.
- High-score / Leaderboard system
- Single and dual turret ships.
- 2 distinctly different enemy types.
- 1 Transport ship to be protected. Also carries a massive long-charge railgun that players must coorperate on activating.
- Short narrative introduction with voiceover upon game load.
- 2 different turret options for players, with 2 different ammunition types.
- Environment including: ocean/water, fog/mist/clouds and suns/moons.
- Menu and pause screen.

## Interactivity:
- **Aiming turrets          :** Rotates turret around own axis (Joystick/Mouse left/right) and controls turret azimuth (Joystick/Mouse up/down).
- **Primary Fire            :** Fires the turrets main weapon (Mousebutton 1? Or the RED arcade button?)
- **Secondary Fire          :** Fires the turrets secondary weapon (Mousebutton 2? Or the TDB arcade button?)
- **Coop Target Designation :** Hold to laser lock a target for the Main Railgun to fire upon. Locking speed increases significantly if both players cooperate on locking the target.
- **Emergency Assist        :** If one player is destroyed, the other player can "revive" the player by firing a special nano-repair bot projectile at the other players ship.
- **Overheat                :** Primary turret weapons can overheat. Overheating takes time to dissipate.
- **HUD                     :** The player HUD shows the following: Minimap based on current ship (shows incoming enemies), turret heat meter, railgun charge indicator, hull integrity meter, convoy HP meter, player score.

## Core Loop
- **The Short Loop (seconds):** Aim - Fire - Manage Turret Heat - Avoid / Intercept incoming threats - Score points.
- **The Mid Loop (minutes)  :** Survive waves - Protect convoy ships - use railgun to kill wave boss/miniboss - wave cleared
- **The Long Loop (session) :** Complete mission(s) - Leaderboard - Repeat
 
## Technical Requirements
- **Engine                  :** Unity 6.3 LTS.
- **Target Hardware         :** Intel Core i5, Nvidia GTX 980 TI.
- **Resolution              :** Full HD (1920 * 1080)
