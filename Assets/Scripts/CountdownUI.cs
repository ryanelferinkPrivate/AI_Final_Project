using UnityEngine;
using TMPro;

// class for the countdown in game UI
public class CountdownUI : MonoBehaviour
{
    // how long the countdown time is 
    public float countdownTime = 3f;
    private float timer;

    // dusplays countdown as text 
    private TextMeshProUGUI counterText;

    // determines if countdown is complete 
    public bool IsFinished { get; private set; } = false;

    // match counter text with the text mesh in unity
    void Awake()
    {
        counterText = GetComponent<TextMeshProUGUI>();
    }

    // when user presses play on unity 
    public void OnEnable()
    {
        // script behavior enabled in unity  
        this.enabled = true;

        // countdown just started so not finished
        this.IsFinished = false;

        // timer starts at countdowntme
        timer = countdownTime;

        //makes text seen in the unity environment
        counterText.gameObject.SetActive(true);
    }

    // when script is done running in the game 
    public void OnDisable()
    {
        // behavior is no longer enabled
        this.enabled = false;

        // text object in game is no longer active
        counterText.gameObject.SetActive(false);
    }

    // update the countdown in unity 
    void Update()
    {
        // if timer is not 0
        if (timer > 0)
        {
            // countdown timer 
            timer -= Time.unscaledDeltaTime;

            // display the change in the timer on screen
            counterText.text = Mathf.Ceil(timer).ToString();
        }

        // otherwise when countdown is up
        else
        {
            // countdown is finished 
            IsFinished = true;

            // text object in game is no longer active
            counterText.gameObject.SetActive(false);

            // // behavior is no longer enabled
            this.enabled = false;
        }
    }
}