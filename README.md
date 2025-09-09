# Infinite Wumpus

A dungeon crawler aspiring to be a roguelike, "Infinite Wumpus" takes inspiration from [the classic text-based adventure game "Hunt the Wumpus"](https://en.wikipedia.org/wiki/Hunt_the_Wumpus).

The player explores a [procedurally generated](https://en.wikipedia.org/wiki/Procedural_generation) dungeon, where each level consists of 20 rooms, and each room connects to 3 other rooms, with the connections forming a dodecahedron.

Every level has one room that contains stairs going down to the level below — thus, the dungeon overall is a collection of concentric dodecahedrons.

Just as in the classic game, there are *pits* that the player can fall into, and *bats* that move the player to a random room on the same level. However, in "Infinite Wumpus", the player has *Hit Points*, and pits merely cause falling damage instead of always being immediately fatal.

And yes, there's a *Wumpus*, but the player can avoid its attack by collecting *donuts* to offer to it.

Also, there are various *treasures* to find and collect!

The player can *rest* to restore a bit of health, but if they do, the bats and Wumpus will move to new rooms.

## Implemented Features

- Levels and rooms procedurally generated "as needed"
- Travel between rooms and levels
- Collectable items
  - Donuts
  - Treasures
- Hazards
  - Pits
  - Bats
  - Wumpus
- Resting to regain HP (also moves bats and wumpus)

## Planned Features

### Pets

I'd like to add *pets* to the game, such that if you travel down past a certain level, you might encounter an animal that follows you and provides a little bonus effect:
- Dog - barks near a Wumpus room, and scares Wumpus to prevent attack
- Cat - warns of nearby pit room(s)
  - e.g. "won't go near room A", or in case of A & B both having pits, "stays near room C"
- Owl - bats always flee instead of attacking (owls eat bats)
- Rat - seeks out donut room(s), and increases donut spawning on new levels while paired
- Bat - other bats don't attack, and perhaps, no pit fall damage
- Pig - oinks near treasure room(s)
- Fox - directs player to safe path to nearest stairs
- Hen - small chance to lay a golden egg (treasure) whenever entering an unvisited empty room

If the player were to fall down a pit while paired with a pet, most pets (except perhaps the bat) would not follow the player down, but instead move to a safe room on their level, so the player could return to that level to find it again. If the player and pet become separated by more than 10 levels, their *bond* breaks and a new pet will eventually spawn for the player to find.

Pets would also need to be fed donuts regularly to stay *bonded*. If a pet's needs are ignored for too long, it may wander away.

(All pet types are three-letter animal names by design.)

### Player Status Effects

A range of status effects that can be bestowed on the player, temporarily or permanently.

Buffs:
- Stout: Additional hit points
- Capacity: Carry more donuts (player starts out being able to carry only 1 donut at a time)
- Pit Map: Reveal pit locations when entering new levels
- Bat Map: Reveal bat locations when entering new levels
- Stealthy: Chance of being ignored by bats and Wumpus
- Nimble: Chance of taking no damage upon falling
- Scary 1: Bats flee instead of attacking
- Scary 2: Wumpus flees instead of attacking
- Tough: Lower chance of Bleeding and Poisoned (see below) and higher chance of curing them

Debuffs:
- Clumsy: Chance of taking *extra* damage upon falling
- Loathsome: Bats and Wumpus flee, *but* pets won't join or follow
- Bleeding: Chance to lose a hit point every (n) turns; chance to be cured by resting. Stacks with Poisoned.
- Poisoned: Chance to lose a hit point every (n) turns; chance to be cured by resting. Stacks with Bleeding.

### Mutually Exclusive Quests

To add challenge and tension, and to encourage dynamic pacing options, the player would occasionally be offered two quests to complete for corresponding rewards, but the tasks would be structured such that they'd have time/resources for only one at most.

For example, let's say the player reaches level 15 and is offered a pair of quests:
- "quest: reach level 50 within the next 100 turns; reward: Stout (add +5 to max HP)"
- "quest: explore every safe room in levels 16-20; reward: Pit Map (reveal pit locations when entering new levels)"

The player could attempt to race down to level 50 (perhaps by tactically seeking out pits to fall down, and strategically resting/healing), *or* they could carefully explore levels 16-20, but with 35 levels between 15 and 50, and 20 rooms per level, it's unlikely that they would be able to accomplish both.

### Themed Descriptions

I'd like to have the early levels be described as lighter and cleaner, and as the player descends, lower levels appear darker and danker.
Eventually, past a certain depth, the level descriptions could rotate through a variety of other fanciful and evocative themes, like "fungal blooms" or "humming purple crystals".

