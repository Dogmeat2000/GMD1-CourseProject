# Milestone 3
## Introduction
This milestone focused on expanding the core gameplay loop with multiplayer support, refining visuals (skybox, lighting, fog, audio), and expanding gameplay elements. While much progress was made visually, I will dive deeper into two specific areas: ``Multiplayer support`` and ``Shaders``.

<img src="Blog%205%20-%20Appetizer.jpg" alt="Overview" width="860">

## Multiplayer
A core vision for this game was cooperative split-screen play, where players work together while competing for kills. Implementing this was surprisingly easy due to my early focus on SOLID principles. Because the code is segmented and loosely coupled, introducing multiplayer meant simply creating another player object and modifying each camera to project to half the screen.

<img src="Blog%205%20-%20Multiplayer.gif" alt="Multiplayer" width="860">

Since each Player prefab already had its own canvas tied to a ``Screen Space - Camera``, my only remaining task was configuring the ``Viewport Rect`` for each camera (e.g., rendering 50% up the vertical axis, taking up 50% of the screen).

<img src="Blog%205%20-%20Multiplayer%20Camera%20Config.jpg" alt="Multiplayer Camera Config" width="860">

<br>

I implemented 3 game modes (``1P``, ``2P COOP same ship``, and ``2P COOP separate ships``), resulting in 5 player objects in the scene. 

```csharp
namespace _01_Scripts.Core.Settings {
    public enum GameMode {
        SinglePlayer,
        CoopTwoShips,
        CoopOneShip
    }
}
 ```

To manage these, I built a ``FleetDeploymentManager`` to configure ships upon scene start. It uses a scalable hardpoint system, activating the player objects corresponding to the chosen mode and disabling the rest to prevent unwanted AI turret spawns (which is a future goal).

 ```csharp
namespace _01_Scripts.Core.Managers {
    public class FleetDeploymentManager : MonoBehaviour, IService {
        // Serialized fields...

        private void Start() {
            DeployFleet();
        }

        private void DeployFleet() {
            GameMode mode = GlobalManager.Instance ? GlobalManager.Instance.GlobalSettings.ActiveGameMode : GameMode.SinglePlayer;
            
            GameDirector gameDirector = ServiceLocator.Get<GameDirector>();
            
            DisableAllPlayerTurrets();
            EnableAllHardpoints();
            
            switch (mode) {
                case GameMode.SinglePlayer:
                    singlePlayerP1Turret.SetActive(true);
                    singlePlayerP1ShipHardPoint.SetActive(false);
                    
                    if (gameDirector && centerPlayerShip) 
                        gameDirector.RegisterPlayerTarget(centerPlayerShip.GetComponent<HealthManager>());
                    break;

                case GameMode.CoopTwoShips:
                    coopSeparateShipsP1Turret.SetActive(true);
                    coopSeparateShipP1ShipHardPoint.SetActive(false);
                    coopSeparateShipsP2Turret.SetActive(true);
                    coopSeparateShipP2ShipHardPoint.SetActive(false);
                    
                    if (gameDirector) {
                        if (leftPlayerShip) 
                            gameDirector.RegisterPlayerTarget(leftPlayerShip.GetComponent<HealthManager>());
                        
                        if (rightPlayerShip) 
                            gameDirector.RegisterPlayerTarget(rightPlayerShip.GetComponent<HealthManager>());
                    }
                    break;

                case GameMode.CoopOneShip:
                    coopSameShipP1Turret.SetActive(true);
                    coopSameShipP1ShipHardPoint.SetActive(false);
                    coopSameShipP2Turret.SetActive(true);
                    coopSameShipP2ShipHardPoint.SetActive(false);
                    
                    if (gameDirector && centerPlayerShip)
                        gameDirector.RegisterPlayerTarget(centerPlayerShip.GetComponent<HealthManager>());
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
            // ...
        }
        
        // ...
    }
}
 ```

<br>

