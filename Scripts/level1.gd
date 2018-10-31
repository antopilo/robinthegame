extends Node2D
#Caching objects that could be in the level.
var ofallingPlatform = preload("res://Scene/Mechanics/FallingPlatform3Wide.tscn")
var oSpike = preload("res://Scene/Mechanics/oSpike.tscn")
var ojumpPad = preload("res://Scene/Mechanics/oJumpPad.tscn")

# Get each layers
onready var tilemapSolid = get_node("fg_tile")
onready var tilemapSpike = get_node("entities")
onready var tilemapBackground = get_node("bg_tile")
onready var tilemapEntities = get_node("entities")
onready var tilemapFgDecals = get_node("fg_decals")
onready var tilemapBgDecals = get_node("bg_decals")

onready var levelSize = Vector2(tilemapSolid.get_used_rect().size.x * 8, (tilemapSolid.get_used_rect().size.y - 0.5) * 8)
onready var levelPosition = self.position
onready var spawnPosition = get_node("spawn").global_position

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
	for child in get_node("objects").get_children():
		if child.is_in_group("fallingPlatform"):
			var new_platform = ofallingPlatform.instance()
			self.add_child(new_platform)
			new_platform.global_position = child.global_position
			
		if child.is_in_group("jumpPad"):
			var new_jumpPad = ojumpPad.instance()
			self.add_child(new_jumpPad)
			
			new_jumpPad.global_position = child.global_position
