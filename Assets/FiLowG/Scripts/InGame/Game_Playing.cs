using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Script này có tác dụng tạo thông tin trận đấu trên database, tham chiếu nó để tạo các chức năng khác như phân định thắng thua,... 
/// </summary>
public class Game_Playing : MonoBehaviour
{
    private DatabaseReference databaseRef;
    public GameObject winner;
    public GameObject loser;
    public Text PlayerScore;
    private string currentGameId;
    private Matching_Game Matching_game;
    private float checkInterval = 2f;
    private float timer = 0f;
    public Text Final_Score;
    public GameObject Final_UI;
    public Text InsideBasket;

    void Start()
    {
        Matching_game = FindObjectOfType<Matching_Game>();
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= checkInterval)
        {
            timer = 0f;
            CheckScoresAndSubmitEndGameData();
        }
    }

    private void CheckScoresAndSubmitEndGameData()
    {
        if (!string.IsNullOrEmpty(currentGameId))
        {
            databaseRef.Child("Game_Playing").Child(currentGameId).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    if (task.Result.Exists)
                    {
                        var gameData = task.Result;
                        string gEndTime = gameData.Child("EndTime").Value?.ToString();
                        string p1Score = gameData.Child("P1Score").Value?.ToString();
                        string p2Score = gameData.Child("P2Score").Value?.ToString();

                        if (p1Score != "none" || p2Score != "none")
                        {
                            string winnerName = "Draw";
                            string uidWinner = "";
                            int.TryParse(PlayerScore.text, out int Playerscore);
                            Matching_game.TriggerSubmitScore(Playerscore);
                            Final_UI.SetActive(true);
                            Final_Score.text = InsideBasket.text;

                            if (p1Score != "none" && p2Score != "none")
                            {
                                int.TryParse(p1Score, out int p1ScoreValue);
                                int.TryParse(p2Score, out int p2ScoreValue);

                                if (p1ScoreValue > p2ScoreValue)
                                {
                                    uidWinner = gameData.Child("Player1UID").Value?.ToString();
                                }
                                else if (p2ScoreValue > p1ScoreValue)
                                {
                                    uidWinner = gameData.Child("Player2UID").Value?.ToString();
                                }
                            }
                            else if (p1Score != "none")
                            {
                                uidWinner = gameData.Child("Player1UID").Value?.ToString();
                            }
                            else if (p2Score != "none")
                            {
                                uidWinner = gameData.Child("Player2UID").Value?.ToString();
                            }

                            if (!string.IsNullOrEmpty(uidWinner))
                            {
                                databaseRef.Child("users").Child(uidWinner).Child("Name").GetValueAsync().ContinueWithOnMainThread(nameTask =>
                                {
                                    if (nameTask.IsCompleted && nameTask.Result.Exists)
                                    {
                                        winnerName = nameTask.Result.Value.ToString();
                                    }
                                    SubmitEndGameData(currentGameId, winnerName);
                                });
                            }
                            else
                            {
                                SubmitEndGameData(currentGameId, winnerName);
                            }
                        }
                    }
                }
            });
        }
    }

    public void CheckAndCreateGameSession(string playerUID, string opponentUID, Action<string> onGameIdDetermined)
    {
        databaseRef.Child("Game_Playing").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                foreach (var game in task.Result.Children)
                {
                    string existingGameId = game.Key;
                    string player1UID = game.Child("Player1UID").Value?.ToString();
                    string player2UID = game.Child("Player2UID").Value?.ToString();

                    if ((player1UID == playerUID || player2UID == playerUID) &&
                        (player1UID != null && player2UID != null))
                    {
                        onGameIdDetermined?.Invoke(existingGameId);
                        return;
                    }
                }
            }

            string newGameId = Guid.NewGuid().ToString();
            currentGameId = newGameId;
            CreateGameSession(newGameId, playerUID, opponentUID);
            onGameIdDetermined?.Invoke(newGameId);
        });
    }

    private void CreateGameSession(string gameId, string player1UID, string player2UID)
    {
        string startTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        var gameData = new Dictionary<string, object>
        {
            { "GameID", gameId },
            { "StartTime", startTime },
            { "EndTime", "" },
            { "Winner", "" },
            { "P1Score", "none" },
            { "P2Score", "none" },
            { "Player1UID", player1UID },
            { "Player2UID", player2UID }
        };

        databaseRef.Child("Game_Playing").Child(gameId).SetValueAsync(gameData).ContinueWithOnMainThread(task =>
        {
        });
    }

    public void SubmitScore(string gameId, string playerUID, int playerScore)
    {
        databaseRef.Child("Game_Playing").Child(gameId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                var gameData = task.Result;
                string player1UID = gameData.Child("Player1UID").Value?.ToString();
                string player2UID = gameData.Child("Player2UID").Value?.ToString();
                int.TryParse(gameData.Child("P1Score").Value?.ToString(), out int p1Score);
                int.TryParse(gameData.Child("P2Score").Value?.ToString(), out int p2Score);

                if (player1UID == playerUID && p1Score == 0)
                {
                    UpdateScore(gameId, "P1Score", playerScore);
                }
                else if (player2UID == playerUID && p2Score == 0)
                {
                    UpdateScore(gameId, "P2Score", playerScore);
                }

                CompareScores(playerUID, p1Score, p2Score);
            }
        });
    }

    private void UpdateScore(string gameId, string scoreKey, int score)
    {
        databaseRef.Child("Game_Playing").Child(gameId).Child(scoreKey).SetValueAsync(score).ContinueWithOnMainThread(task =>
        {
        });
    }

    private void CompareScores(string playerUID, int p1Score, int p2Score)
    {
        if (p1Score > p2Score && playerUID == "Player1UID" ||
            p2Score > p1Score && playerUID == "Player2UID")
        {
            Winner();
        }
        else
        {
            Loser();
        }
    }

    public void SubmitEndGameData(string gameId, string winnerName)
    {
        string endTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        var endGameData = new Dictionary<string, object>
        {
            { "EndTime", endTime },
            { "Winner", winnerName }
        };

        databaseRef.Child("Game_Playing").Child(gameId).UpdateChildrenAsync(endGameData).ContinueWithOnMainThread(task =>
        {
        });
    }

    private void Winner()
    {
        winner.SetActive(true);
        loser.SetActive(false);
    }

    private void Loser()
    {
        winner.SetActive(false);
        loser.SetActive(true);
    }

    public string UID;

    public void IntoLobby()
    {
        if (!string.IsNullOrEmpty(UID))
        {
            databaseRef.Child("users").Child(UID).Child("IsMatching").SetValueAsync(false).ContinueWithOnMainThread(task =>
            {
            });
        }
        SceneManager.LoadScene("Lobby");
    }
}
