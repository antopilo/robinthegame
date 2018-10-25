extends Control

onready var tilemap = get_node("tilemaps/solid_tiles")
var current_tool = "draw"
var current_position
# Called when the node enters the scene tree for the first time.
func _ready():
	tilemap.set_cell(4,4,0)

func _process(delta):
	current_position = tilemap.world_to_map(get_viewport().get_mouse_position())

	if Input.is_action_pressed("fire") and current_tool == "draw":
		tilemap.set_cell(current_position.x, current_position.y, 2)
