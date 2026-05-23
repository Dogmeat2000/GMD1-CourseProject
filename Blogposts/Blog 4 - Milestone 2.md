# Milestone 2
## Introduction
This milestone focused on the core gameplay loop - from level start to level win/lose.

<img src="Blog%204%20-%20Appetizer.jpg" alt="Overview" width="860">

## Enemy Spawning and Behavior
Enemies are now spawned dynamically via the ``WaveDirector`` script. It uses a "Threat Budget" calculation to "buy" enemies from a defined threat size for each wave. This allows for progressively more challenging waves, and adds a random element, since the ``WaveDirector`` can buy different amounts of enemy types on each playthrough.

<img src="Blog%204%20-%20Spawning%20and%20Flight.gif" alt="Enemies Spawn and Attack" width="860">

## Handling Settings
To adhere to the Single Responsibility Principle I implemented Unity's ``ScriptableObject`` architecture to create ``LevelSettings`` and ``GlobalSettings``.

This allows scripts to query the active ``LevelSettings`` asset for data, allowing configuring the game from the inspector.

```csharp
[CreateAssetMenu(fileName = "NewLevelSettings", menuName = "Game/Settings/Level Settings")]
public class LevelSettings : ScriptableObject {
    // ... Many other configurations and settings
    [field: SerializeField] public float MinSpawnDistance { get; private set; } = 500f;
    [field: SerializeField] public float MaxSpawnDistance { get; private set; } = 1000f;
    // ... Many other configurations and settings

    // ...
    public float GetDifficultyMultiplier(GameDifficulty currentDifficulty) {
        return currentDifficulty switch {
            GameDifficulty.Easy => EasyDifficultyMultiplier,
            GameDifficulty.Normal => NormalDifficultyMultiplier,
            GameDifficulty.Hard => HardDifficultyMultiplier,
            GameDifficulty.Nightmare => NightmareDifficultyMultiplier,
            _ => NormalDifficultyMultiplier
        };
    }
    // ...
}
```

Settings object in Unity Inspector:
<img src="Blog%204%20-%20Settings.jpg" alt="Settings overview in Unity" width="860">

```csharp
// Example of how Settings are queried from other scripts.
public class LevelManager : MonoBehaviour
    {
        [SerializeField] private LevelSettings activeSettings;
        
        ///...
        
        /// Other scripts within the level can then retrieve the settings through the LevelManagers public property.
        public LevelSettings Settings => activeSettings;
        
        ///...
    }
```

## Win/Lose Conditions
The `GameDirector` controls the level lifecycle via event-driven programming, ensuring loose coupling.

<img src="Blog%204%20-%20Victory%20and%20Defeat.jpg" alt="Victory and Defeat" width="860"> <br>

- **Victory:** Triggered when the ```WaveDirector``` broadcasts ```OnAllWavesCleared```
- **Defeat:** Triggered if ```FleetDirector``` broadcasts ```OnFleetDestroyed```, or all player' health reaches 0

```csharp
public class GameDirector : MonoBehaviour {
        ///...
        private void Start() {
        _alivePlayers = playerHealths.Count;
        
        if (_waveDirector) {
            _waveDirector.OnAllWavesCleared += HandleVictory; //Subscribe to Win condition (OnAllWavesCleared)
        }
        
        if (_fleetDirector) {
            _fleetDirector.OnFleetDestroyed += HandleDefeat; //Subscribe to Defeat condition 1 (OnFleetDestroyed)
        }

        foreach (var player in playerHealths) {
            if (player) player.OnZeroHealth += HandlePlayerDeath; //Subscribe to Defeat condition 2 (player.OnZeroHealth)
        }
    }
    ///...
}
```

## Menu & HUD
**Menus** were expanded to support the core gameplay loop:

<img src="Blog%204%20-%20Menus.jpg" alt="Victory and Defeat" width="860"> <br>

It uses Unity's ``uGUI system`` and ``EventSystem``. Navigation is handled through ``On Click()`` methods. To add visual effects and sounds, I implemented ``UIButtonVisuals.cs``, getting default effects from the game settings but allowing for overrides.

<img src="Blog%204%20-%20Menu%20Change%20in%20Unity.jpg" alt="Victory and Defeat" width="860"> <br>


**Heads Up Display (HUD)** uses the Model-View-Controller (MVC) pattern. This allows for separation of concerns and loose coupling via dependency inversion and the observer pattern (events) to present updated info.

<img src="Blog%204%20-%20HUD%20Details.jpg" alt="HUD Details" width="860"> <br>



## The Architecture
I put care into improving the foundation going into Milestone 3 using these design patterns:

**Service Locator (Structural):** Allows scripts to find managers via ``ServiceLocator.Get<T>()`` without relying on rigid dependencies or Singletons. ``LevelManager``, ``GameDirector``, ``WaveDirector``, ``FleetDirector``, ``BattlefieldRadar`` all register with this.

```csharp
// LevelBootstrapper.cs
ServiceLocator.Register(waveDirector);

// Usage in any other script
_waveDirector = ServiceLocator.Get<WaveDirector>();
```

**State Pattern (Behavioral):** ``GameStateService`` dictates the current rules of the game (``Deploying``, ``Playing``, ``Paused``, ``GameOver``). Instead of scattered boolean flags (_isPaused), a central authority defines the active state, centralizing logic like ``Time.timescale`` handling.

**Strategy Pattern (Behavioral):** Allows hot-swapping complex algorithms. A Kamikaze drone doesn't need to know how to pick a target; it simply asks the Radar to use a specific Strategy to hand it the best target mathematically available. Also used when determining flight behavior via ``IMovementBehavior``  (``SeekBehavior``, ``EvasiveBehavior``, ``SeparationBehavior``).

```csharp
public ITargetable SelectTarget(List<ITargetable> targets, Vector3 position) {
    // Concrete implementations like 'WeightedRandomStrategy' or 'ProximityThreatStrategy' live here.
}
```

**Observer Pattern (Behavioral):** UI scripts don't ask "Are you dead yet?". The ``HealthManager`` broadcasts ``OnHealthChanged`` only when health changes, improving performance. Also used in ``PlayerScoreHUD``, ``GameDirector``, ``WaveDirector`` and ``FleetDirector``.

**Object Pool (Creational):** Recycles Memory. Instead of Unity aggressively allocating and destroying memory, ``UniversalPoolService`` keeps a hidden pool of inactive objects ready for instant deployment (used for projectiles, VFX, and enemies).

**Singleton (Creational):** Used for persisting cross-scene data, such as ```LeaderboardManager.Instance``` saving high scores between the Main Menu and the level. Also used in the ``GlobalManager`` and ``UniversalPoolService``.
