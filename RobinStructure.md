Game Structure

[TOC]

​![1542558639206](C:\Users\antoi\AppData\Roaming\Typora\typora-user-images\1542558639206.png) This is the whole game in a glance. Separated in 3 layers. Transition, Game, UI.



-----

## Overlay

![1542554916341](C:\Users\antoi\AppData\Roaming\Typora\typora-user-images\1542554916341.png)

The overlay node is responsible for the Screen transition animation. There is a ColorRect node that contains the Image for the transition. There is also a AnimationPlayer that Animate the progression of the transition. The script Attached on the Overlay only Calls "Play()" on the animation player. The only purpose of the script is to be called from GameManager.cs see Spawn method.

## GameManager, also can be called World

![1542555069980](C:\Users\antoi\AppData\Roaming\Typora\typora-user-images\1542555069980.png)

The GameManger is child of a viewport node. The viewport node can be ignored because its only purpose is to Scale the game up from low res to 1080p.

The GameManager is a synonym of world be because it is the Node that contains all of the levels and the player. All of the levels must be node2d that have the Group "level". The Player is also located under this node and must have the group "Player". 

This node is the most important one because it handles: Level Transition, Spawning, Transition,Settings, etc.

### The script 

GameController.cs has 4 main variables. The player, The current Level, The Starting level and the DebugGrid toggle. The Starting level can be directly changed in the godot inspector like so:

![1542555467791](C:\Users\antoi\AppData\Roaming\Typora\typora-user-images\1542555467791.png)

UpdateRoom() : This method is called every frame and it determine which level is the player located in. It uses basic math to tell if the player is inside the rectangle of a level. It changes CurrentRoom.

MoveCamToRoom(pRoom): This is called when the player Changes level. it Smoothly move the camera from the old level to *pRoom*. It uses a Tween Node. A Tween node Smoothly change a property over time, in this case it changes the camera Limits values to be contained inside a single room.

Spawn(WithAnimation): This is called when a player Spawn, It uses the same concept as MoveCamToRoom but instead of changing the camera limits, it changes the Player Position and moves it to the closest spawn.	

ChangeRoom(): This is only useful when you want to Force the game to change room. Pretty much only used with the TP command. see Console.cs for commands.

----

## Levels

Levels are ALWAYS child of the GameManager Node a.k.a World. For Levels to be considered as levels by the game manager, they must have the group "level" applied to them. When Selecting your level, in the inspector there is a "Node" tab, then a Group option and see if your level has the "level" group. You can also click this icon: ![1542556138775](C:\Users\antoi\AppData\Roaming\Typora\typora-user-images\1542556138775.png)

![1542556091066](C:\Users\antoi\AppData\Roaming\Typora\typora-user-images\1542556091066.png)

A level has 5 Layers(for now) and 1 Node2D called objects.All of the levels information are contained inside those 5 layers. Each layer is a TileMap node. TileMap nodes are Nodes that you can Draw Tiles on. Here are all of the layers.

![1542556241050](C:\Users\antoi\AppData\Roaming\Typora\typora-user-images\1542556241050.png)

bg_tile: This is the Background Tile Layer. Its basicly Background that you can paint. It is non-solid. Player cant interact with this.

bg_decals: This is also a background layer, but this is for decals(non-tile), Example: Vines, rocks, Lantern.

fg_tile: ForeGround Tile is the Layer the player can walk on. It is solid and uses Tiles. This is the main layer.

entities: entities are Dynamic object like spikes, Jump pads, Spawns, Key, Door. Since A Tilemap can only place non-dynamic object. This layer is only for placeholder. Meaning that when the game is loaded. Each tile on this layer will be replaced by the REAL dynamic object through script.

fg_decals: This is for foreground decals.

### Script

This is probably the most intimidating script in the game apart from the player script. The main function of this script is to place entities when the level is loaded, Choose the closest spawn, and Set the Levels properties like the size of the level and its position in the world.

Main variables:

```
LevelSize: This is the levelSize for the camera. Because of resolution issues 1 level is not Screen perfect. Levels are always half a tile longer than the screen size(16:9). Because of that, we need to substract half a tile from the Y of the size.

LevelRect: True Size of the level before the substraction.

LevelPosition: This is just the GlobalPosition of the Level inside the World.

SpawnPositon: This is the spawn the player is going to if he dies. AKA Current spawn.

Spawns: List of all the Spawns inside that level.
```

Methods :

LoadEntities(): This method is the most important one, like I said earlier, The entities layer is only placeholder for the real Entities. This method Scan each tile in that layer and then Place the entity with the real one. 

PlaceEntity(pPosition, pName, pEntity): This method place one entity at pPosition, renames it pName, and is of type pEntity. pEntity is the path to the scene of the entity. It also rotate the entity if the tile was rotated. If the entity is a Spawn. It adds it to the list of possible spawns. When is it done placing a spawn. It calls ChooseSpawn(). Also, All entities are placed under the "objects" node.

ChooseSpawn(): When all of the spawns are placed. this methods Loops through each possible spawn and choose the closest one to the player and set it as the current spawn.

ResetSpawn(): When leaving the level. all spawn must be turn off.

----

## Player OOF

The player is a KinematicBody2D because this type of node is used for moving bodies. KinematicBody2D have the MoveAndSlide function built-in which is used to move the player. The player has 3 States: Ground, Wall and Air. The most important part to know about the player is the main loop.

![1542557624647](C:\Users\antoi\AppData\Roaming\Typora\typora-user-images\1542557624647.png)

This is called every frame.

```
ItGetInput: Decide what is the user pressing(left, right, up, down) then it chooses a InputDirection.

UpdateVelocity: After Deciding what is the direction of the player. it adds speeds in the good Direction to the player.

UpdateState: Checks if the player is either on the Ground, on a Wall or in the Air. This is useful to decide what can the player do.

CanWallJump(): After seeing what is the player state. With condition we can check if the player has the ability to walljump.

SpeedLimits(): This is called to bring down the speed of the player If he is going faster than MAX_SPEED.

MoveAndSlide: Built-in method to move a KinematicBody2D with a specified Velocity.

ApplyGravity(): Apply gravity only if the player is not on the ground.

GetArrow(): This checks everyframe if there is an arrow in the world.
```

If you want more to know more about the player, you can look into Player.cs, most of it is commented and with a little bit of time you can understand the code without problem. Even tho it is a big script, it is well structured and is commented. You can always ask me and I'll explain it to you.

### Weapon

This node could be merged inside the Player.cs Directly but I prefered it to be separeted from the player to keep things organized. All it does is Spawn an arrow.tscn in the world when the Fire button is pressed. Thats all it does.

### Arrow

When an arrow is spawned into the world. It checks if a controller is used, if not then it follows the mouseposition. IMPORTANT: Since the game is pixelated, the actual resolution of the game is really small. Mouse position is calculated from the window resolution. To obtain the correct mouse position, you must Divide the mouse position by its scaling. Dont bother trying to understand the algorithm I got lost too.

----

## Entities

Most entities are small script that calls function on the player directly. the most complex one are the Key and Door.

## UI

For now there is only a Console and a Dialog box concering UI. The console is pretty simple, it uses basic switch for each command possible and most of them just toggle a boolean on the world node.

The dialog box is just a textBox that we slowly change visible_character using a TweenNode(again) for the animation.