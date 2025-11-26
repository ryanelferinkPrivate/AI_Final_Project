using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using System;
using System.Collections;


public class MenuManager : MonoBehaviour
{
    public GameObject startPage;

    public CountdownUI countdown;

    public BehaviorParameters[] agents;
    public GameObject[] markers;

    public Boolean tagger;

    public Boolean runner1;

    public Boolean runner2;


    void Start()
    {
        startPage.SetActive(true);
        countdown.enabled = false;
        Time.timeScale = 0f;
        int i = 0;
        foreach (var agent in agents) {
            agent.gameObject.SetActive(false);
            markers[i].SetActive(false);
            i++;
        }
            

    }

    public void PlayAsTagger()
    {
        tagger = true;
        StartGame();
    }

    public void PlayAsRunner(int runner)
    {
        switch (runner)
        {
            case 1: 
                runner1 = true;
                break;
            case 2: 
                runner2 = true;
                break;
        }
        StartGame();
    }


    public void StartGame()
    {

        startPage.SetActive(false);
        countdown.enabled = true;
    
        StartCoroutine(EnableAgentsAfterCountdown());
}

    private IEnumerator EnableAgentsAfterCountdown()
    {
        while (!countdown.IsFinished)
        {
            yield return null;
        }

        Time.timeScale = 1f;

        for (int i = 0; i < agents.Length; i++)
        {
            agents[i].gameObject.SetActive(true);

            if ((tagger && agents[i].gameObject.CompareTag("Tagger")) ||
                (runner1 && agents[i].gameObject.CompareTag("Runner")) ||
                (runner2 && agents[i].gameObject.CompareTag("Runner2")))
            {
                agents[i].BehaviorType = BehaviorType.HeuristicOnly;
                markers[i].SetActive(true);
            }
            else
            {
                agents[i].BehaviorType = BehaviorType.InferenceOnly;
            }
        }
    }
}
