using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangKyNgay : MonoBehaviour
{
    public GameObject popUpLogin;
    public GameObject popUpSignUp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DangKyNow()
    {
        popUpLogin.SetActive(false);
        popUpSignUp.SetActive(true);
    }
    public void Dacotaikhoan()
    {
        popUpLogin.SetActive(true);
        popUpSignUp.SetActive(false);
    }
}
