# Robin the game

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

### Movement  :arrows_counterclockwise:

The player can:

+ Move sideways

+ Normal jumping

+ Crouch jumping (A.K.A Super jumping.)

+ Wall jumping (includes wall sliding.)


> For walljumping, the player doesnt have to hold one of the sideways keys. He can just stand next to wall. The detection zone is around 4 pixels on each side of the player.

