using UnityEngine;

/// <summary>
/// Script này có tác dụng bật tắt âm thanh, tắt mở UI setting.
/// </summary>
public class OnOFF_Volume : MonoBehaviour
{
    public GameObject Volume;
    public GameObject LogOut;

    void Start()
    {
    }

    public void ToggleObjects()
    {
        if (Volume != null)
        {
            Volume.SetActive(!Volume.activeSelf);
        }

        if (LogOut != null)
        {
            LogOut.SetActive(!LogOut.activeSelf);
        }
    }

    public void DisableAudio()
    {
        AudioListener.volume = 0f;
    }

    public void EnableAudio()
    {
        AudioListener.volume = 1f;
    }
}
