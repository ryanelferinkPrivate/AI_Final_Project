using UnityEngine;
using System.Collections.Generic;

public class FreezeTagManager : MonoBehaviour
{
    public List<RunAwayAgent> runners = new List<RunAwayAgent>();
    public MoveToTarget tagger;

    [SerializeField] GameObject ui;
    [SerializeField] bool toggleUi;

    public void Awake()
    {
        if (ui != null)
        {
            ui.SetActive(toggleUi);
        }
    }

    public bool AllRunnersFrozen()
    {
        foreach (var r in runners)
        {
            if (!r.IsFrozen())
            {
                return false;
            }
        }
        return true;
    }

    public void FreezeRunner(RunAwayAgent runner)
    {
        runner.Freeze();

        //UNCOMMENT FOR TRAINING
        if (toggleUi) {
            return;
        }
        if (AllRunnersFrozen())
        {
            Debug.Log("Tagger wins!");
            tagger.AddReward(+100f);

            tagger.EndEpisode();
            foreach (var r in runners)
            {
                r.EndEpisode();
            }

        }
    }

    public void UnfreezeRunner(RunAwayAgent runner)
    {
        runner.Unfreeze();
        Debug.Log("Runner unfrozen -Manager");
    }

    public void Reset()
    {
        tagger.ResetAgentState();

        foreach (var r in runners)
        {
            r.ResetAgentState();
        }
    }

    
}
