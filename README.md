# Dimworld

This is mainly a testbed for me to implement technologies I think are cool but the game itself will be something similar to Rimworld (As evidenced by the name).

It's created in Godot using C#.

## Features

### Agents

Agents are the characters in the game. Functionally, there is no different between a player controlled agent and an AI controlled agent - they can interact with the world in the same ways. The only difference is where they receive inputs from.

### (GOAP) Goal Oriented Action Planning

This is how the NPC agents are controlled and make decisions. It's a system where agents have a set of goals they want to achieve and a set of actions they can take to achieve those goals. The agent will then generate a plan to achieve those goals. This gives the illusion of intelligent decision making.

I was inspired to implement this given the recent closure of Monolith, creators of FEAR. They were the first to utilize this system in a video game so there has been a bunch fo discussion online about the system itself. The creator has written [a really interesting paper about it](https://www.gamedevs.org/uploads/three-states-plan-ai-of-fear.pdf) which I would reccommend reading.

I wanted to implement something similar from scratch as I've already implemented the traditional approach to AI using state machines in an older project.

Currently the agent has two priorities:
1. Ensure visibility
2. Patrol

They can use the following actions to achieve these goals:
1. Turn on the light
2. Turn off the light
3. Patrol the house (Requires the sword to be equipped)
4. Equip a sword (Requires the sword to be in the agent's detection radius)

The agent will generate a plan to achieve these goals using the GOAP system. This results in some interesting emergent behaviour. For example, if the agent is trying to patrol the house they will come up with a plan that includes equipping the sword first.

I've also added a basic sensory system to the agent can "see" things in their surroundings and use this information to determine if actions can be taken. For example, the agent needs to be able to see a sword in order to equip it.

### Godot pathfinding

Agents can move around the world using the built-in pathfinding system in Godot. Nothing fancy here.

### Basic inventory system

Agents and chests have an inventory which can be interacted with.

Items can be moved between inventories. For example, if an agent needs a sword, they can take one from a chest nearby. Players can also take items from chests via the UI.

Players can equip items from their inventory too. Currently this is just limited to the torch which will create a light source centered around them when equipped.

## Plans

### Communication actions

I want to add an action which will relay information from one agent to another. This will allow agents to share parts of their state with another. For example, if one agent sees a sword they can tell another agent about it.

### Agent memory syste,

I want to add a memory system to the agents so they can remember things they have seen in the past. This will allow them to make more informed decisions and will also allow them to receive information from other agents which they might not know about otherwise. For example, if an agent wants to patrol but needs a sword to do so, they can ask another agent if they have seen a sword in the case that they have not seen one themselves.
