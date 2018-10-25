extends Control

onready var camera =get_node("Camera2D")
onready var currentZoom = camera.zoom
onready var currentLayer = get_node("grid/tilemaps/solid_tiles")
var offsetX
var offsetY
# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func _process(delta):
	if Input.is_action_pressed("fire"):
		draw()
		
func draw():
	print(get_global_mouse_position())
