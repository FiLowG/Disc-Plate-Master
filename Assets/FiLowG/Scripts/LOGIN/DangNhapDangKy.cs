using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Database;

/// <summary>
///Script này có tác dụng đăng ký tài khoản, đăng nhập tài khoản bằng Firebase. 
/// </summary>
public class DangNhapDangKy : MonoBehaviour
{
    public InputField TaiKhoan;
    public InputField MatKhau;
    public InputField NhapLaiMatKhau;
    public GameObject NoticeObject;
    public Text NoticeText;
    public Text UIDText;
    public GameObject UIDContainer;

    private FirebaseAuth auth;
    private DatabaseReference databaseRef;
    private Input_Name_Player SetName;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        SetName = FindObjectOfType<Input_Name_Player>();
        if (UIDContainer != null)
        {
            DontDestroyOnLoad(UIDContainer);
            UIDContainer.SetActive(false);
        }
        NoticeObject.SetActive(false);
    }

    private void ShowNotice(string message)
    {
        NoticeText.text = message;
        NoticeObject.SetActive(true);
    }

    public void DangKy()
    {
        string email = TaiKhoan.text.Trim();
        string matkhau = MatKhau.text.Trim();
        string nhapLaiMatKhau = NhapLaiMatKhau.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(matkhau) || string.IsNullOrEmpty(nhapLaiMatKhau))
        {
            ShowNotice("Không được bỏ trống thông tin!");
            return;
        }

        if (!email.EndsWith("@gmail.com"))
        {
            ShowNotice("Định dạng Email không hợp lệ!");
            return;
        }

        if (matkhau != nhapLaiMatKhau)
        {
            ShowNotice("Mật khẩu không trùng nhau!");
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(email, matkhau).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                if (task.Exception != null && task.Exception.InnerExceptions.Count > 0)
                {
                    var errorMessage = task.Exception.InnerExceptions[0].Message;
                    if (errorMessage.Contains("already in use"))
                    {
                        ShowNotice("Email đã tồn tại!");
                    }
                    else
                    {
                        ShowNotice("Đăng ký thất bại!");
                    }
                }
                else
                {
                    ShowNotice("Đăng ký thất bại!");
                }
                return;
            }

            if (task.IsCompleted)
            {
                ShowNotice("Tài khoản đã được đăng ký thành công!");
                if (SetName != null)
                {
                    SetName.SaveUserData(email, matkhau);
                }
            }
        });
    }

    public void DangNhap()
    {
        string email = TaiKhoan.text.Trim();
        string matkhau = MatKhau.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(matkhau))
        {
            ShowNotice("Không được bỏ trống thông tin!");
            return;
        }

        if (!email.EndsWith("@gmail.com"))
        {
            ShowNotice("Định dạng Email không hợp lệ!");
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(email, matkhau).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                ShowNotice("Tài khoản hoặc mật khẩu không chính xác!");
                return;
            }

            if (task.IsCompleted)
            {
                UIDContainer.SetActive(true);
                string uid = auth.CurrentUser.UserId;
                UIDText.text = uid;
                ShowNotice("Đăng nhập thành công!");
                SceneManager.LoadScene("Lobby");
            }
        });
    }
}
