using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    public float countdownTime = 10f;
    private float timer;
    public TextMeshProUGUI counterText { get; private set; }
    public bool IsFinished { get; private set; } = false;
    void Awake()
    {
        counterText = GetComponent<TextMeshProUGUI>();
    }

    public void OnEnable()
    {
        this.enabled = true;
        this.IsFinished = false;
        timer = countdownTime;
        counterText.gameObject.SetActive(true);
    }

    public void OnDisable()
    {
        this.enabled = false;
        counterText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.unscaledDeltaTime;
            int timerInt = (int)Mathf.Ceil(timer);
            int minutes = timerInt/60;
            int seconds = timerInt%60;
            counterText.text = (minutes >= 10 ? 
            minutes.ToString() : "0" + minutes.ToString()) + ":" +
            (seconds >= 10 ? 
            seconds.ToString() : "0" + seconds.ToString());
        }
        else
        {
            counterText.text = "Runners";
            IsFinished = true;
        }
    }
}
