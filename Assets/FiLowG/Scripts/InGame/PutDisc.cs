using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script này có tác dụng xoay đĩa về phía rổ sau khi ném.
/// </summary>
public class PutDisc : MonoBehaviour
{
    public GameObject Disc; 
    public GameObject Basket;
    public float rotationSpeed = 100f; 
    private bool isRotating = true;    

    void Start()
    {
        RotateDiscToBasket();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRotating && Disc != null && Basket != null)
        {
            Vector3 directionFromBasket = Disc.transform.position - Basket.transform.position;
            directionFromBasket.y = 0; 

            Vector3 currentForward = Disc.transform.forward;
            currentForward.y = 0;

            float angleToBasket = Vector3.Angle(currentForward, directionFromBasket);

            if (angleToBasket > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionFromBasket);
                Disc.transform.rotation = Quaternion.RotateTowards(
                    Disc.transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
            else
            {
                isRotating = false;
            }
        }
    }
    public void RotateDiscToBasket()
    {
        if (Disc == null || Basket == null)
        {
            return;
        }

        isRotating = true;
    }
}
