extends Node2D

#Caching objects that could be in the level.
var ofallingPlatform = preload("res://Levels/level_assets/entities/falling_platforms/FallingPlatform3Wide.tscn")
var oSpike = preload("res://Levels/level_assets/entities/spikes/oSpike.tscn")
var ojumpPad = preload("res://Levels/level_assets/entities/jump_pad/oJumpPad.tscn")
var oDoor = preload("res://Levels/level_assets/entities/door_and_key/oDoor.tscn")
var oKey = preload("res://Levels/level_assets/entities/door_and_key/oKey.tscn")
var oExtender = preload("res://Levels/level_assets/entities/extender/oExtender.tscn")
var oSpawn = preload("res://Levels/level_assets/entities/spawn/oSpawn.tscn")
var oFallBlockSingle1 = preload("res://Levels/level_assets/entities/falling_platforms/FallingPlatform1.tscn")

# Get each layers
onready var tilemapSolid = get_node("fg_tile")
onready var tilemapSpike = get_node("entities")
onready var tilemapBackground = get_node("bg_tile")
onready var tilemapEntities = get_node("entities")
onready var tilemapFgDecals = get_node("fg_decals")
onready var tilemapBgDecals = get_node("bg_decals")
onready var objects = get_node("objects")

onready var levelSize = Vector2(tilemapSolid.get_used_rect().size.x * 8, (tilemapSolid.get_used_rect().size.y - 0.5) * 8)
onready var levelPosition = self.position
onready var spawnPosition
export var camera_zoom = 1

const MIN_WIDTH = 320
const MIN_HEIGHT = 180

func _ready():
	spawnEntities()
	
	if levelSize.x < MIN_WIDTH: levelSize.x = MIN_WIDTH
	if levelSize.y < MIN_HEIGHT: levelSize.y = MIN_HEIGHT

func place_spike(tile):
	var new_spike = oSpike.instance()

	#Rotate the spike object to match the tile rotation.
	if tilemapEntities.is_cell_transposed(tile.x, tile.y) and !tilemapEntities.is_cell_x_flipped(tile.x, tile.y):
		new_spike.set_global_rotation(deg2rad(270))
		new_spike.set_global_position(tilemapEntities.map_to_world(Vector2(tile.x, tile.y + 1)))
	elif tilemapEntities.is_cell_transposed(tile.x, tile.y) and !tilemapEntities.is_cell_x_flipped(tile.x, tile.y - 1):
		new_spike.set_global_rotation(deg2rad(90))
		new_spike.set_global_position(tilemapEntities.map_to_world(Vector2(tile.x + 1, tile.y)))
	elif !tilemapEntities.is_cell_transposed(tile.x, tile.y) and tilemapEntities.is_cell_y_flipped(tile.x, tile.y):
		new_spike.set_global_rotation(deg2rad(180))
		new_spike.set_global_position(tilemapEntities.map_to_world(Vector2(tile.x + 1, tile.y + 1 )))
	else: 
		new_spike.set_global_rotation(deg2rad(0))
		new_spike.set_global_position(tilemapEntities.map_to_world(tile))
	objects.add_child(new_spike)
	tilemapEntities.set_cell(tile.x, tile.y, -1)
	
func spawnEntities():
	for ent in tilemapEntities.get_used_cells():
		var cell = tilemapEntities.get_cell(ent.x, ent.y)
		
		match(cell):
			0: place_ent(ent,"oDoor", oDoor)
			1: place_ent(ent,"oKey", oKey)
			2,3: place_ent(ent,"oFallBlockSingle1", oFallBlockSingle1)
			4: place_ent(ent,"oExtender", oExtender)
			5: place_ent(ent,"spawn", oSpawn)
			6: place_ent(ent,"oJumpPad", ojumpPad)
			7: place_spike(ent)
			
func place_ent(tile, name, object):
	var new_ent = object.instance()
	new_ent.name = name
	objects.add_child(new_ent)
	new_ent.global_position = tilemapEntities.map_to_world(tile)
	tilemapEntities.set_cell(tile.x,tile.y, -1)
	if object == oSpawn: spawnPosition = new_ent.global_position
