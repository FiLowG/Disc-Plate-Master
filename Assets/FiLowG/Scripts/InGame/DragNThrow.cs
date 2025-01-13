using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script này có chức năng ném đĩa bằng cách tính khoảng cách từ 1 điểm đến Icon đĩa.
/// </summary>
public class DragNThrow : MonoBehaviour
{
    public Canvas canvas;
    public Image draggableImage;
    public Transform pointDisc, pointPUT;
    public Transform Fly_Direction;
    public GameObject Sign_Drag;
    public GameObject Plate;
    public GameObject CanvasDrag1, CanvasDrag2;
    public float Distance_Limit;
    public float velocityFactor = 1.0f;
    public float liftDuration = 2.0f;
    public float liftMultiplier = 0.5f;
    public Text Throwed;

    private RectTransform imageRectTransform;
    private bool isDragging = false;
    private Rigidbody plateRigidbody;
    private float initialX;
    private bool isThrowing = false;
    private bool hasResetPlate = false;
    private bool isReadyToThrow = false;
    private CamFollow camfl;
    private Coroutine liftCoroutine;

    void Start()
    {
        camfl = FindObjectOfType<CamFollow>();
        if (draggableImage != null)
        {
            imageRectTransform = draggableImage.GetComponent<RectTransform>();
            initialX = imageRectTransform.localPosition.x;
        }
        if (Plate != null)
        {
            plateRigidbody = Plate.GetComponent<Rigidbody>();
            if (plateRigidbody != null)
            {
                plateRigidbody.isKinematic = true;
            }
            if (pointPUT != null)
            {
                Plate.transform.position = pointPUT.position;
                Plate.transform.rotation = pointPUT.rotation;
                Plate.transform.localScale = pointPUT.localScale;
            }
        }
    }

    void Update()
    {
       
        if (isThrowing)
        {
            CanvasDrag1.SetActive(false);
            CanvasDrag2.SetActive(false);

            if (plateRigidbody.velocity.magnitude <= 0.01f)
            {
                isThrowing = false;
                isReadyToThrow = false;

                if (liftCoroutine != null)
                {
                    StopCoroutine(liftCoroutine);
                    liftCoroutine = null;
                }
            }
            hasResetPlate = false;
        }
        else
        {
            CanvasDrag1.SetActive(true);
            CanvasDrag2.SetActive(true);

            if (!hasResetPlate)
            {
                hasResetPlate = true;
                isReadyToThrow = true;
            }
        }

        Sign_Drag.SetActive(!isDragging);

        if (isDragging)
        {
            DragImageToMouse();
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            StopDragging();
        }
    }

    public void StartDragging()
    {
        isDragging = true;
    }

    public void StopDragging()
    {

        camfl.setcam = true;
        if (!isReadyToThrow) return;

        isDragging = false;

        Vector2 imagePosition = imageRectTransform.localPosition;
        Vector2 pointDiscPosition = pointDisc.localPosition;
        float distance = Vector2.Distance(imagePosition, pointDiscPosition);
        if (distance >= 75f)
        {
            float velocity = distance / velocityFactor;

            if (plateRigidbody != null)
            {
                plateRigidbody.isKinematic = false;

                Vector3 velocityDirection = (Fly_Direction.position - Plate.transform.position).normalized;

                liftCoroutine = StartCoroutine(LiftDisc());
                plateRigidbody.velocity = velocityDirection * velocity;

                LockRotation();
                Invoke(nameof(UnlockRotation), 1.0f);
            }

            isThrowing = true;

            draggableImage.transform.position = pointDisc.position;

            if (Throwed != null && int.TryParse(Throwed.text, out int currentThrows))
            {
                currentThrows++;
                Throwed.text = currentThrows.ToString();
            }
        }
        else
        {
            draggableImage.transform.position = pointDisc.position;
        }
    }
        private IEnumerator LiftDisc()
        {
            float elapsedTime = 0f;

            while (elapsedTime < liftDuration)
            {
                float progress = elapsedTime / liftDuration;
                float liftFactor = Mathf.Lerp(1.0f, 0.5f, progress);

                float liftAmount = liftMultiplier * liftFactor * Time.deltaTime;

                Plate.transform.position += new Vector3(0, liftAmount, 0);

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    
    private void DragImageToMouse()
    {
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out mousePosition);

        Vector2 pointDiscPosition = pointDisc.localPosition;
        float distance = Vector2.Distance(mousePosition, pointDiscPosition);

        if (distance > Distance_Limit)
        {
            Vector2 direction = (mousePosition - pointDiscPosition).normalized;
            mousePosition = pointDiscPosition + direction * Distance_Limit;
        }

        imageRectTransform.localPosition = mousePosition;
    }

    private void LockRotation()
    {
        plateRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void UnlockRotation()
    {
        plateRigidbody.constraints = RigidbodyConstraints.None;
    }

 
}