To handle the situation where player1 dies, but player2 is still alive, needed to modify my ``GameDirector``, so active players can be registered with the director and tracked as they die. I chose to do this with the builtin C# ``Observer Pattern``, both listening for game events and assigning methods that should execute when listened events occur. For instance when a ``playerHealth.OnZeroHealth`` event is received, the ``HandlePlayerDeath`` method is executed.

```csharp
namespace _01_Scripts.Core.Managers{
    public class GameDirector : MonoBehaviour, IService { 
        // Variables

        // ...

        private void Start() {
            if (_waveDirector) {
                _waveDirector.OnAllWavesCleared += HandleVictory;
            }
            
            if (_fleetDirector) {
                _fleetDirector.OnFleetDestroyed += HandleDefeat;
            }
        }
        
        public void RegisterPlayerTarget(HealthManager playerHealth) {
            if (!playerHealth || _playerHealths.Contains(playerHealth)) 
                return;
                
            _playerHealths.Add(playerHealth);
            _alivePlayers++;
            playerHealth.OnZeroHealth += HandlePlayerDeath;
        }

        private void OnDestroy() {
            // ...
        }

        private void HandlePlayerDeath(HealthManager player, GameObject killer) {
            if (_gameState.CurrentState == GameState.GameOver) 
                return;

            player.OnZeroHealth -= HandlePlayerDeath;
            _alivePlayers--;
            
            if (_alivePlayers <= 0) {
                HandleDefeat();
            }
        }

        private void HandleVictory() {
            StartCoroutine(EndGameRoutine(MatchResult.Victory, 4f));
        }

        private void HandleDefeat() {
            StartCoroutine(EndGameRoutine(MatchResult.Defeat, 1.5f));
        }

        // ...
    }
}
 ```

Thus, now the game continues even if one of the players die!

<img src="Blog%205%20-%20Player%20Death.jpg" alt="Player Death, yet the game plays on" width="860">

<br><br><br>

## Shaders
Shaders play a huge role in achieving convincing visuals. I implemented; 
1. Horizon fog that blends seamlessly with the skybox.
2. A lightning VFX shader simulating fast, top-to-bottom strikes.
3. An ocean movement shader (texture shifting).
4. An ocean displacement shader (vertical sway).

It was a matter of trial-and-error to get effects I am satisfied with.

<img src="Blog%205%20-%20Shaders%20created.jpg" alt="Shaders overview" width="860">

Let's look deeper into the ocean shaders:

### SH_OceanWaves

<img src="Blog%205%20-%20Ocean%20Waves%20Shader.jpg" alt="Shaders overview" width="860">

This shader consists of 3 parts:
- **Red Box:** Calculates vertical vertex positions based on the dynamic texture produced by ``SH_OceanWeatherSim``.
- **Green Box:** Handles displaying, tiling, and moving the base ocean color texture over time.
- **Yellow Box:** Applies the normal map, creating surface ripples that interact with lighting.

### SH_OceanWeatherSim
To support future water displacement from ships, I created a dedicated shader combining various sine nodes of varying sizes and directions for irregular wave patterns (instead of one big uniform wave).

<img src="Blog%205%20-%20Ocean%20Weather%20Sim.jpg" alt="Shaders overview" width="860">

This wave output is drawn onto a ``SM_WaveHeightProjectionMap`` (Marked yellow in hiearchy on screenshot below). A downward-facing camera captures this changing texture and outputs it to a ``Render Texture``. ``SH_OceanWaves`` then reads this texture to displace each ocean mesh. This approach ensures my 9 ocean planes blend seamlessly without tearing, and it makes the system scalable for future effects like ship wakes physically pushing the water down, or breaking water at their fronts. I would be able to apply these effects by combining onto the produced ``SM_WaveHeightProjectionMap`` texture, which uses grayscale coloring to determine height in the world, that would then be observed by the camera and applied seamlessly to my current ocean waves through ``SH_OceanWaves``.

<img src="Blog%205%20-%20Ocean%20Weather%20Sattelite.jpg" alt="Shaders overview" width="860">