using System.Collections.Generic;
using _01_Scripts.Core.Managers;
using _01_Scripts.Core.Scoring;
using _01_Scripts.Core.Services;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static _01_Scripts.Core.Utilities.CursorUtilities;

namespace _01_Scripts.Core.UI
{
    /// <summary>
    /// <p>Handles the Game Over Menu, that is displayed after win/lose condition for game level is reached</p>
    ///</summary>
    public class GameOverPresenter : BaseHighscorePresenter
    { 
        [Header("UI Data and Bindings")]
        // TODO Add description
        [SerializeField] private GameObject gameOverCanvas;
        
        // TODO Add description
        [SerializeField] private TextMeshProUGUI matchResultText;
        
        [Tooltip("The text element displaying the final scores at the bottom of the screen")]
        [SerializeField] private TextMeshProUGUI finalTelemetryText;

        // TODO Add description
        [SerializeField] private string victoryText = "VICTORY";
        
        // TODO Add description
        [SerializeField] private Color victoryColor = Color.cyan;
        
        // TODO Add description
        [SerializeField] private string defeatText = "DEFEAT";
        
        // TODO Add description
        [SerializeField] private Color defeatColor = Color.red;
        
        [Header("Prefabs")]
        [Tooltip("Prefab for a row with an active InputField for the local player")]
        [SerializeField] private GameObject inputScoreRowPrefab;

        [Header("Audio")]
        // TODO Add description
        [SerializeField] private AudioSource audioSource;
        
        // TODO Add description
        [SerializeField] private AudioClip victoryAudioClip;
        
        // TODO Add description
        [SerializeField] private AudioClip defeatAudioClip;

        [Header("Local Players")]
        [Tooltip("Slot all PlayerScore scripts, for players in this level, here")]
        [SerializeField] private List<PlayerScore> localPlayers;
        
        [Header("Scene Navigation")]
        [Tooltip("The exact name of the Main Menu scene to load upon exit")]
        [SerializeField] private string mainMenuSceneName = "SCN_MainMenu";
        
        [Header("UI Navigation")]
        [Tooltip("The button the joystick should snap to after saving a score")]
        [SerializeField] private GameObject returnToMainMenuButton;

        private GameDirector _gameDirector;
        private int _unsavedInputsCount = 0;

        private void Awake() {
            _gameDirector = ServiceLocator.Get<GameDirector>();
        }
        
        private void Start() {
            if (gameOverCanvas)
                gameOverCanvas.SetActive(false);
            
            if (_gameDirector)
                _gameDirector.OnMatchEnded += ShowGameOverMenu;
        }
        
        private void OnDestroy() {
            if (_gameDirector)
                _gameDirector.OnMatchEnded -= ShowGameOverMenu;
        }

        private void ShowGameOverMenu(MatchResult result) {
            if (gameOverCanvas) {
                gameOverCanvas.SetActive(true);
                UnlockAndShowCursor();
            }

            if (result == MatchResult.Victory) {
                if (matchResultText) {
                    matchResultText.text = victoryText;
                    matchResultText.color = victoryColor;
                }
                
                if (audioSource && victoryAudioClip)
                    audioSource.PlayOneShot(victoryAudioClip);
                
            } else {
                if (matchResultText) {
                    matchResultText.text = defeatText;
                    matchResultText.color = defeatColor;
                }
                
                if (audioSource && defeatAudioClip)
                    audioSource.PlayOneShot(defeatAudioClip);
            }

            GenerateLeaderboard();
            GeneratePlayerSummary();
        }
        
        private void GeneratePlayerSummary() {
            if (!finalTelemetryText) 
                return;

            string displayText = "";
            for (int i = 0; i < localPlayers.Count; i++) {
                if (localPlayers[i].CurrentScore > 0) {
                    int score = localPlayers[i].CurrentScore;
                    int rank = GetRank(score);
                    displayText += $"{localPlayers[i].DefaultPlayerName} - SCORE: {score:N0} (RANK {rank})\n";
                }
            }
            
            finalTelemetryText.text = displayText.TrimEnd('\n'); 
        }
        
