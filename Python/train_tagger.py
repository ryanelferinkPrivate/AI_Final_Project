from mlagents_envs.environment import UnityEnvironment, ActionTuple
import numpy as np
import torch

class PolicyNet(torch.nn.Module):
    def __init__(self, obs_dim, act_dim):
        super().__init__()

        self.fc1 = torch.nn.Linear(obs_dim, 128)
        self.fc2 = torch.nn.Linear(128, 128)
        self.mu_head = torch.nn.Linear(128, act_dim)
        self.log_std = torch.nn.Parameter(torch.zeros(act_dim))

    def forward(self, x):
        x = torch.relu(self.fc1(x))
        x = torch.relu(self.fc2(x))
        mu = self.mu_head(x)
        std = torch.exp(self.log_std)
        return mu, std



def compute_returns(rewards, gamma=0.99):
    G = 0.0
    out = []

    for r in reversed(rewards):
        G = r + gamma * G
        out.insert(0, G)

    returns = torch.tensor(out)
    mean = returns.mean()
    std = returns.std()
    returns = (returns - mean) / (std + 1e-8)
    return returns


def main():
    env = UnityEnvironment(file_name=None)
    env.reset()

    behavior_name = list(env.behavior_specs.keys())[0]
    spec = env.behavior_specs[behavior_name]

    obs_shape = spec.observation_shapes[0]
    obs_dim = int(np.prod(obs_shape))

    act_dim = spec.action_spec.continuous_size

    print("obs_dim:", obs_dim)
    print("act_dim:", act_dim)

    policy = PolicyNet(obs_dim, act_dim)
    optimizer = torch.optim.Adam(policy.parameters(), lr=3e-4)

    num_episodes = 1000

    for ep in range(num_episodes):
        env.reset()
        done = False

        episode_rewards = []
        log_probs = []

        while not done:
            decision_steps, terminal_steps = env.get_steps(behavior_name)
            if len(terminal_steps) > 0:
                step = terminal_steps
                done = True
            else:
                step = decision_steps

            # get observation and convert to torch tensor
            obs = step.obs[0][0]
            obs_t = torch.tensor(obs, dtype=torch.float32).unsqueeze(0)

            # forward pass through policy
            mu, std = policy(obs_t)
            dist = torch.distributions.Normal(mu, std)

            # sample action and compute log prob
            action_t = dist.sample()
            log_prob_t = dist.log_prob(action_t).sum(-1)

            # convert action to numpy for Unity
            action_clamped = torch.clamp(action_t, -1.0, 1.0)
            action_np = action_clamped.squeeze(0).detach().numpy()

            # send action to unity env
            action_tuple = ActionTuple(continuous=action_np[None, :])
            env.set_actions(behavior_name, action_tuple)
            env.step()

            # get reward 
            reward = step.reward[0]

            episode_rewards.append(reward)
            log_probs.append(log_prob_t.squeeze(0))

        returns = compute_returns(episode_rewards, gamma=0.99)

        loss = 0.0
        for G, logp in zip(returns, log_probs):
            loss = loss - logp * G

        optimizer.zero_grad()
        loss.backward()
        optimizer.step()

        print(
            "Episode",
            ep,
            " ", "total reward:",
            float(sum(episode_rewards))
        )

    env.close()


if __name__ == "__main__":
    main()
