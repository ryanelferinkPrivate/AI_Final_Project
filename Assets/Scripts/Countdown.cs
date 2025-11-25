using UnityEngine;
using TMPro;

public class CountdownUI : MonoBehaviour
{
    public float countdownTime = 3f;
    private float timer;
    private TextMeshProUGUI counterText;
    public bool IsFinished { get; private set; } = false;
    void Awake()
    {
        counterText = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        timer = countdownTime;
        counterText.gameObject.SetActive(true);
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.unscaledDeltaTime;
            counterText.text = Mathf.Ceil(timer).ToString();
        }
        else
        {
            counterText.text = "GO!";
            IsFinished = true;
            counterText.gameObject.SetActive(false);
            this.enabled = false;
        }
    }
}