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
	Add types to subjects so that elements can handle multiples subjects.
Add unit ability framework
add AI difficulty	
	Fix effect and death mechanics
	Rework unit movement completly by removing root motion
Implement units keywords
	Implement flight
	Go In Hexgrid see https://catlikecoding.com/unity/tutorials/hex-map/part-27/
	Rework BFS

UI:
	Implement help ressource bar
	Implement help selection bar
	Implement card tutorial (start of game: would you like to tuto + menuhelp)
	Implement help card selection
	Implement menu
	Implement effect help (show which effects are active + little panel to display consequences of the effects when hover over card)
	Implement T2 help
	Change fontsize of carddisplay (spell overflow)
	Implement camera automove (double click centers it, etc)
Implement UI controler: click on barracks unlock barracks unit tab, etc.
	Rework the UI
	Add notifications (unit died, need more food, building constructed)? see tuto quill18
Think about artstyle
Modify building display if in construction


Graphic:
	Implement visual effect on spell effect
	Implement unit diffrence from player
	Implement start of turn effects + start of turn pop up
	Implement starvation effect
	Hide enemy notification
Do card unit with ability
Do card unit with requirement schoolom
Implement TechTreeUI
	Improve terrain
	Change models in order to add more units
	Balistic projectiles
	On Target Attack Range
hexgrid:
	add attack
	add range attack
	and spells
	debug visibility
Debug visibility (death effect, change visibility during oppo turn)
	Debug AI
Debug range attack
add a way to check if game is unplayable
think about a way to place building
remove ability to place building/unit at wrong places
Rework building/unit placement
Implement damage source type (food, attack, riposte, spell, effect...)
	center camera on hall center at start of game	

Balance:
Rebalance game
?Construction Time?
	Implement riposte

	
Content:
	Start a global reflexion before implementing anything
	Implement barrack T2 unit
	Continue the global reflexion: Units & buildings.
Finish the global reflexion: Units & buildings.
Implement more building & units
	Implement terrain effect
	Implement more frost spells
	Implement more fire spells
	Implement more basic spells
	Implement more spell effects
	Implement building restr(max 1 building)
	Implement green magic
	Implement more green spells
	Implement range attack indé from mvt
?Implement void spells
	Implement scouts (hallcenterbuilding)
Implement better AI (more units, spells?)
Start a reflexion on play during enemy turn/interaction

Audio:
	Find audio track
Add audio track
Add spell sounds
Add untis sounds
Add barracks sounds
Add Menu & button sounds

AI
Rework range attack
	Rework attack priority
Rework path finding

Debug:
Fix building out of places
	Add animations attack
Found a bug on double click for a spell: 
NullReferenceException: Object reference not set to an instance of an object
ConstructionManager.ConstructSpell (.Node node) (at Assets/Scripts/GameEngine/ConstructionManager.cs:108)
Node.Construct (Boolean makeUnwalkable) (at Assets/Scripts/World/Node.cs:180)
Node.OnMouseDown () (at Assets/Scripts/World/Node.cs:157)
UnityEngine.SendMouseEvents:DoSendMouseEvents(Int32)

Optimization:
Implement batching
Look update methods
Caching components
Balance
Refactor code (remove prefabs from other places than buttons?
debug
user test
implement day/night for time constraint