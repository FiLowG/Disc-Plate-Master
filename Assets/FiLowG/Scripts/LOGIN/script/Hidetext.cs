using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Script này có chức năng che chữ tại scene login để tạo hiệu ứng.
/// </summary>
public class ActivateOnInputFieldSelect : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public GameObject targetObject;  
    public InputField inputField;    

    private void Start()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (targetObject != null)
        {
            if (string.IsNullOrEmpty(inputField.text))
            {
                targetObject.SetActive(false);
            }
        }
    }
}
