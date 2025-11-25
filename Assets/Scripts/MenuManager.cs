using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using System;


public class MenuManager : MonoBehaviour
{
    public GameObject startPage;

    public GameObject countdown;

    public BehaviorParameters[] agents;

    public Boolean tagger;

    public Boolean runner;


    void Start()
    {
        startPage.SetActive(true);
        Time.timeScale = 0f;

        foreach (var agent in agents)
            agent.gameObject.SetActive(false);
    }

    public void PlayAsTagger()
    {
        tagger = true;
        StartGame();
    }

    public void PlayAsRunner()
    {
        runner = true;
        StartGame();
    }


    public void StartGame()
    {

        startPage.SetActive(false);

        Time.timeScale = 1f;

        foreach (var agent in agents)
        {
            agent.gameObject.SetActive(true);
            if ((tagger && agent.gameObject.tag == "Tagger") || (runner && agent.gameObject.tag == "Runner"))
            {
                agent.BehaviorType = BehaviorType.HeuristicOnly;
            } else
            {
                agent.BehaviorType = BehaviorType.InferenceOnly;
            }

        }
    }
}
