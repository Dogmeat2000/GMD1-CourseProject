using System.Collections.Generic;
using _01_Scripts.Core.Managers;
using _01_Scripts.Core.Scoring;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static _01_Scripts.Core.Utilities.CursorUtilities;

namespace _01_Scripts.Core.UI
{
    /** <summary>
     * <p>Handles the Game Over Menu, that is displayed after win/lose condition for game level is reached</p>
     * </summary>
     * */
    public class GameOverPresenter : BaseHighscorePresenter
    { 
        [Header("UI Data and Bindings")]
        [SerializeField] 
        private GameObject gameOverCanvas;
        
        [SerializeField] 
        private TextMeshProUGUI matchResultText;
        
        [Tooltip("The text element displaying the final scores at the bottom of the screen")]
        [SerializeField] 
        private TextMeshProUGUI finalTelemetryText;

        [SerializeField] 
        private string victoryText = "VICTORY";
        
        [SerializeField] 
        private Color victoryColor = Color.cyan;
        
        [SerializeField] 
        private string defeatText = "DEFEAT";
        
        [SerializeField] 
        private Color defeatColor = Color.red;
        
        [Header("Prefabs")]
        [Tooltip("Prefab for a row with an active InputField for the local player")]
        [SerializeField] 
        private GameObject inputScoreRowPrefab;

        [Header("Audio")]
        [SerializeField] 
        private AudioSource audioSource;
        
        [SerializeField] 
        private AudioClip victoryAudioClip;
        
        [SerializeField] 
        private AudioClip defeatAudioClip;

        [Header("Local Players")]
        [Tooltip("Slot all PlayerScore scripts, for players in this level, here")]
        [SerializeField] 
        private List<PlayerScore> localPlayers;
        
        [Header("Scene Navigation")]
        [Tooltip("The exact name of the Main Menu scene to load upon exit")]
        [SerializeField] 
        private string mainMenuSceneName = "SCN_MainMenu";
        
        [Header("UI Navigation")]
        [Tooltip("The button the joystick should snap to after saving a score")]
        [SerializeField] 
        private GameObject returnToMainMenuButton;
        
        private void Start() {
            if (gameOverCanvas) {
                gameOverCanvas.SetActive(false);
            }
            
            if (GameDirector.Instance) {
                GameDirector.Instance.OnMatchEnded += ShowGameOverMenu;
            }
        }
        
        private void OnDestroy() {
            if (GameDirector.Instance) {
                GameDirector.Instance.OnMatchEnded -= ShowGameOverMenu;
            }
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
                if (audioSource && victoryAudioClip) {
                    audioSource.PlayOneShot(victoryAudioClip);
                }
            } else {
                if (matchResultText) {
                    matchResultText.text = defeatText;
                    matchResultText.color = defeatColor;
                }
                if (audioSource && defeatAudioClip) {
                    audioSource.PlayOneShot(defeatAudioClip);
                }
            }

            GenerateLeaderboard();
            GeneratePlayerSummary();
        }
        
        private void GeneratePlayerSummary() {
            if (!finalTelemetryText) 
                return;

            string displayText = "";
            for (int i = 0; i < localPlayers.Count; i++) {
                int score = localPlayers[i].CurrentScore;
                int rank = LeaderboardManager.Instance.GetProjectedRank(score);
                displayText += $"PLAYER {i + 1} - SCORE: {score:N0} (RANK {rank})\n";
            }
            
            finalTelemetryText.text = displayText.TrimEnd('\n'); 
        }

        private void GenerateLeaderboard() {
            if (!LeaderboardManager.Instance) {
                Debug.LogWarning("LeaderboardManager is missing. Skipping leaderboard generation.");
                return;
            }
            
            ClearBoard();
            
            var topScores = LeaderboardManager.Instance.GetTopScores();
            int currentRankOffset = 0;

            List<PendingEntry> pendingEntries = new List<PendingEntry>();
            foreach (var player in localPlayers) {
                if (player.CurrentScore > 0) { 
                    int projectedRank = LeaderboardManager.Instance.GetProjectedRank(player.CurrentScore);
                    if (projectedRank <= 10) {
                        pendingEntries.Add(new PendingEntry { Rank = projectedRank, ScoreData = player });
                    }
                }
            }
            
            pendingEntries.Sort((a, b) => a.Rank.CompareTo(b.Rank));

            int dataIndex = 0;
            for (int displayRank = 1; displayRank <= 10; displayRank++) {
                if (pendingEntries.Count > 0 && pendingEntries[0].Rank == displayRank) {
                    InjectInputFieldRow(displayRank, pendingEntries[0].ScoreData);
                    pendingEntries.RemoveAt(0);
                    currentRankOffset++;
                    continue;
                }
                
                if (dataIndex < topScores.Count) {
                    InjectStaticRow(displayRank, topScores[dataIndex].playerName, topScores[dataIndex].score);
                    dataIndex++;
                } else if (pendingEntries.Count == 0) {
                    InjectStaticRow(displayRank, "---", 0);
                }
            }
        }

        private void InjectInputFieldRow(int rank, PlayerScore localPlayer) {
            GameObject rowObj = Instantiate(inputScoreRowPrefab, leaderboardContainer);
            
            if (rowObj.TryGetComponent<InputLeaderBoardRow>(out var rowScript)) {
                rowScript.Initialize(rank, localPlayer.CurrentScore, localPlayer.CommitScore);
                
                rowScript.Initialize(rank, localPlayer.CurrentScore, (playerName) => {
                    localPlayer.CommitScore(playerName); 
                    if (returnToMainMenuButton && UnityEngine.EventSystems.EventSystem.current) {
                        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(returnToMainMenuButton);
                        if (returnToMainMenuButton.TryGetComponent<UnityEngine.UI.Selectable>(out var selectable)) {
                            selectable.Select();
                        }
                    }
                });
            }
        }
        
        public void ReturnToMainMenu() {
            Time.timeScale = 1f;
            SceneManager.LoadSceneAsync(mainMenuSceneName);
        }

        private struct PendingEntry {
            public int Rank;
            public PlayerScore ScoreData;
        }
    }
}
