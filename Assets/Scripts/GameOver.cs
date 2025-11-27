using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class GameOver : MonoBehaviour
{
    private Image panelImage;
    private TextMeshProUGUI winnerText;

    void Awake()
    {
        winnerText = GetComponentInChildren<TextMeshProUGUI>();
        panelImage = GetComponent<Image>();
    }

    public void Winner(bool tagger)
    {
        if (tagger)
        {
            winnerText.text = "Taggers Win!";
            panelImage.color = Color.red;
        } else
        {
            winnerText.text = "Runners Win!";
            panelImage.color = Color.blue;
        }
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