        /// <summary>
        /// Calculates the accurate rank of a player by evaluating both historical data 
        /// and the scores of other active players in the current session.
        /// </summary>
        private int GetRank(int playerScore) {
            int rank = 1;
            
            IReadOnlyList<ScoreEntry> topScores = LeaderboardManager.Instance.GetTopScores();
            foreach (ScoreEntry saved in topScores) {
                if (saved.score > playerScore)
                    rank++;
            }
            
            foreach (PlayerScore player in localPlayers) {
                if (player.CurrentScore > playerScore)
                    rank++;
            }
            
            return rank;
        }

        private void GenerateLeaderboard() {
            if (!LeaderboardManager.Instance) {
                Debug.LogWarning("LeaderboardManager is missing. Skipping leaderboard generation.");
                return;
            }
            
            ClearBoard();
            
            var topScores = LeaderboardManager.Instance.GetTopScores();

            List<PendingEntry> pendingEntries = new List<PendingEntry>();
            foreach (var player in localPlayers) {
                if (player.CurrentScore > 0) { 
                    int projectedRank = LeaderboardManager.Instance.GetProjectedRank(player.CurrentScore);
                    pendingEntries.Add(new PendingEntry { Rank = projectedRank, ScoreData = player });
                }
            }
            
            pendingEntries.Sort((a, b) => b.ScoreData.CurrentScore.CompareTo(a.ScoreData.CurrentScore));

            _unsavedInputsCount = pendingEntries.Count;
            
            int dataIndex = 0;
            int currentRank = 1;
            int previousScore = -1;
            
            for (int i = 0; i < 10; i++) {
                bool usePending = false;
                int currentSlotScore = 0;
                
                if (pendingEntries.Count > 0) {
                    if (dataIndex < topScores.Count) {
                        if (pendingEntries[0].ScoreData.CurrentScore >= topScores[dataIndex].score)
                            usePending = true;
                    } else {
                        usePending = true;
                    }
                }
                
                if (usePending) {
                    currentSlotScore = pendingEntries[0].ScoreData.CurrentScore;
                } else if (dataIndex < topScores.Count) {
                    currentSlotScore = topScores[dataIndex].score;
                } else {
                    InjectStaticRow(i + 1, "---", 0);
                    continue;
                }
                
                if (i > 0 && currentSlotScore != previousScore)
                    currentRank = i + 1; 
                
                if (usePending) {
                    InjectInputFieldRow(currentRank, pendingEntries[0].ScoreData); 
                    pendingEntries.RemoveAt(0);
                } else {
                    InjectStaticRow(currentRank, topScores[dataIndex].playerName, topScores[dataIndex].score);
                    dataIndex++;
                }

                previousScore = currentSlotScore;
            }
        }

        private void InjectInputFieldRow(int rank, PlayerScore localPlayer) {
            GameObject rowObj = Instantiate(inputScoreRowPrefab, leaderboardContainer);
            
            if (rowObj.TryGetComponent<InputLeaderBoardRow>(out var rowScript)) {
                rowScript.Initialize(rank, localPlayer.CurrentScore, localPlayer.DefaultPlayerName, (playerName) => {
                    
                    localPlayer.CommitScore(playerName); 
                    _unsavedInputsCount--;
                    
                    if (UnityEngine.EventSystems.EventSystem.current) {
                        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                        
                        if (_unsavedInputsCount > 0) {
                            ArcadeNameInput[] allInputs = leaderboardContainer.GetComponentsInChildren<ArcadeNameInput>();
                            foreach (var input in allInputs) {
                                if (input.interactable) {
                                    input.Select();
                                    return;
                                }
                            }
                        }
                        
                        if (returnToMainMenuButton && returnToMainMenuButton.TryGetComponent<UnityEngine.UI.Selectable>(out var selectable))
                            selectable.Select();
                    }
                });
            }
        }
        
        // TODO Add description
        public void ReturnToMainMenu() {
            ServiceLocator.Get<GameStateService>()?.ResumeGame();
            SceneManager.LoadSceneAsync(mainMenuSceneName);
        }

        // TODO Add description
        // TODO Consider moving to another class/file -> Libs perhaps?
        private struct PendingEntry {
            public int Rank;
            public PlayerScore ScoreData;
        }
    }
}
