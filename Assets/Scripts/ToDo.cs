

//Current bugs
//Update mana to calculate like health

//Turn order (ui items) - the Turn managers "int turn" is to reflect the entire length of units
///including dead ones. need a way to filter that or update it for the ui manager/HUD. 
///Should i refactor how it works altogether? Completely reset and sort turn order on death? and readd to it on revival??
///Do i need to store the entire turn order?

///Idea 1
///On Start create turn order and sort it by speed.
///On removal of unit, remove the unit from turn order, reset the UI items
///On add of unit, calculate where it should be
////from last to first, calculate if its speed is 10%, 20%, 30%... higher and place them in the respective position
////OR have specifed positions for them to be?


//Make unit a serializable class, with playable character and enemy being scriptable objects. Inside use battle unit class
//Rename BattleUnit to BattleController
//Move spell launch/recieve transforms to character mesh
//Update all parameters to _var
//Create battleUnit, spell, etc pool, and use the mesh from the Unit SO, also use animator controller override variable
//Setup a combat scene, with a battle trigger, battle scene, and a boss enemy
//Remove all traces of Soulless (soul well, ....) 
//Add music and sound fx
//Decide on story - Time Soldiers? 
//Update Main menu
//Update combat UI To be under 1 canvas
//Expand combat
//Screen or minimap effect when in enemy territory
//Create experience - Level design, enemy triggers , dialogue, quest, boss fight.
//Add all created code (teleporters, etc.)
//Select moves and all get executed at once
//Unit indicator changes based on character, class, etc.

//Revaluate Position