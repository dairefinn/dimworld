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
2. Equip a sword
3. Patrol the house (Requires the sword to be equipped)

The agent will generate a plan to achieve these goals using the GOAP system. This results in some interesting emergent behaviour. For example, if the agent is trying to patrol the house they will come up with a plan that includes equipping the sword first.

## Plans

### Sensory system for agents

Currently the agent's state is only affected by the actions they perform. I want them to be able to "observe" the properties of the world around. For example, if they spot the player, they should be able to store that in their memory/state which will affect their future plans generated using the GOAP system.
