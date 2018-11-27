Units: the idea is to have a gradation in units power as the game goes, along as a more versitality options. Units are supposed to be various, with unique
traits and have some synergies (with others units but also with spells).
The gradation comes from the tech tree, as the first building (barracks) unlock the basic units. Then, multiple building can be purchased to unlock specific
units. Using the t2 system, the goal is to make it difficult (and non efficient) to unlock every units.
Unique traits can not be achieved by mere stats only, which enforces the idea that giving ability to units is good. A range of weyworks allow to complexify
the units in general without adding to much reading difficulty (armor, regen, etc.)
Finally, linking the units to the spell system schoolom could allow to 1° make the tech tree more diverse, as each schoolom could have its own recruitement
building, 2° link more heavly a player to its schoolom commitment, and thus making the game more diverse. Design wise, it good as well (more commit to a color=
playstyle more coherent and with more "personnality")


Keywords:
Ranged(X): if this units has more or equals movement points than X, it can attack a unit at a max distance of X.
Armor(X): prevents X damages
?Riposte: this units deals damage back when being attacked
Non living: this units does not consume food
Etheral: this units consume mana
deathrattle, battlecry

Abilities:
Manabind: This units attack is equals to 2 times your mana
Aggressive: +X attack when attacking
Spells cost less, spell have less cooldown, more damage, zone buff (attack/regen/armor?)
gain mvt points/vision range if it hasn't attack last turn
when a units dies, do...

Unit ability, effect, notif reflexion

Unit should be defined at their core by:
	name
	health
	armor
	attack
	mouvementpoints
	vision
	List<Ability>
	List<Status>

Status right now are called Effects (bad), and are defined by a type, a duration and (an amplitude).
Status should probably not always have a duration, and should be the only possible modifier of a units attribute such as attack, etc.
Status can either modifiy the units, or modify units on trigger, or apply status on trigger: +3attack, deal 5 damage when attacking, gain 2 regen at the start of turn?

Abilities should be able to apply status, but not always is an ability a status???
Abilities and effects starts to pile up with notifs. How to articulate it in a meanigful and efficient way?
1)What do we want?
->Triggered abilities, such as start of turn, etc. but also activable, which are a trigger
->Aura abilities, such as cost reduction, unit around
->Diverse effects: gain gold, gain buff, but also heal nearby units, apply debuff to target(s)...
->Unit status: burn, regen, temp attack buffed

