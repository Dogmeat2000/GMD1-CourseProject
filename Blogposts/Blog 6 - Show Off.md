# Blog 6: Conclusion & Feature Show-Off

## Introduction:
**Spearhead - Nereus Protocol** has reached is final, deployed state. It is ready to be played on VIA's Arcade machine, offering visitors fast-paced, intense solo or split-screen coop battles. Players operate turrets against overwhelming odds, fighting alongside their fleet in a desperate bid for survival.

<img src="Blog%206%20-%20GameLogo.png" alt="Spearhead: Nereus Protocol Tactical Logo" width="500">

## Show Off:
Whether playing via WebGL or the Arcade cabinet, players will experience a polished, cohesive arcade shooter. 

Key features include:

* **Responsive Menus:** Menus tie together multiple scenes.
    * **Main Menu:**<br>
    <img src="Blog%206%20-%20Main%20Menu.jpg" alt="Main Menu" width="500">

    * **Pause Menu:**<br>
    <img src="Blog%206%20-%20Pause%20Menu.jpg" alt="Pause Menu" width="500">

    * **Ready Menu:**<br>
    <img src="Blog%206%20-%20Ready%20Menu.jpg" alt="Pause Menu" width="500">

* **Configurable Settings:** Adjust the experience to your preference<br>
<img src="Blog%206%20-%20Settings%20Menu.jpg" alt="Main Menu" width="500">
    
* **Multiple Game Modes:** Play with a friend, or play alone.
    * **Singleplayer**:<br>
    <img src="Blog%206%20-%20SP.jpg" alt="Single Player Mode" width="500">

    * **Split-Screen 2 Player COOP, on the same ship**: Players live or die on the same ship. Emphasizes working together.<br> 
    <img src="Blog%206%20-%20Coop%20Feature.jpg" alt="Multiplayer Mode 1" width="500">

    * **2 Player COOP, on different ships**: Players live or die independently. Emphasizes competing against each other.<br>
    <img src="Blog%206%20-%20Coop%20Multiple%20Ships.jpg" alt="Multiplayer Mode 2" width="500">

* **Shooting Mechanics:** 3 Fire modes. Use the main weapon for long engagements with airburst explosion radius. Use the  auxiliary cannon for focused fire on single entities. Use the special weapon to perform repairs on allied ships.<br>
<img src="Blog%206%20-%20Shooting%20Mechanics.gif" alt="Shooting Mechanics gif" width="500">

* **Health Management:** Keeping an eye on your own health, and your allies, is crucial to winning the game. As allies die, your fire support decreases. Lose all allies and you lose the game.<br>
<img src="Blog%206%20-%20Health.gif" alt="Health gif" width="500">

* **Multiple Enemy Types:** 

    * **Kamikaze Drone:** Explodes on collision, or on death.<br>
    <img src="Blog%206%20-%20Kamikaze%20Drone.jpg" alt="Explosive Drone" width="500">

    * **Bio Swarmer:** Uses ranged attack pattern<br>
    <img src="Blog%206%20-%20%20Ranged%20Drone.jpg" alt="Ranged Enemy" width="500"> 

* **Allied NPC Turrets:** Dealing less damage than players, they build an atmosphere of a large battle.<br>
<img src="Blog%206%20-%20NPC%20Turrets.gif" alt="NPC Turret" width="500"> 

* **Leaderboards:** Players score points and compete against each other. Highscores are persisted between game launches.
    * **Game Over Menu:** Add name on win or defeat<br>
    <img src="Blog%206%20-%20Defeat%20Highscore.jpg" alt="Defeat Highscore" width="500"> 
    
    * **High Score Menu:** View in global highscore<br>
    <img src="Blog%206%20-%20Main%20Menu%20Highscore.jpg" alt="Global Highscore" width="500"> 

* **Immersive SFX and VFX:** Ships smoke as they take damage, ultimately burning and exploding. Enemies use animations as they move, show various VFX as they attack (explode/die/shoot). Turrets move and rotate, shots produce flashes and various impact VFX. The ocean swells as waves move across the scene. Can you spot all the effects?

* **Multiple platforms:** Deployed on WebGL and Windows platforms, with optimizations defined for each



## Size
* Arcade deployment fulfills the max. 500mb requirement <br>
<img src="Blog%206%20-%20Disk%20Usage.jpg" alt="Game Size" width="300"> 

* GitHub pages deployment requirement is limited by githubs max 100mb file size. Reducing textures and audio compression, allowed the compressed install to reach ~90mb
<img src="Blog%206%20-%20Compressed%20Size.jpg" alt="Game Size Webgl" width="1280"> 

## Conclusion
Spearhead - Nereus Protocol shows the process of setting the vision for a game, executing this vision and deploying it, utilizing modern game development tools and practices. Focusing on 3D gameplay, this game shows how an Entity Component System (ish) based game engine can be utilized to achieve immersive 3d gameplay, using C# scripting, game asset handling (meshes, audio, textures, animations, shaders, lighting, etc.) through extensive prefab usage and programmatic event-based gamplay loop.

As a developer, I have grown from a rookie - having only heard of Unity and dreamt of one day being able to implement a game of my own - to now having proof of my abilities! A fully functional 3D multi-platform game!

<br>

<b>Thank you for playing.</b><br>
<a href=https://dogmeat2000.github.io/GMD1-CourseProject/>
<img src="Blog%206%20-%20GameIcon.png" alt="Spearhead Nereus Protocol Icon" width="200"><br>
<b>Click to play</b></a>