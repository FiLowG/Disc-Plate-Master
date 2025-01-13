 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Script này có khả năng xuống dòng khi nhập thông tin đăng nhập bằng cách nhấn Enter.
/// </summary>
public class changeInput : MonoBehaviour
{
    private EventSystem input1;
    // Start is called before the first frame update
    void Start()
    {
        input1 = EventSystem.current;
    }

    // Update is called once per frame
    void Update()
    {
        bool change = Input.GetKeyDown(KeyCode.Return);
            if (change)
        {
            Selectable next = input1.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if ( next != null)
            {
                next.Select();
            }
        }
    }
}
