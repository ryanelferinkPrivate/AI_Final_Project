# Freeze Tag AI — Unity ML-Agents Final Project

## 1. Project Overview
This project implements a multi-agent **Freeze Tag** environment in Unity using **Unity ML-Agents** and PPO reinforcement learning. The environment includes **one Tagger** and **two Runners** with asymmetric goals:

- **Tagger:** freeze all Runners as quickly as possible  
- **Runners:** avoid being tagged and unfreeze frozen teammates

We used **curriculum learning** to gradually introduce complex behavior. Through training, agents developed meaningful strategies including chasing, evasion, teamwork, and guarding.

## 2. Team Members
- *Ayoub Sougrati*
- *Ethan Anthony*
- *Kevin Wang*
- *Ryan Elferink*

## 3. Environment Setup

### Agents
- **1 Tagger**
- **2 Runners**

### Observations
- Positions of all agents on the board
- Frozen/unfrozen states of all runners

### Actions
- Move forward/backward/left/right

### Episode End Conditions
- Both runners are frozen
- Timer runs out

## 4. Reward Structure

### Tagger Rewards
| Event | Reward |
|-------|--------|
| Tag a runner | **+1** |
| Freeze all runners (win) | **+100** |
| Run into a wall | **-0.10** |

### Runner Rewards
| Event | Reward |
|-------|--------|
| Survive each step | **+0.005** |
| Get tagged | **-1** |
| Run into a wall | **-0.10** |
| Unfreeze a teammate | **+0.5** |

These rewards encourage the tagger to chase efficiently while teaching runners to survive, avoid mistakes, and work together.

## 5. Curriculum Learning

We trained agents in four stages to improve stability and learning efficiency:

1. **Tagger vs. Hardcoded Runner**  
   - Tagger learns basic chasing behavior.

2. **1 Runner vs. Trained Tagger**  
   - Runner learns basic evasion.

3. **2 Runners vs. Tagger (no unfreezing)**  
   - Runners learn multi-agent evasion.

4. **Full Freeze Tag Logic Enabled**  
   - Complete environment: tagging, freezing, unfreezing, cooperation.

This progressive curriculum allowed agents to build skills step-by-step without collapsing early.

## 6. Learned Behaviors

### Tagger Behaviors
- Aggressively chased the closest runner.
- Cornered runners against walls.
- Sometimes **guarded frozen runners**, preventing unfreezing (an emergent, strategic behavior).

### Runner Behaviors
- Learned to maintain distance and avoid the tagger.
- Moved away from walls after learning wall penalties.
- Unfroze teammates when safe.
- Displayed teamwork and evasive motion patterns.

## 7. ML-Agents Configuration

- **Framework:** Unity ML-Agents  
- **Algorithm:** PPO  
- **Training:** Curriculum Learning + PPO  
- **Behavior Parameters:**  
  - Vector observation size: *11*  
  - Action space: *Continuous*  

## 8. Results

- Tagger learned efficient, direct chase strategies.  
- Runners learned to evade, survive longer, and collaborate through unfreezing.  
- The tagger often developed a **camping/guarding strategy**, protecting frozen runners.  
- Curriculum training significantly improved stability and final performance.

(Insert TensorBoard screenshots or images here if available.)

## 9. How to Run

1. Clone the repository  
2. Install **Unity (version 6000.0.32f1)**  
3. Install MLAgents
Make sure you have it installed in the Package Manager: (Unity Project > Window > Package Manager > Unity Registry > Search and install ML Agents)

Complete the steps below in the project directory to install
```
conda create -n mlagents python=3.10.8
conda activate mlagents
pip install mlagents==0.30.0
pip install protobuf==3.20.3 --force-reinstall
pip install six
mlagents-learn --help
```
