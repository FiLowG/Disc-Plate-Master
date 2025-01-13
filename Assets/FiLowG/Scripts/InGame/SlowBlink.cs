using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script này có nhiệm vụ nhấp nháy 1 object được chỉ định, dùng để tạo hiêun ứng
/// </summary>
public class SlowBlink : MonoBehaviour
{
    public Image targetImage, targetImage2;
    public float blinkSpeed = 5f;

    private float alphaMin = 20f / 255f;
    private float alphaMax = 1f;
    private bool increasing = true;

    void Update()
    {
        if (targetImage == null) return;

        Color color = targetImage.color;

        if (increasing)
        {
            color.a += blinkSpeed * Time.deltaTime;
            if (color.a >= alphaMax)
            {
                color.a = alphaMax;
                increasing = false;
            }
        }
        else
        {
            color.a -= blinkSpeed * Time.deltaTime;
            if (color.a <= alphaMin)
            {
                color.a = alphaMin;
                increasing = true;
            }
        }

        targetImage.color = color;

        if (targetImage2 == null) return;

        Color color2 = targetImage2.color;

        if (increasing)
        {
            color2.a += blinkSpeed * Time.deltaTime;
            if (color2.a >= alphaMax)
            {
                color2.a = alphaMax;
                increasing = false;
            }
        }
        else
        {
            color2.a -= blinkSpeed * Time.deltaTime;
            if (color2.a <= alphaMin)
            {
                color2.a = alphaMin;
                increasing = true;
            }
        }

        targetImage2.color = color2;
    }
}
