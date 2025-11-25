using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject startPage;

    public GameObject[] agents;

    void Start()
    {
        startPage.SetActive(true);
        foreach (var agent in agents)
            agent.SetActive(false);
    }

    public void StartGame()
    {
        foreach (var agent in agents)
            agent.SetActive(true);
        startPage.SetActive(false);
    }
}
