# Game Design Document

## Project Overview
- **Game Title          :** Spearhead: Nereus Protocol
- **Genre               :** Singleplayer/Co-op Arcade Action Sci-Fi Shooter
- **Platform            :** Windows-based VIA Arcade Machine (Custom Inputs) / WebGL (Browser)
- **Target Audience     :** Players seeking challenging, fast-paced, narrative-driven co-op arcade sessions (5–10 minute runs)

![Nereus-9](Concept%20Art/Environment/space_view_of_nereus_9_on_approach_v01.jpg)


## Game Concept
Spearhead: Nereus Protocol is a fast, tense 3D arcade shooter where 1–2 players operate naval turrets on a warship. Protecting a fleet from a first-person perspective, players fight off airborne alien techno-organic horrors. The experience balances aggressive target prioritization, energy capacitor management, and cooperative fleet repair. It is designed to feel epic, cinematic, and relentlessly tense while remaining immediately accessible.

![Game Concept](Concept%20Art/Mood_WideShots/mood_naval_turret_destroying_attacking_aliens_v01.jpg)


## Target Aesthetics (MDA)
- **Fellowship           :** Players actively monitor and protect each other and the AI-controlled fleet.
- **Challenge            :** Sustained survival against progressively overwhelming waves of diverse enemy archetypes, driving competitive high-score chasing.
- **Submission           :** Players are part of the fleet. Their survival depends on the fleet. They do not control the ships, only the weapons. If the ship dies, so does the player.
- **Sensation            :** Visual effects support the experience of being isolated in alien seas, and under attack by alien beings.

![Alien Drone](Concept%20Art/Entities/alien_biofuzed_kamikaze_drone_v01.jpg)

## World Foundation
- **Planet              :** Nereus-9, bioluminescent seas, thin horizon mist, exotic weather.
- **World State         :** Humanity arrived peacefully to extract resources; drilling awakened an ancient race of aliens that became hellbent on wiping out humanity.
- **Threat              :** Semi-biological, semi-mechanical alien entities in various sizes.

![Aether Crystal](Concept%20Art/Environment/land_aether_crystal_closeup_v01.jpg)

## Player Experience & Game POV
- **POV                 :** First person turret view placed on the upper part of the turret, showing the guns as well as the area around.
- **Screen              :** Full screen (singleplayer). Horizontal split screen (coop)
- **Player Feelings     :** Teamwork, tension (managing ammo, power, health), competing for high-score
- **Cooperative options :** Either dual turret ships (require 2 players), or single turret ship(s). In single mode each player will be placed on turrets on seperate ships

![Player Ship](Concept%20Art/Ships/technical_concept_ship_escort_frigate_v02.jpg)

## Art and Audio
- **Visual Style        :** Grimdark Military Sci-Fi meets Alien
- **Color Palette       :** Deep ocean blues and stormy grays, violently contrasted by neon cyan/yellow/red weapon fire and bright bioluminescent enemy markers.
- **Audio               :** Authoritative UI soundscapes, heavy kinetic explosions, high-pitch energy weapon discharges, layered over a persistent, atmospheric alien ocean ambience.


## Initial Scope
![Milestones](Milestones.png)

## Interactivity:
- **Aiming                  :** Turret pitch and yaw rotation via arcade sticks/mouse
- **Primary Fire            :** High-velocity airburst projectiles for long-range and wide impact
- **Secondary Fire          :** Auxiliary focused-fire cannons for single, large targets
- **Emergency Assist        :** Limited-ammo payloads used to heal allied ships
- **Capacitor & HUD         :** Weapons drain a central power meter. The HUD tracks capacitor charge, hull integrity, ally HP, and player score

## Core Loop
- **The Short Loop (seconds):** Aim -> Manage Capacitor -> Choose fire mode -> Fire -> Score
- **The Mid Loop (minutes)  :** Survive waves -> Repair ships -> Persist highscore.
 
## Technical Requirements
- **Engine                  :** Unity 6.3 LTS.
- **Target Hardware         :** Intel Core i5, Nvidia GTX 980 TI.
