# Robin the game

[GAME DOCUMENTATION]: RobinStructure.md	"Game Doc."



## What is Robin:video_game:

> Robin is a platforming game about robinhood that finds a magic arrow that allows him to control it in the air. the progression of the game is room-based

---

## Game Structure :japanese_castle:

### Worlds :earth_americas:

> Worlds are the main chapters of the game. A world can be travelled from start-to-end without any loading screens

Worlds are made from multiple ***rooms*** chained together to create a path for the player to move accross. Worlds can be described as a collection of ***Rooms***.

### Rooms :door:

> Rooms are small zones that the player can enter and leave. They consists of a gridmap that containes tiles.

Here is what a room contains :

- A spawnpoint (position2D node)
- A minimum size of 40 tiles x 23 tiles
- A room has 4 layers:paperclips:
  - Background Layer(Background tiles)
  - Foreground Layer(Solid Tiles)
  - Entities Layer(Objects)
  - Foreground Decals(Foreground details)
  - Background Decals(Background details)

### Tiles:ballot_box:

>Tiles are 2D Images that a square. Robin uses tiles that are 8 by 8 pixels. Normally all tiles are regroupped into a single .png that holds all of the tiles for a specific world. These are called tilesets.

----

## The Player :runner:

You are playing as Robin, Robin is a little boy who found a magic arrow that allows him to control it. Soon into the game, the player realizes that he can use that arrow as a platform.

### Stats

+ Height: 16 pixels
+ Jump Height: 32 pixels
+ SuperJmp Height: 48 pixels
+ Normal jump distance = 8 tiles
+ Super jump distance = 9 tiles

### Movement  :arrows_counterclockwise:

The player can:

+ Move sideways

+ Normal jumping

+ Crouch jumping (A.K.A Super jumping.)

+ Wall jumping (includes wall sliding.)


> For walljumping, the player doesnt have to hold one of the sideways keys. He can just stand next to wall. The detection zone is around 4 pixels on each side of the player.

----

## Making a level

First load the template to make levels. The scene is located in: "res://worlds/level_template.tscn". First thing first, to place the spawn point, there is a "spawn" position2D Node. You can use the grid at 8 pixel by 8 pixels to snap the spawn in place. 

### Making the shape

There are 5 Layers to make a level. the first layer is the foreground tile layer. The minimum resolution or size of a level should always be equivalent to the screen size. in other words, the minimum size of a level should always be 40 by 23 tiles. 

- fg_tile : The "Solid" part of the level. this is the layer the players walks on and collides with.
- bg_tile : This is the background tile layer. the player cant interact with it. Only serves aesthetic purposes.
- entities : The layer interactable objects are placed on. can be solid or not.
- fg_decals : This is the foreground detail layer. shows in front of everything else.
- bg_decals : THis isthe background detail layer, shows in front of the bg_tile.

The ordered layer structure look like this. The first element of this list display in front of everything else.

1. fg_decals
2. entities
3. fg_tile (main layer)
4. bg_decals
5. bg_tile

### Spawn

The spawn is of type *Position2D* and is fixed. It can be changed in the scene itself. 

### Entities

The main scene already comes with all of the entities placed in the level. you can duplicate the entitie by doing **ctrl + D** this will duplicate the selected item and youll now be able to move it. Here is the list of available entities.

### Good levels

If these rules are followed. the level will be considered good. First thing first, your level should already have all of the minimum requirements such as a spawn, the minimum level size(at least), an exit(or more).

To turn a decent level into a good level you should at least follow those rules.

1. The minimum breathable area for a non-claustrophobic level should always be 3. for example the ceilling height shouldn't be lower than 3. the minium walljump distance should never be lower than 3 or it will feel clunky as hell. In general there should always be a 3x3 zone for the player to move in.
2. you dont want the level to look empty. You want interactibilty in your level. something that makes the player think that he has freedom. Another good point would be to have multiple options. At least one normal way and one super hard way for speedrunners or etc.
3. Implementation of a puzzle or some sort. something that will make the player think a little instead of just breezing through your level in 10 seconds. trust me, you'll feel pretty dissapointed when you'll see something pass your level in 5 second that you just put 45 minutes into it.

4. It should always feel FAIR. No excuses. No pixel perfect jump. This is not megaman OK.
5. Measurements rules should be respected for your level to feel good.

### Measurement rules

| Ability     | Easy      | Medium    | Hard      |
| ----------- | --------- | --------- | --------- |
| Normal jump | 0-6 tiles | 6-7 tiles | 7 tiles   |
| Super jump  | 0-7 tiles | 7 tiles   | 8-9 tiles |
