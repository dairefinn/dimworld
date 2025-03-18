# Dimworld

This is mainly a testbed for me to implement technologies I think are cool but the game itself will be something similar to Rimworld (As evidenced by the name).

It's created in Godot using C#.

## Technologies

### Godot pathfinding

Agents can move around the world using the built-in pathfinding system in Godot. Nothing fancy here.

### (GOAP) Goal Oriented Action Planning

With the recent closure of Monolith, creators of FEAR, there has been some discussion online about their revolutionary AI system. The creator has written [a really interesting paper about it](https://www.gamedevs.org/uploads/three-states-plan-ai-of-fear.pdf) which I would reccommend reading.

I wanted to implement something similar from scratch as I've already implemented the traditional approach to AI using state machines in an older project.

Currently the agent has two priorities:
1. Ensure visibility
2. Patrol

They can use the following actions to achieve these goals:
1. Turn on the light
2. Patrol the house (Requires the sword to be equipped)
3. Equip a sword (Requires the sword to be in the agent's detection radius)

The agent will generate a plan to achieve these goals using the GOAP system. This results in some interesting emergent behaviour. For example, if the agent is trying to patrol the house they will come up with a plan that includes equipping the sword first.

I've also added a basic sensory system to the agent can "see" things in their surroundings and use this information to determine if actions can be taken. For example, the agent needs to be able to see a sword in order to equip it.

## Plans

### Communication actions

I want to add an action which will relay information from one agent to another. This will allow agents to share parts of their state with another. For example, if one agent sees a sword they can tell another agent about it.
