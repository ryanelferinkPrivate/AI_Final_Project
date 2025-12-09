using UnityEngine;
using TMPro;

// controls behavior for timer UI 
public class TimerUI : MonoBehaviour
{
    // how many seconds the timer should start with
    public float countdownTime = 10f;
    private float timer;

    //text mesh that displays counter 
    public TextMeshProUGUI counterText { get; private set; }

    // determines if countdown is done 
    public bool IsFinished { get; private set; } = false;

    // get the textmesh component for counter text
    void Awake()
    {
        counterText = GetComponent<TextMeshProUGUI>();
    }

    // called when enabled
    public void OnEnable()
    {
        this.enabled = true;

        // counter not done
        this.IsFinished = false;

        // set timer to countdown time
        timer = countdownTime;

        // set counter text to active in unity 
        counterText.gameObject.SetActive(true);
    }

    // called when disabled
    public void OnDisable()
    {
        this.enabled = false;

        // set counter text to inactive
        counterText.gameObject.SetActive(false);
    }

    // update called when active per step in unity
    void Update()
    {
        // if timer not 0
        if (timer > 0)
        {
            // reduce the timer 
            timer -= Time.unscaledDeltaTime;
            // convert to min/sec 
            int timerInt = (int)Mathf.Ceil(timer);
            int minutes = timerInt/60;
            int seconds = timerInt%60;
            //build in Minute:second format
            counterText.text = (minutes >= 10 ? 
            minutes.ToString() : "0" + minutes.ToString()) + ":" +
            (seconds >= 10 ? 
            seconds.ToString() : "0" + seconds.ToString());
        }

        // otherwise when timer reached 0 set to finished and counter text to display "Runners"
        else
        {
            counterText.text = "Runners";
            IsFinished = true;
        }
    }
}
