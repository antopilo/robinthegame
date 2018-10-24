extends Node2D

onready var tilemapSolid = get_node("tilemapSolid")
onready var tilemapSpike = get_node("tilemapSpike")
onready var spawnPosition = get_node("spawnPoint").global_position

export(float,0.5,2) var camera_zoom = 1

onready var levelSize = Vector2(tilemapSolid.get_used_rect().size.x * 16, (tilemapSolid.get_used_rect().size.y - 0.5) * 16)
onready var levelPosition = self.position

var toggle = false

#Scene File path.
var ofallingPlatform = preload("res://Scene/Mechanics/FallingPlatform3Wide.tscn")
var oSpike = preload("res://Scene/Mechanics/oSpike.tscn")
var ojumpPad = preload("res://Scene/Mechanics/oJumpPad.tscn")

func _ready():
	spawnEntities()
	spawnSpikes()
	
func spawnSpikes():
	#Place spikes on each Spike cells 
	for tile in tilemapSpike.get_used_cells_by_id(3):
		var new_spike = oSpike.instance()
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
		!tilemapSpike.add_child(new_spike)
		tilemapSpike.set_cell(tile.x, tile.y, -1)

	tilemapSpike.clear()
	
func spawnEntities():
	for child in get_node("Elements").get_children():
		if child.is_in_group("fallingPlatform"):
			var new_platform = ofallingPlatform.instance()
			self.add_child(new_platform)
			
			new_platform.global_position = child.global_position
			
		if child.is_in_group("jumpPad"):
			var new_jumpPad = ojumpPad.instance()
			self.add_child(new_jumpPad)
			
			new_jumpPad.global_position = child.global_position
func _draw():
	if toggle:
	    draw_set_transform(Vector2(), 0, Vector2(16 , 16))
		
	    for y in range(0, levelSize.x):
	        draw_line(Vector2(0, y), Vector2(levelSize.y, y),Color(1,0,0), 1.0)
	
	    for x in range(0, levelSize.y):
	        draw_line(Vector2(x, 0), Vector2(x, levelSize.x),Color(1,0,0), 1.0)