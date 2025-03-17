
# Planning based AI

Based off [this paper](https://www.gamedevs.org/uploads/three-states-plan-ai-of-fear.pdf).

## Main components

- **Action**: An action is a single step that the agent can take to change the state of the world. Actions can be simple, like moving to a new location, or complex, like solving a puzzle.
- **Goal**: The goal is the desired state of the world that the agent is trying to achieve. The planner generates a plan that will take the agent from the current state of the world to the goal state.
- **Effect**: The effect of an action is the change that it makes to the world state. For example, moving to a new location will change the agent's location in the world state.

## Some other key concepts

- **Planner**: The planner is responsible for generating a plan based on the current state of the world and the desired goal. The plan is a sequence of actions that the agent must take to reach the goal.
- **World State**: The world state is a representation of the current state of the world. It includes information about the agent's location, the location of objects, and other relevant information.
- **Plan**: The plan is a sequence of actions that the agent must take to reach the goal. The planner generates the plan based on the current state of the world and the desired goal.
