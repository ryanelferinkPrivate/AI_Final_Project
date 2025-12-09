using UnityEngine;
using System.Collections.Generic;

// manager for the freeze tag game 
// handles freezing, resetting and rewards for agents
public class FreezeTagManager : MonoBehaviour
{
    // list of all runners in the game 
    public List<RunAwayAgent> runners = new List<RunAwayAgent>();

    // tagger in the game
    public MoveToTarget tagger;

    // variable for ui of game
    [SerializeField] GameObject ui;

    // controls whether ui is showing
    [SerializeField] bool toggleUi;

    // toggles UI of game when unity game starts
    public void Awake()
    {
        if (ui != null)
        {
            ui.SetActive(toggleUi);
        }
    }

    // checks if all runners are frozen 
    public bool AllRunnersFrozen()
    {
        // for each runner in the game 
        foreach (var r in runners)
        {
            // if runner is not frozen
            if (!r.IsFrozen())
            {
                // not all runners frozen so return false
                return false;
            }
        }

        // otherwise all frozen return true
        return true;
    }

    // behavior for when tagger freezes runner 
    public void FreezeRunner(RunAwayAgent runner)
    {
        // runner enters frozen state 
        runner.Freeze();

        //UNCOMMENT FOR TRAINING
        // in training if UI is on stop here
        if (toggleUi) {
            return;
        }

        // otherwise if all runners frozen 
        if (AllRunnersFrozen())
        {
            // note that tagger wins
            Debug.Log("Tagger wins!");

            //tagger gets +100 reward
            tagger.AddReward(+100f);

            //episode is over and ends episode for tagger and every runner in game
            tagger.EndEpisode();
            foreach (var r in runners)
            {
                r.EndEpisode();
            }

        }
    }

    // behavior for when a runner gets unfrozen
    public void UnfreezeRunner(RunAwayAgent runner)
    {
        // runner leaves frozen state
        runner.Unfreeze();

        // notes that runner is unfrozen
        Debug.Log("Runner unfrozen -Manager");
    }

    // resets positions and states of runner and taggers 
    public void Reset()
    {
        // reset tagger
        tagger.ResetAgentState();

        // reset each runner 
        foreach (var r in runners)
        {
            r.ResetAgentState();
        }
    }

    
}
