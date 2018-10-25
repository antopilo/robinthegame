extends Control

const tile_size = 16
onready var container = get_node("Container")
onready var viewportContainer = container.get_node("ViewportContainer")
onready var viewport = viewportContainer.get_node("Viewport")
onready var camera = viewport.get_node("Camera2D")
onready var grid_container = viewport.get_node("grid_viewer_root/grid")
onready var grid = viewport.get_node("grid_viewer_root/grid/grid")

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
	#if(camera.global_position.x >= 0 and camera.global_position.y <= 0):
	camera.global_position = camera.global_position + cam_movement 

	cam_movement = Vector2()

	initialPos = get_viewport().get_mouse_position()


func shortcut():
	if Input.is_action_just_pressed("zoomin"):
		camera.zoom.x -= 0.1
		camera.zoom.y -= 0.1
	if Input.is_action_just_pressed("zoomout"):
		camera.zoom.x += 0.1
		camera.zoom.y += 0.1
	if Input.is_action_just_pressed("ui_right"):
		expandW()
	if Input.is_action_just_pressed("ui_left"):
		reduceW()
	if Input.is_action_just_pressed("ui_down"):
		expandH()
	if Input.is_action_just_pressed("ui_up"):
		reduceH()

func expand(button):
	if button.name == "xpd_right":
		expandW()
	if button.name == "xpd_left" and grid.width > 0:
		reduceW()
	if button.name == "xpd_top" and grid.height > 0:
		reduceH()
	if button.name == "xpd_bot":
		expandH()

	if button.name == "zoom_in":
		camera.zoom.x += 0.1
		camera.zoom.y += 0.1

	if button.name == "zoom_out":
		camera.zoom.x -= 0.1
		camera.zoom.y -= 0.1

func can_expand():
	if grid.height > 0 and grid.width > 0:
		return true
	return false

func expandW():
	grid_container.rect_size.x += tile_size / 2
	grid.width += 1
	grid.update()
func expandH():
	grid_container.rect_size.y += tile_size / 2
	grid.height += 1
	grid.update()
func reduceW():
	grid.width -= 1
	grid.update()
	grid_container.rect_size.x -= tile_size / 2
	#grid_container.rect_position.y -= tile_size / 2
func reduceH():
	grid_container.rect_size.y -= tile_size / 2
	#container.rect_position.y += tile_size / 2
	grid.height -= 1
	grid.update()


















