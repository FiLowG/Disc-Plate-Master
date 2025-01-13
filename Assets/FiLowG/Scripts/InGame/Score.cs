using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Script này có tác dụng tính điểm mỗi khi đĩa rơi vào rổ.
/// </summary>
public class Score : MonoBehaviour
{
    public Text Score_Display;
    private int score = 0;
    private bool canCollide = true;
    public float collisionCooldown = 10f;
    public float fixedVelocity = 0.3f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        UpdateScoreDisplay();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("basket") && canCollide)
        {
            score++;
            UpdateScoreDisplay();

            if (rb != null && Mathf.Abs(rb.velocity.z - fixedVelocity) > 0.01f)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, fixedVelocity);
            }

            StartCoroutine(CollisionCooldown());
        }
    }

    private void UpdateScoreDisplay()
    {
        if (Score_Display != null && Score_Display.text != score.ToString())
        {
            Score_Display.text = score.ToString();
        }
    }

    private IEnumerator CollisionCooldown()
    {
        canCollide = false;
        yield return new WaitForSeconds(collisionCooldown);
        canCollide = true;
    }
}
