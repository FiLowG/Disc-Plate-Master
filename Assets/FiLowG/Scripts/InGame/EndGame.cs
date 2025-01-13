using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///Script này có tác dụng kết thúc trận đấu bằng cách xác định hết lượt ném, số điểm tối đa, hết thời gian,...sau đó đẩy thông tin lên database. 
/// </summary>
public class EndGame : MonoBehaviour
{
    public Text CountDown;
    public float timeLimit = 300f;

    public Text ThrowTurn;
    public int maxThrowTurns = 10;

    public Text InsideBasket;
    public int maxInsideBasket = 4;

    public GameObject Basket;
    public Text Distance;
    public Text Final_Score;
    public GameObject Final_UI;
    private float currentTime;

    void Start()
    {
        currentTime = timeLimit;
        StartCoroutine(CountdownTimer());
    }

    void Update()
    {
        CheckThrowTurns();
        CheckInsideBasket();
        UpdateDistance();
    }

    IEnumerator CountdownTimer()
    {
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            CountDown.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
            yield return null;
        }
        EndScore();
    }

    void CheckThrowTurns()
    {
        if (int.TryParse(ThrowTurn.text, out int throwTurns))
        {
            if (throwTurns >= maxThrowTurns)
            {
                EndScore();
            }
        }
    }

    void CheckInsideBasket()
    {
        if (int.TryParse(InsideBasket.text, out int insideBasket))
        {
            if (insideBasket >= maxInsideBasket)
            {
                EndScore();
            }
        }
    }

    void UpdateDistance()
    {
        if (Basket != null)
        {
            float distance = Vector3.Distance(transform.position, Basket.transform.position);
            Distance.text = Mathf.FloorToInt(distance / 2).ToString("D2") + "m";
        }
    }

    void EndScore()
    {
        if (int.TryParse(InsideBasket.text, out int finalScore))
        {
            FindObjectOfType<Matching_Game>().TriggerSubmitScore(finalScore);
            Final_UI.SetActive(true);
            Final_Score.text = InsideBasket.text;
        }
    }
}
