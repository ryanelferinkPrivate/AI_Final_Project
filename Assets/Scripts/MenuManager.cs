using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;



public class MenuManager : MonoBehaviour
{
    public GameObject startPage;

    public GameOver endpage;

    public CountdownUI countdown;

    public TimerUI timer;

    public List<BehaviorParameters> runners;
    public List<BehaviorParameters> taggers;
    public List<GameObject> runnerMarkers;
    public List<GameObject> taggerMarkers;
    public List<BehaviorParameters> agents;
    public List<GameObject> markers;


    private string player;
    private Boolean taggerWon;
    private Dictionary<BehaviorParameters, GameObject> runnerToMarker;
    private Dictionary<BehaviorParameters, GameObject> taggerToMarker;
    private Dictionary<BehaviorParameters, GameObject> agentToMarker;

        void Awake()
    {

        // Check lists are same length
        if (runners.Count != runnerMarkers.Count)
        {
            Debug.LogError("Runners and Runner Markers lists must be the same length!");
            return;
        }

        // Check lists are same length
        if (taggers.Count != taggerMarkers.Count)
        {
            Debug.LogError("Taggers and Tagger Markers lists must be the same length!");
            return;
        }

        // Zip the lists into a dictionary
        runnerToMarker = runners.Zip(runnerMarkers, (agent, marker) => (agent, marker))
                              .ToDictionary(x => x.agent, x => x.marker);

        // Zip the lists into a dictionary
        taggerToMarker = taggers.Zip(taggerMarkers, (agent, marker) => (agent, marker))
                              .ToDictionary(x => x.agent, x => x.marker);
        agentToMarker = runnerToMarker
            .Concat(taggerToMarker)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            foreach (var kvp in agentToMarker)
            {
                Debug.Log($"Agent: {kvp.Key.name}, Marker: {kvp.Value.name}");
            }
    }


    //Starts up the menu page and disables everything else
    void Start()
    {
        endpage.SetActive(false);
        startPage.SetActive(true);
        timer.OnDisable();
        Time.timeScale = 0f;
        ToggleAgents(false);
    }

    //Controls Play as Tagger button
    public void PlayAsTagger()
    {
        player = "Tagger";
        StartGame();
    }

    //Controls Play as Runner button
    public void PlayAsRunner(int runner)
    {
        switch (runner)
        {
            case 1: 
                player = "Runner";
                break;
            case 2: 
                player = "Runner2";
                break;
        }
        StartGame();
    }

    //Method to control the Restart Button
    public void RestartGame()
    {
        player = "";
        taggerWon = false;
        Start();
    }

    //Starts up the game (including countdown)
    public void StartGame()
    {
        startPage.SetActive(false);
        endpage.SetActive(false);
    
        StartCoroutine(Game());
    }
    //Controls the game; starts and ends the game
    private IEnumerator Game()
    {
        countdown.OnEnable();
        //waits for initial countdown
        yield return new WaitUntil(() => countdown.IsFinished);

        //turns on the game
        Time.timeScale = 1f;
        timer.OnEnable();
        ToggleAgents(true);

        //waits for timer countdown or tagger wins
        yield return new WaitUntil(() => timer.IsFinished || AllRunnersFrozen());

        //turns off the game
        Time.timeScale = 0f; //ends the game
        timer.OnDisable();
        endpage.SetActive(true);
        endpage.Winner(taggerWon);
        ToggleAgents(false);
    }

    //checks if all runners are frozen
    //void: changes taggerWon to true
    private bool AllRunnersFrozen()
    {
        int total = runners.Count;
        foreach (var bp in runners)
        {
            var agent = bp.GetComponent<RunAwayAgent>();
            if (agent != null && agent.frozen)
            {
                total--;
            }
        }
        bool won = total == 0;
        taggerWon = won;
        return won;
    }

    //Toggles the agents (and their markers) on or off
    private void ToggleAgents(Boolean enable)
    {
        if(enable)
        {
            foreach(var agent in agentToMarker)
            {
                agent.Key.gameObject.SetActive(true);
                if (agent.Key.CompareTag(player))
                {
                    agent.Key.BehaviorType = BehaviorType.HeuristicOnly;
                    agent.Value.SetActive(true);
                } else
                {
                    agent.Key.BehaviorType = BehaviorType.InferenceOnly;
                }
            }
            
        } else
        {
            foreach (var item in agentToMarker) 
            {
                item.Key.gameObject.SetActive(false);
                item.Value.SetActive(false);
            }
        }
    }
}
