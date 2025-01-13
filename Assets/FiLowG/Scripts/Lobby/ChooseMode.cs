using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script này có tác dụng điều khiển UI và chuyển hướng sang chế độ chơi Online hoặc Offline.
/// </summary>
public class ChooseMode : MonoBehaviour
{
    public GameObject buttonOnline;
    public GameObject buttonOffline;

    void Start()
    {
        buttonOffline.SetActive(false);
        buttonOnline.SetActive(false);
    }
    public void SetOfflineMode()
    {
        buttonOffline.SetActive(true);
        buttonOnline.SetActive(false);
    }

    public void SetOnlineMode()
    {
        buttonOnline.SetActive(true);
        buttonOffline.SetActive(false);
    }
}
