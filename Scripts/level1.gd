extends Node2D
#Caching objects that could be in the level.
var oSpike = preload("res://Scene/Mechanics/oSpike.tscn")
var ojumpPad = preload("res://Scene/Mechanics/oJumpPad.tscn")
var oDoor = preload("res://Scene/Mechanics/oDoor.tscn")
var oKey = preload("res://Scene/Mechanics/oKey.tscn")
var oExtender = preload("res://Scene/Mechanics/oExtender.tscn")
var oSpawn = preload("res://Scene/Mechanics/oSpawn.tscn")
var oFallBlockSingle1 = preload("res://Scene/Mechanics/FallingPlatform1.tscn")

var iDoor = 0
var iKey = 1
var iFallBlockSingle1 = 2
var iFallBlockSingle2 = 3
var iExtender = 4
var iSpawn = 5
var iJumpPad = 6

# Get each layers
onready var tilemapSolid = get_node("fg_tile")
onready var tilemapSpike = get_node("entities")
onready var tilemapEntities = get_node("entities")
onready var objects = get_node("objects")

onready var levelSize = Vector2(tilemapSolid.get_used_rect().size.x * 8, (tilemapSolid.get_used_rect().size.y - 0.5) * 8)
onready var levelPosition = self.position
onready var spawnPosition

export var camera_zoom = 1

const MIN_WIDTH = 320
const MIN_HEIGHT = 180

func _ready():
	spawnEntities()
	spawnSpikes()

	if levelSize.x < MIN_WIDTH: levelSize.x = MIN_WIDTH
	if levelSize.y < MIN_HEIGHT: levelSize.y = MIN_HEIGHT

func spawnSpikes():
	#Place spikes on each Spike cells 
	for tile in tilemapSpike.get_used_cells_by_id(3):
		var new_spike = oSpike.instance()

		#Rotate the spike object to match the tile rotation.
		if tilemapSpike.is_cell_transposed(tile.x, tile.y) and !tilemapSpike.is_cell_x_flipped(tile.x, tile.y):
			new_spike.set_global_rotation(deg2rad(270))
			new_spike.set_global_position(tilemapSpike.map_to_world(Vector2(tile.x, tile.y + 1)))
		elif tilemapSpike.is_cell_transposed(tile.x, tile.y) and !tilemapSpike.is_cell_x_flipped(tile.x, tile.y - 1):
			new_spike.set_global_rotation(deg2rad(90))
			new_spike.set_global_position(tilemapSpike.map_to_world(Vector2(tile.x + 1, tile.y)))
		elif !tilemapSpike.is_cell_transposed(tile.x, tile.y) and tilemapSpike.is_cell_y_flipped(tile.x, tile.y):
			new_spike.set_global_rotation(deg2rad(180))
			new_spike.set_global_position(tilemapSpike.map_to_world(Vector2(tile.x + 1, tile.y + 1 )))
		else: 
			new_spike.set_global_rotation(deg2rad(0))
			new_spike.set_global_position(tilemapSpike.map_to_world(tile))
		tilemapSpike.add_child(new_spike)
		tilemapSpike.set_cell(tile.x, tile.y, -1)

	tilemapSpike.clear()
	
func spawnEntities():
	
	for ent in tilemapEntities.get_used_cells():
		
		# Door
		if tilemapEntities.get_cell(ent.x,ent.y) == iDoor:
			var new_door = oDoor.instance()
			new_door.name = "door"
			objects.add_child(new_door)
			new_door.global_position = tilemapEntities.map_to_world(ent)
			tilemapEntities.set_cell(ent.x, ent.y, -1)
			
		# Key
		elif tilemapEntities.get_cell(ent.x,ent.y) == iKey:
			var new_key = oKey.instance()
			new_key.name = "oKey"
			objects.add_child(new_key)
			new_key.global_position = tilemapEntities.map_to_world(ent)
			tilemapEntities.set_cell(ent.x,ent.y, -1)
			
		# FallBlockSingle1
		elif tilemapEntities.get_cell(ent.x,ent.y) == iFallBlockSingle1:
			var new_key = oFallBlockSingle1.instance()
			new_key.name = "oFallBlockSingle1"
			objects.add_child(new_key)
			new_key.global_position = tilemapEntities.map_to_world(ent)
			tilemapEntities.set_cell(ent.x,ent.y, -1)
			
		# Spawn
		elif tilemapEntities.get_cell(ent.x,ent.y) == iSpawn:
			var new_key = oSpawn.instance()
			new_key.name = "spawn"
			objects.add_child(new_key)
			new_key.global_position = tilemapEntities.map_to_world(ent)
			tilemapEntities.set_cell(ent.x,ent.y, -1)
			spawnPosition = new_key.position
			
		#iJumpPad
		elif tilemapEntities.get_cell(ent.x,ent.y) == iJumpPad:
			var new_key = ojumpPad.instance()
			new_key.name = "oJumpPad"
			objects.add_child(new_key)
			new_key.global_position = tilemapEntities.map_to_world(ent)
			tilemapEntities.set_cell(ent.x,ent.y, -1)
		#iExtender
		elif tilemapEntities.get_cell(ent.x,ent.y) == iExtender:
			var new_key = oExtender.instance()
			new_key.name = "oExtender"
			objects.add_child(new_key)
			new_key.global_position = tilemapEntities.map_to_world(ent)
			tilemapEntities.set_cell(ent.x,ent.y, -1)
	
	
	
