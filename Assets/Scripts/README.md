#Other
Need to think about the general balance and ideas behind magic
-> try to assign magic tree to playstyle, then balance
give clear and coherent bondaries between schoolom
brainstorm ideas: 
	arcane=cheap, quick, non effective
	fire=aoe, self damage, aggression, buffs
	frost=control, mana gain, economic disruption (food removal?)
	green=value, economic power (food & gold?), heal
	void=powerful, costly, high cooldown, combos?

reflected in early game decision making: start military (barrack), magic (magic center) or economic (windmill)
each of these building, although necessary for mid/late game, should be key: each unlocks more technologie which follows the logic of the
first choice. Ex: barrack unlocks another building for different units and maybe some special building. Magic unlocks shrines and thus 
the schools of magic. Windmill should unlock similar buildings in a similar idea. maybe some special units which generate ressources? this 
is hard to balance as this could go exponentially. Maybe some improvements? Should be able to generate action points to as it is link to 
green magic. Some healers as well to follow the same idea. Maybe Windmill unlocks the ability to T2? BTW, shrines should probably not requires
HC T2
From this comes multiples playstyles, as all "main strategie" can be mixed with one another with various intensity. All in in terms of units
means full aggro, splash it with windmill and you have a midrange deck
The magic system integrates itself with that as each school of normal magic (ie not void) helps you accomplish your goal. Red for units, green
for dev, blue for magic.
At this points, one could try to link aggro with red/units, combo with blue/magic, control for green/economic. Althought it is inspired by
that, maybe it can go beyond that.

Some gameplay elements to take into consideration are the notion of tempo and player state. Player state is what the a player has access to,
ie its buildings, its economie, its school of magic levels and its cooldown. Tempo is what a player has on the board, and how much.
Going barracks>full units grants tempo, but a terrible player state where all the actions points are gone and no tech is devellopped. 
Going windmill or magicc gives no tempo but improves one player state. This is heavly link to the notion of aggro/combo/control. However,
in rts, the notions of aggro/control/combo are less important? (boarf) as a "control" (ie economic player) can launch counter offensives and
the aggro can transition without having an opponent notice. In card games, this is rarely possibles, as a deck is prefixed and thus already
prone to a certain gamestyle. This apply to this game as well, but as there is some basic buildings, a transition is possible in any playstyle,
although it would be less powerful than a player building its game on it.

Anyway. The point is that starting a path should not lock you from the others and that a control style with units should be possible.
	
#Implementation ideas
Implement unit mana & spells?
Implement Void magic, a more powerful magic (more costly, control/cheese strat), not supposed to be a started magic
Implement riposte?


#TODOLIST

Basic:
	Implement Enemy spawn
	Implement basic enemy AI
	Implement spell effect
	implement end of mvtpts after an attack
	Implement tier2
	Implement shrines 
	Implement help UI when user hover
	Improve Card display UI
	
Engine:
	Change way requirements are handled

UI:
	Implement help ressource bar
	Implement help selection bar
	Implement card tutorial (start of game: would you like to tuto + menuhelp)
	Implement help card selection
	Implement menu
	Implement effect help (show which effects are active + little panel to display consequences of the effects when hover over card)
	Implement T2 help
	
Graphic:
	Implement visual effect on spell effect
Implement unit diffrence from player
Implement start of turn effects
	
Content:
	Implement barrack T2 unit
Construction Time?
Implement more building
Implement more frost spells
Implement more fire spells
Implement more basic spells
Implement more spell effects
	Implement building restr(max 1 building)
	Implement green magic
Implement more green spells
Implement scouts (hallcenterbuilding)
Implement better AI

Optimization:
Implement batching
Look update methods
Caching components

Balance
Refactor code (remove prefabs from other places than buttons)
debug
user test
implement day/night for time constraint