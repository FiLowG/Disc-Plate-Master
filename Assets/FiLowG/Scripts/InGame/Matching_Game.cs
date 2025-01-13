using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Script này có tác dụng ghép người chơi với 1 người chơi khác bằng các thuộc tính trên Database.
/// </summary>
public class Matching_Game : MonoBehaviour
{
    private Text UID;
    public Text Player_Name;
    public Text Enemy_Name;
    public GameObject UI_Matched1;
    public GameObject UI_Matching;
    public GameObject UI_CountDown;
    private FirebaseAuth auth;
    private DatabaseReference databaseRef;
    public GameObject Matching_Scripts;
    private Game_Playing gamePlaying;
    private bool isMatchFound = false;
    private string currentGameId;
    Animator countDownAnimator;

    void Start()
    {
        gamePlaying.UID = UID.text;
        countDownAnimator = UI_CountDown.GetComponent<Animator>();
        gamePlaying = FindObjectOfType<Game_Playing>();
        UI_Matched1.SetActive(false);
        UI_Matching.SetActive(true);
        auth = FirebaseAuth.DefaultInstance;
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        GameObject uidDisplayObject = GameObject.Find("UID_DISPLAY");
        if (uidDisplayObject != null)
        {
            Transform codeTransform = uidDisplayObject.transform.Find("CODE");
            if (codeTransform != null)
            {
                UID = codeTransform.GetComponent<Text>();
                if (UID != null)
                {
                    ResetMatchingWith();
                    SetOnDisconnect();
                }
            }
        }
        DontDestroyOnLoad(Matching_Scripts);
        StartCoroutine(CallFindMatch());
    }

    IEnumerator CallFindMatch()
    {
        while (UI_Matching.activeSelf)
        {
            FindMatchingPlayer();
            yield return new WaitForSeconds(0.5f);
        }
    }

    void ResetMatchingWith()
    {
        string currentUID = UID?.text.Trim();
        if (string.IsNullOrEmpty(currentUID)) return;
        databaseRef.Child("users").Child(currentUID).Child("MatchingWith").SetValueAsync("none");
    }

    void SetOnDisconnect()
    {
        string currentUID = UID?.text.Trim();
        if (string.IsNullOrEmpty(currentUID)) return;
        databaseRef.Child("users").Child(currentUID).Child("IsMatching").OnDisconnect().SetValue(false);
        databaseRef.Child("users").Child(currentUID).Child("MatchingWith").OnDisconnect().SetValue("none");
    }

    void FindMatchingPlayer()
    {
        if (isMatchFound) return;
        string currentUID = UID?.text.Trim();
        if (string.IsNullOrEmpty(currentUID)) return;
        databaseRef.Child("users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                var users = task.Result.Children;
                List<string> eligibleUsers = new List<string>();
                foreach (var user in users)
                {
                    string uid = user.Key;
                    bool isMatching = user.Child("IsMatching").Value != null &&
                                      bool.Parse(user.Child("IsMatching").Value.ToString());
                    if (isMatching && uid != currentUID)
                    {
                        eligibleUsers.Add(uid);
                    }
                }
                if (eligibleUsers.Count > 0)
                {
                    string opponentUID = eligibleUsers[UnityEngine.Random.Range(0, eligibleUsers.Count)];
                    databaseRef.Child("users").Child(currentUID).Child("MatchingWith").SetValueAsync(opponentUID);
                    databaseRef.Child("users").Child(opponentUID).Child("MatchingWith").SetValueAsync(currentUID);
                    databaseRef.Child("users").Child(opponentUID).Child("Name").GetValueAsync().ContinueWithOnMainThread(nameTask =>
                    {
                        if (nameTask.IsCompleted && nameTask.Result.Exists)
                        {
                            string enemyName = nameTask.Result.Value.ToString();
                            Enemy_Name.text = enemyName;
                            UI_Matched1.SetActive(true);
                            UI_Matching.SetActive(false);
                            string gameId = Guid.NewGuid().ToString();
                            gamePlaying.CheckAndCreateGameSession(currentUID, opponentUID, gameId =>
                            {
                                isMatchFound = true;
                                currentGameId = gameId;
                                UI_CountDown.SetActive(true);
                                countDownAnimator.Play("COUNTDOWN");
                            });
                        }
                    });
                }
            }
        });
    }

    public void TriggerSubmitScore(int playerScore)
    {
        string currentUID = UID?.text.Trim();
        if (!string.IsNullOrEmpty(currentGameId) && !string.IsNullOrEmpty(currentUID))
        {
            gamePlaying.SubmitScore(currentGameId, currentUID, playerScore);
        }
    }

    public void LoadMap()
    {
        SceneManager.LoadScene("MAP_MULTIPLAYER");
        countDownAnimator.speed = 0;
    }

    public void LeaveQueue()
    {
        string currentUID = UID?.text.Trim();
        if (!string.IsNullOrEmpty(currentUID))
        {
            databaseRef.Child("users").Child(currentUID).Child("IsMatching").SetValueAsync(false);
            databaseRef.Child("users").Child(currentUID).Child("MatchingWith").SetValueAsync("none");
        }
    }

    void OnApplicationQuit()
    {
        LeaveQueue();
    }
}
