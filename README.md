# Dimworld

This is mainly a testbed for me to implement technologies I find interesting. Originally, it was going to be similar to Rimworld (as evidenced by the name) but I figured it'd be more interesting to build a big immersive sim world and figure out what the game actually is once it becomes fun to play.

It's created in Godot using C#.


## Folder structure

- `Assets/` - Base assets like textures and fonts
- `Core/` - Core game logic and systems
- `Game/` - Game-specific scenes and resources


## Features

### Agents

Agents are the characters in the game. Functionally, there is no difference between a player-controlled agent and an AI-controlled agent - they can interact with the world in the same ways. The only difference is where they receive inputs from.


### Godot pathfinding

Agents can move around the world using the built-in pathfinding system in Godot. Nothing fancy here.


### (GOAP) Goal Oriented Action Planning

This is how the NPC agents are controlled and make decisions. It's a system where agents have a set of goals they want to achieve and a set of actions they can take to achieve those goals. The agent will then generate a plan to achieve those goals. This gives the illusion of intelligent decision-making.

I was inspired to implement this given the recent closure of Monolith, creators of FEAR. They were the first to utilize this system in a video game so there has been a bunch of discussion online about the system itself. The creator has written [a really interesting paper about it](https://www.gamedevs.org/uploads/three-states-plan-ai-of-fear.pdf) which I would recommend reading.

I wanted to implement something similar from scratch as I've already implemented the traditional approach to AI using state machines in an older project.

Currently, the agent has two priorities:
1. Ensure visibility
2. Patrol

They can use the following actions to achieve these goals:
1. Turn on the light
2. Turn off the light
3. Patrol the house (Requires the sword to be equipped)
4. Equip a sword (Requires the sword to be in the agent's detection radius)

The agent will generate a plan to achieve these goals using the GOAP system. This results in some interesting emergent behaviour. For example, if the agent is trying to patrol the house, they will come up with a plan that includes equipping the sword first.

I've made sure this all runs on a separate thread so that it doesn't impact performance too much. With it running on a separate thread, the planning system can be run at a higher frequency which makes the agent's decisions feel more responsive. This tickrate can be configured to be faster or slower depending on the performance of the machine it's running on.


#### Sensory system

I've also added a sensory system so the agent can "see" things in their surroundings and use this information to determine if actions can be taken. For example, the agent needs to be able to see a sword in order to equip it.

##### Memory system

The agent can "remember" information about the world around them. For example, if they have seen a chest or (if they have taken something from that chest before) they will remember the contents of that chest. This feeds into the planning system so the agent can use this information to determine if they need to take an action.


### Inventory system

Agents and chests have an inventory which can be interacted with.

Items can be moved between inventories. For example, if an agent needs a sword, they can take one from a chest nearby. Players can also take items from chests via the UI.

The player has access to a full inventory UI which allows them to manipulate their own inventory, access chests, or add items to their hotbar. Items can be drag-and-dropped between any two inventories. The player can also use the mouse wheel or the number keys to select a slot on their hotbar.

Some items can be equipped and some can be used from the hotbar. For example, the torch can be equipped via the hotbar or the context menu (right-click) which will cause a light to be created on the player.


### Modifiers

The modifiers system is used to augment different parts of the game. These could affect basically anything in the game so it's a very flexible system. For example, the agent can have a modifier which multiplies their speed or health. The player's sprint action uses this system too.


### Effects

The effects system is used to create areas which cause effects to happen to any valid node that enters them. For example, an effect might cause damage or apply a modifier to a node that enters it. This can be used to create things like traps, hazards, weapons, or even buffs.

These can be stacked too - you could add an explosion which causes damage and knocks back any nodes inside of it.


### Speech

Agents can speak, which will cause a speech bubble to appear above their head. For now, these are created while an agent is taking an action but they can be created at any time. Because the player is the exact same as the NPC characters, they could also speak but I haven't added any way to do this yet.


### Developer tools

I've added a developer console which can be opened by pressing the tilde key. I've replaced most calls to `GD.Print` with calls to `DeveloperConsole.Print` so that it is redirected to the console instead. In case of runtime errors or crashes, the console inputs can be printed to the main editor output too if required.

The console has full BBCode support so you can use it to print out formatted text, which makes it much easier to read than the editor output.

The console supports commands too, which allow me to examine or manipulate the game world from the console. For example, I can use the `teleport` command to teleport the player to a specific location in the world. This is useful for debugging and testing.


### Level transitions

The levels in the game can be loaded and unloaded at any time. This is done using a node in the main scene, which is always active, to swap out the currently active level.

This also uses level transition areas and spawn points to create areas that trigger the level transition. For example, if the player enters a level transition area, the new level will be loaded and they will be teleported to the relevant spawn point in the new level.


### Weapons

Using the Effects and Inventory systems, I've added a sword and a revolver. These can be used by left clicking with them selected in the hotbar. The sword will damage any character caught in it's area of effect and knock them back a short distance. The revolver will fire a projectile which will damage any character it hits.


### Factions and faction relations

Any node can belong to a faction: characters, objects, items, etc. Factions have an affinity towards each other which can be positive or negative. This allows for a dynamic relationship between factions. This is used to determine if an agent will be friendly towards another or if they can access an item/object owned by another faction.


## Planned features

### Communication actions

I want to add an action which will relay information from one agent to another. This will allow agents to share parts of their state with another. For example, if one agent sees a sword they can tell another agent about it. The memory system was a key prerequisite for this because it made sure the agents had their own version of the world state and didn't magically know information about the world around them.


### Group planning

The GOAP system gives an agent a goal and it figures out how to achieve that goal using its available actions. Because of this, it should be possible to have a planner which can control the goals a number of child agents have. For example, if a squad of agents want to attack a building, they could each be given separate goals which help this happen. One might keep watch from a distance, another might cover the back door, and another might go in through the front door.


### Vehicles

Will make traversal easier. Not sure how complex this will be yet. Multiple seats? Upgrades? Are they items or entities in the world? I need to think about this more.


### Building structures

I want the player to be able to place structures in the world. I'm not fully sure how to go about this yet. Do I use a grid system or just let them free place the structures? Should they be predefined sizes or should there be a dynamic element to it? I need to think about this more.


### Item ownership and inventory permissions

I want to add a system which allows items to be owned by a specific agent. This will allow characters to have their own inventories and not be able to access other characters' inventories. Once implemented, I can add the concept of stealing items from other characters. Fun fact: I had this idea when I accidentally introduced a bug where an agent trying to find a sword to patrol would see the player as a container because they implement the `IHasInventory` interface and chase them around to search them.


### Save/Load system

I want to be able to save the game to a file and load it back in. This will allow you to pick up where you left off.
