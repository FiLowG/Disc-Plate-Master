using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Script này có tác dụng khai báo 1 contructor dữ liệu người chơi để chuyển lên database, hiển thị và đặt tên nhân vật.
/// </summary>
public class Input_Name_Player : MonoBehaviour
{
    public GameObject Put_Name;
    public InputField Input_Name;
    public Text Name_Display;
    private FirebaseAuth auth;
    private DatabaseReference databaseRef;
    private bool SetMatching;

    void Start()
    {
        SetMatching = false;
        auth = FirebaseAuth.DefaultInstance;
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        if (Put_Name != null)
        {
            Put_Name.SetActive(false);
        }

        CheckUserName();
        SetNameDisplay();

        InvokeRepeating(nameof(CheckIsMatching), 0f, 1f);
    }

    void CheckIsMatching()
    {
        if (SceneManager.GetActiveScene().name != "LoginLogout")
        {
            FirebaseUser user = auth.CurrentUser;
            if (user != null)
            {
                string uid = user.UserId;
                databaseRef.Child("users").Child(uid).Child("IsMatching").GetValueAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted && task.Result.Exists)
                    {
                        bool isMatchingValue = bool.Parse(task.Result.Value.ToString());
                    }
                });
            }
        }
    }

    public void SaveUserData(string email, string password)
    {
        FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            string uid = user.UserId;

            User newUser = new User
            {
                UID = uid,
                Email = email,
                Password = password,
                Name = "guest",
                IsMatching = SetMatching,
                MatchingWith = null
            };

            databaseRef.Child("users").Child(uid).SetRawJsonValueAsync(JsonUtility.ToJson(newUser)).ContinueWithOnMainThread(task =>
            {
            });
        }
    }

    public void CheckUserName()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            string uid = user.UserId;

            databaseRef.Child("users").Child(uid).Child("Name").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.Result.Exists)
                {
                    string currentName = task.Result.Value.ToString();

                    if (currentName == "guest" && Put_Name != null)
                    {
                        Put_Name.SetActive(true);
                    }
                }
            });
        }
    }

    public void SubmitName()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            string uid = user.UserId;
            string newName = Input_Name.text.Trim();

            if (string.IsNullOrEmpty(newName))
            {
                return;
            }

            databaseRef.Child("users").Child(uid).Child("Name").SetValueAsync(newName).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    if (Put_Name != null)
                    {
                        Put_Name.SetActive(false);
                    }
                    SetNameDisplay();
                }
            });
        }
    }

    public void SetNameDisplay()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            string uid = user.UserId;

            databaseRef.Child("users").Child(uid).Child("Name").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.Result.Exists)
                {
                    string currentName = task.Result.Value.ToString();
                    Name_Display.text = currentName;
                }
            });
        }
    }

    public void SetMatchingTrue()
    {
        UpdateMatchingStatus(true);
    }

    public void SetMatchingFalse()
    {
        UpdateMatchingStatus(false);
    }

    private void UpdateMatchingStatus(bool status)
    {
        FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            string uid = user.UserId;

            databaseRef.Child("users").Child(uid).Child("IsMatching").SetValueAsync(status).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    SetMatching = status;
                }
            });
        }
    }

    [System.Serializable]
    public class User
    {
        public string UID;
        public string Email;
        public string Password;
        public string Name;
        public bool IsMatching;
        public string MatchingWith;
    }
}
