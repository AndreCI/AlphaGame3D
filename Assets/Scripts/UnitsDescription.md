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
when a units dies, do... (create unit)

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

Units classement: cost, requierment, schoolom
it should be possible to:
	get powerful army stand alone (high stats, easy synergies) with or without schoolom (Allow playstyle mono units/units+building or magic)
	get low power/utility units at the start of the game (early unit, scout)
	get decent units with schoolom (utility, etc.)
it should not be possible:
	switch to powerful units easily (tech choice should matter: building requirement for non magic units should not be the same for magic units)

starts:
	mono unit: few building to unlock tech tree, then build high cost units. Should it be a viable long game strats? Or is it rush?
	unit + building: allow for more units, thanks to food. Maybe tech tree for monoU and BuildU can be shared?
		This create a spectrum between monoU and BuildU, as the more building you make, the more units you can make. This should be more powerful but
		slower than monoU, implying that monoU is a more rush strats
	unit + magic: G: low and mid tier units which follow G logic(heal, value oriented)
				  R: low and mid tier units which follow R logic(damage, rush, aoe, self harm)
				  B: mid and high tier units which follow B logic(mana, etc.)
				  V: high tier units which follow V logic (combo, etc.)

Baseline: tier is defined by tech & schoolom requierments
	(B)base tier: ~30 stats
	(L)low tier: ~45 stats ->+15
	(M)mid tier: ~65 stats ->+20
	(H)high tier: ~90 stats ->+25
	(G)god tier: ~120 stats ->+30
base to god ratio: 1/4...
See HS: 1mana=3-4stats, 2mana=5-6stats, 3mana=7-8stats, etc. 1mana to 10 mana ratio: 4/24=1/6

Magic Center units(attack/life/movement/vision)
B	MCT1: ?
L	MCT2: ?
G units (requirement/attack/life/movement/vision)
L	MT1: 1/4/20/3/2 : heal all friendly units in a radius of Y for X at the start of the turn
M	MT1: 2/25/40/3/1: gain gold for each kill
H	MT2: 2/45/35/4/3: when a friendly units dies in a radius of Y, gain 1 action point
G	MT2: 3/20/100/3/3: lifesteal, can't counterattack?
H	Invoc: 3/
B units
L	MT1: 1/10/25/3/2: ranged(2), each time a spell is cast, gain 1 mana
M	MT1: 2/25/40/3/3: freeze enemy on hit
G	MT2: 3/40/50/2/5: at the start of your turn, reduce the cooldown of your last spell by 1
G	MT2: 3/35/80/3/3: ranged(3), when this kill an enemy, freeze adjacent units
H	Invoc: 3/
R units:
L	MT1: 1/20/20/4/2: each time this takes damage, gain 2 armor (for 5 turns?)
L	MT1: 1/10/20/3/3: ranged(2), gain 5 attack on each kill
H	MT2: 2/40/40/4/4: 
G	MT2: 3/
H	Invoc: 3/
V units:
H	VT1:
G+?	VT2:
H/G Invoc: ?
Barracks Units (attack, life, mouvement, vision)
B	BT1: 10/20/3/3: deal X additional damage while reposting
B	BT1: 10/10/3/2: ranged(2)
L	BT2: 15/30/3/3: gain 5 armor until next turn if it didn't mov
L	BT2: 
Stables units:
M	ST1: 20/30/5/3: gain 5 attack until end of turn for each mvt point spent
H	ST2: 40/50/5/2: flight (at the end of your turn, starts to fly: unit can go over water and move through cliff, has increased vision. On hit or attack, it goes back
							the ground if it can)?
Castle units:
H	CT1: 30/60/3/3: 5 armor. On kill, gain 1 movement point (can attack again)
H	CT1:
G	CT2: 40/80/3/3: 
G	CT2:
HallCenter units:
B	HT1: 5/10/2/3: At the end of your turn, if it didn't move, gain 1 vision until this loses a mouvement point to a maximum of 5. 
TOTAL: 28 + (6?) unique units

Building requierment:
Hallcenter -> Barracks -> Castle
					   -> Stables
		   -> Monster pit -> Void portal
		   -> Magic Center
between 14 and 28 unique units
BT1: low melee unit & range unit (basic, simple to understand and not powerful improvements)
BT2: mid melee & range (powerful for mid tier, complex improvements)
CT1: mid melee (& range?) less powerful than BT2, more stand alone, flavorful
CT2: high melee (& range?) powerful, stand alone, cost a lot
ST1: mid melee unit with a lot of movements, cavalary. Maybe also mid flying unit (range?). simple improvements
ST2: high cavalary & maybe flying, complex improvements, stand alone
MP1 - R: low and mid unit?:  support swarm? and big strats: 1 range unit with aoe buff/attack? 1 powerful unit (self heal? Combo with fire) 
MP1 - G: low and mid unit?: support swarm and bring value: single heal, value unit (gain gold/food/action point)
MP1 - B: low and mid unit?: support mana & control: bring mana, freeze enemies, make them consume more food?
MP2 - R: mid and high unit?: 
MP2 - G:
MP2 - B:
VP1:
VP2:

Unit ideas (health, life, mouvement, vision):
soldier : 10/20/3/3 - deal additional damage when riposte?
archer : 10/10/3/2 - range 2
skeleton : 10/25/3/3 - ?
mage : 10/10/3/2 - range 2, summon skeleton on kill
raider : 12/15/5/3 - gain attack for each mouvementpoints spent until end of turn?
sorceress
catapulte
orcwolf
pikeman
brute
Karate
Ninja
2Handed
Knight
Hammer
spearman
swordsman
...
