# Roll-a-ball: A Brief Technical Description of My First Game Implementation in Unity
https://github.com/user-attachments/assets/350a1edf-8d60-45e0-b97b-651d4cfb2386

## Introduction
My first experience with Unity Game development began with the [3D Game Roll-a-Ball tutorial](https://learn.unity.com/project/roll-a-ball). 
The tutorial walks through the creation of a complete 3D game project, covering scene setup, physics-based movement, AI navigation, UI systems and build/deployment.

## Unity Setup and Core Mechanics
I started by installing Unity Hub and Unity 6.3. Then I created a new Universal 3D project. This opened a blank project with a SampleScene. This gave the first introduction to the Component-based architecture used to define every GameObject - which determine each object's behavior.

First steps involved creating primitive GameObjects, applying materials to these, and manipulating their Transform properties (Position, Rotation, Scale). This gave me an understanding of how game scenes are visually constructed in the editor.

To enable player movement, I added a Rigidbody component, which integrates with Unity's physics engine, allowing forces and collisions to be simulated automatically. Player control was implemented with C# scripting, where Object behavior can be customized - such as Start (game start), Update (pr. frame), Move, etc. I used JetBrains Rider as my IDE for scripting - which Unity integrates nicely with.

Camera controls/movement was implemented by referencing the player, and showed how the Camera movement can either be defined by script or by the inspector in the editor directly.

## Prefabs, Interaction and UI
The Prefab concept is a major architectural concept in Unity, that allows for converting GameObjects into Prefabs, and thus creating a reusable template for objects. Any changes to the Prefab are automatically applied to all instances of this - which significantly improves maintainability and scalability.

Interactions between objects are handled through collision detection and triggers. Additionally, tags are used to categorize objects (e.g. pickupItems and enemies), which enable conditional logic inside scripts. UI elements such as score counter and win/lose messages (and other object references between objects) must be connected in the editor to avoid NullReferenceException errors. This approach moves the handling of relations away from the script implementations themselves and instead lets the Unity Editor handle these.

## AI Navigation and Deployment
AI movement requires "baking" a NavMesh. This defines the traversable areas of the environment, taking developer configured constraints into consideration. Such as slope and height constraints. This only applies to AI movement, and does not limit the players own ability to move. Finally I built the project from the editor and deployed this in a standalone .exe format, which allowed me to run the game directly from Windows. 

## Lessons Learned
I enjoyed following the tutorial, and was surprised with the relative ease of creating something playable in Unity.

I now know the basics of Unity’s component-driven design, physics system, prefab workflow, and event-based scripting model. The most important takeaway is how strongly Unity separates scene configuration (editor) from behavior (scripts). Even a small project benefits significantly from structured folders, reusable Prefabs, and clean script organization.
