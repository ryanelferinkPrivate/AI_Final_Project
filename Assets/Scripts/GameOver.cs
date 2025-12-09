using UnityEngine;
using TMPro;
using UnityEngine.UI;

// class for game over ui screen
public class GameOver : MonoBehaviour
{
    // panel image component
    private Image panelImage;

    // text mesh for winner text 
    private TextMeshProUGUI winnerText;

    // gets compoent for winnter text and panel image in unity 
    void Awake()
    {
        winnerText = GetComponentInChildren<TextMeshProUGUI>();
        panelImage = GetComponent<Image>();
    }

    // determines winner of the game
    public void Winner(bool tagger)
    {
        // if tagger wins 
        if (tagger)
        {
            // displays "Taggers Win!" on a red backdrop
            winnerText.text = "Taggers Win!";
            panelImage.color = Color.red;
        } else
        {
            // displays "Runners Win!" on a blue backdrop
            winnerText.text = "Runners Win!";
            panelImage.color = Color.blue;
        }
    }

    // controls if game over panel is active or inactive 
    public void SetActive(bool active)
    {
        // set it to specified status 
        gameObject.SetActive(active);
    }
}
