using System.Collections.Generic;
using _01_Scripts.Core.Managers;
using _01_Scripts.Core.Scoring;
using _01_Scripts.Core.Services;
using _01_Scripts.Core.Utilities;
using TMPro;
using UnityEngine;
using static _01_Scripts.Core.Utilities.CursorUtilities;

namespace _01_Scripts.Core.UI
{
    /// <summary>
    /// <p>Handles the Game Over Menu, that is displayed after win/lose condition for game level is reached</p>
    ///</summary>
    public class GameOverPresenter : BaseHighscorePresenter
    { 
        [Header("UI Data and Bindings")]
        [Tooltip("The Primary Canvas that contains the Game Over UI elements")]
        [SerializeField] private GameObject gameOverCanvas;
        
        [Tooltip("A TextMeshPro Game Object which should be used for displaying the results of the Game (I.e. Victory or Defeat)")]
        [SerializeField] private TextMeshProUGUI matchResultText;
        
        [Tooltip("The text element displaying the final scores at the bottom of the screen")]
        [SerializeField] private TextMeshProUGUI playerScoresText;

        [Tooltip("Text to display in matchResultText Game Object upon Victory")]
        [SerializeField] private string victoryText = "VICTORY";
        
        [Tooltip("Color to display victory text in.")]
        [SerializeField] private Color victoryColor = Color.cyan;
        
        [Tooltip("Text to display in matchResultText Game Object upon Defeat")]
        [SerializeField] private string defeatText = "DEFEAT";
        
        [Tooltip("Color to display defeat text in.")]
        [SerializeField] private Color defeatColor = Color.red;
        
        [Header("Prefabs")]
        [Tooltip("Prefab for a row with an active InputField for the local player")]
        [SerializeField] private GameObject inputScoreRowPrefab;

        [Header("Audio")]
        [Tooltip("Audio Source to use to play Win/Lose music")]
        [SerializeField] private AudioSource audioSource;
        
        [Tooltip("Music to play upon victory")]
        [SerializeField] private AudioClip victoryAudioClip;
        
        [Tooltip("Music to play upon defeat")]
        [SerializeField] private AudioClip defeatAudioClip;

        [Header("Local Players")]
        [Tooltip("Slot all PlayerScore scripts, for players in this level, here")]
        [SerializeField] private List<PlayerScore> localPlayers;
        
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
            if (!playerScoresText) 
                return;

            string displayText = "";
            for (int i = 0; i < localPlayers.Count; i++) {
                if (localPlayers[i].CurrentScore > 0) {
                    int score = localPlayers[i].CurrentScore;
                    int rank = GetRank(score);
                    displayText += $"{localPlayers[i].DefaultPlayerName} - SCORE: {score:N0} (RANK {rank})\n";
                }
            }
            
            playerScoresText.text = displayText.TrimEnd('\n'); 
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
        
        /// <summary>
        /// Loads the Main Menu Scene.
        /// </summary>
        public void ReturnToMainMenu() {
            SceneNavigationUtilities.LoadMainMenu();
        }

        /// <summary>
        /// Temporary player scores that have not yet been submitted to the Leaderboard.
        /// </summary>
        private struct PendingEntry {
            public int Rank;
            public PlayerScore ScoreData;
        }
    }
}
