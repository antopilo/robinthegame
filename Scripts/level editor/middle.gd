extends Control

onready var container = get_node("Container")
onready var viewportContainer = container.get_node("ViewportContainer")
onready var viewport = viewportContainer.get_node("Viewport")
onready var camera = viewport.get_node("grid_viewer_root/Camera2D")
onready var tilemap = viewport.get_node("layers/TileMap")

# Drag Camera
var offset
var offsetX
var offsetY
var camDrag = false
var initialPos
var cam_movement = Vector2()

# Called when the node enters the scene tree for the first time.
func _ready():
	for button in get_tree().get_nodes_in_group("btn_expand"):
		button.connect("pressed", self, "expand", [button])

func _process(delta):
	drag_camera()
	shortcut()

func drag_camera():
	offsetX = get_node("Container").rect_position.x
	offsetY = get_node("Container").rect_position.y
	offset = Vector2(offsetX, offsetY)

	if Input.is_action_just_pressed("mouse3") or Input.is_action_pressed("jump") and Input.is_action_just_pressed("fire"):
		camDrag = true
	if Input.is_action_just_released("mouse3") or Input.is_action_just_released("jump") or Input.is_action_just_released("fire"):
		camDrag = false
	if camDrag == true:
		cam_movement = (initialPos - get_viewport().get_mouse_position()) * camera.zoom.x

	camera.global_position = camera.global_position + cam_movement * camera.zoom.x
	cam_movement = Vector2()
	initialPos = get_viewport().get_mouse_position()

func shortcut():
	if Input.is_action_just_pressed("zoomin"):
		camera.zoom.x -= 0.1
		camera.zoom.y -= 0.1
	if Input.is_action_just_pressed("zoomout"):
		camera.zoom.x += 0.1
		camera.zoom.y += 0.1

















