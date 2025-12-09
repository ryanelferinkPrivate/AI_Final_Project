using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;


// class for main menu of game, game start, countdown, timer, and
// which agent player can play as and watch the AI play the game
public class MenuManager : MonoBehaviour
{
    // start menu page
    public GameObject startPage;

    // game over page
    public GameOver endpage;

    // countdown UI
    public CountdownUI countdown;

    // timer UI
    public TimerUI timer;

    //list of behaviors for runner and tagger
    public List<BehaviorParameters> runners;
    public List<BehaviorParameters> taggers;

    // list of UI markers for runner and taggers
    public List<GameObject> runnerMarkers;
    public List<GameObject> taggerMarkers;
    
    // list of all behaviors and markers for agents 
    public List<BehaviorParameters> agents;
    public List<GameObject> markers;

    // player user can play as 
    private string player;

    // deterimines if tagger won
    private Boolean taggerWon;

    // maps each runner/tagger/agent to corresponding marker
    private Dictionary<BehaviorParameters, GameObject> runnerToMarker;
    private Dictionary<BehaviorParameters, GameObject> taggerToMarker;
    private Dictionary<BehaviorParameters, GameObject> agentToMarker;

    // initializes lists and dictionaries before game start
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
        // hide game over page
        endpage.SetActive(false);

        // display start menu
        startPage.SetActive(true);

        // disable timer 
        timer.OnDisable();
        
        // pauses game
        Time.timeScale = 0f;

        // makes sure agents aren't running 
        ToggleAgents(false);
    }

    //Controls Play as Tagger button
    public void PlayAsTagger()
    {
        // record user chose tagger 
        player = "Tagger";

        // start game
        StartGame();
    }

    //Controls Play as Runner button
    public void PlayAsRunner(int runner)
    {
        // record user chose either runner or other runner
        switch (runner)
        {
            case 1: 
                player = "Runner";
                break;
            case 2: 
                player = "Runner2";
                break;
        }

        // start game
        StartGame();
    }

    //Method to control the Restart Button
    public void RestartGame()
    {
        // reset which side the player is on
        player = "";

        // reset win state 
        taggerWon = false;

        // start game
        Start();
    }

    //Starts up the game (including countdown)
    public void StartGame()
    {
        // hide the start menu
        startPage.SetActive(false);

        // hide game over UI
        endpage.SetActive(false);

        // start the countdown
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
        // gets total num of runners
        int total = runners.Count;

        // for each runner 
        foreach (var bp in runners)
        {
            // get the RunAwayAgent script from runner
            var agent = bp.GetComponent<RunAwayAgent>();

            // if agent is frozen
            if (agent != null && agent.frozen)
            {
                // decrease total num of unfrozen runners 
                total--;
            }
        }
        // tagger wins if total number of runners unfrozen is 0
        bool won = total == 0;

        // tagger wins
        taggerWon = won;

        // return tagger wins
        return won;
    }

    //Toggles the agents (and their markers) on or off
    private void ToggleAgents(Boolean enable)
    {
        // if agent enabled 
        if(enable)
        {
            // for every agent 
            foreach(var agent in agentToMarker)
            {
                agent.Key.gameObject.SetActive(true);

                // if agent tag matches with player type either a runner tagger, etc 
                if (agent.Key.CompareTag(player))
                {
                    // change so player is in control of the behavior 
                    agent.Key.BehaviorType = BehaviorType.HeuristicOnly;

                    // show marker for the player 
                    agent.Value.SetActive(true);
                } 
                // otherwise it is controlled by AI and uses trained model
                else
                {
                    agent.Key.BehaviorType = BehaviorType.InferenceOnly;
                }
            }
            
        } 
        // otherwise if not enabled 
        else
        {
            // for all agents 
            foreach (var item in agentToMarker) 
            {
                // set to inactive 
                item.Key.gameObject.SetActive(false);

                // disable its designated marker
                item.Value.SetActive(false);
            }
        }
    }
}
