using System.Collections;
using UnityEngine;

/// <summary>
/// Script này có tác dụng điều khiển camera và đĩa để sau mỗi lần ném, đĩa và camera có thể được đặt ở vị trí hợp lý.
/// </summary>
public class CamFollow : MonoBehaviour
{
    public GameObject camIdle1, camIdle2;
    public GameObject camFollow;
    public GameObject disc;
    public Transform pointDisc;
    public Transform Basket;
    public GameObject Put_MainCam;
    public GameObject Fly_To_It;
    public float distanceThreshold = 5f;
    public float followSpeed = 5f;
    private float zOffset = -3.55f;
    public float timescale;
    private Rigidbody discRb;
    public bool setcam = false;
    public GameObject RotationFind;
    public PutDisc putDisc;

    void Start()
    {
        putDisc = FindObjectOfType<PutDisc>();
        Time.timeScale = timescale;
        discRb = disc.GetComponent<Rigidbody>();
    }

    void Update()
    {
        float distance = Vector3.Distance(disc.transform.position, pointDisc.position);
        FollowDisc();

        if (distance >= distanceThreshold)
        {
            camIdle1.SetActive(false);
            camIdle2.SetActive(false);
            camFollow.SetActive(true);
        }

        if (discRb.velocity.magnitude == 0)
        {
            disc.transform.rotation = Quaternion.Euler(
                0f,
                disc.transform.rotation.eulerAngles.y,
                0f
            );

            camIdle1.SetActive(true);
            camIdle2.SetActive(true);
            camFollow.SetActive(false);

            camIdle1.transform.position = Put_MainCam.transform.position;
            camIdle1.transform.rotation = Put_MainCam.transform.rotation;

            camIdle2.transform.position = Put_MainCam.transform.position;
            camIdle2.transform.rotation = Put_MainCam.transform.rotation;

            if (!discRb.isKinematic)
            {
                disc.transform.position = pointDisc.position;
                disc.transform.rotation = pointDisc.rotation;

                discRb.isKinematic = true;
            }

            if (setcam)
            {
                Vector3 yAdjustment = new Vector3(0, 1, 0);

                disc.transform.position += yAdjustment;
                pointDisc.position += yAdjustment;
                camIdle1.transform.position += yAdjustment;
                camIdle2.transform.position += yAdjustment;
                setcam = false;
                putDisc.RotateDiscToBasket();
            }

            if (disc.transform.position.z < Basket.position.z)
            {
                camFollow.transform.rotation = Quaternion.Euler(camFollow.transform.rotation.eulerAngles.x, 0.08f, camFollow.transform.rotation.eulerAngles.z);
                zOffset = -7.01f;
            }
            else
            {
                camFollow.transform.rotation = Quaternion.Euler(camFollow.transform.rotation.eulerAngles.x, -178.025f, camFollow.transform.rotation.eulerAngles.z);
                zOffset = 3.55f;
            }
        }
    }

    void FollowDisc()
    {
        Vector3 targetPosition = new Vector3(
            camFollow.transform.position.x,
            disc.transform.position.y + 7,
            disc.transform.position.z + zOffset
        );

        camFollow.transform.position = Vector3.Lerp(
            camFollow.transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );
    }
}
