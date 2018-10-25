extends Control

const tile_size = 16
onready var container = get_node("Container")
onready var viewport = container.get_node("ViewportContainer")
onready var camera = viewport.get_node("Viewport/Camera2D")
onready var grid = viewport.get_node("Viewport/grid")
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
func drag_camera():
	offsetX = get_node("Container").rect_position.x
	offsetY = get_node("Container").rect_position.y
	offset = Vector2(offsetX, offsetY)

	if Input.is_action_just_pressed("mouse3") or Input.is_action_pressed("jump") and Input.is_action_just_pressed("fire"):
		camDrag = true
	if Input.is_action_just_released("mouse3") or Input.is_action_just_released("jump") or Input.is_action_just_released("fire"):
		camDrag = false

	if camDrag == true:
		cam_movement = (initialPos - get_global_mouse_position())
	camera.global_position = camera.global_position + cam_movement 
	cam_movement = Vector2()


	initialPos = get_global_mouse_position()
func expand(button):
	if button.name == "xpd_right":
		container.rect_size.x += tile_size
		grid.width += 1
		grid.update()
		container.rect_position.x -= tile_size / 2
	if button.name == "xpd_left":
		container.rect_size.x += tile_size
		grid.width -= 1
		grid.update()
		container.rect_position.x -= tile_size / 2
	if button.name == "xpd_top":
		container.rect_size.y += tile_size
		container.rect_position.y -= tile_size / 2
		grid.height -= 1
		grid.update()
	if button.name == "xpd_bot":
		grid.height += 1
		grid.update()
		container.rect_size.y += tile_size
		container.rect_position.y -= tile_size / 2
	if button.name == "zoom_plus":
		viewport.stretch_shrink += 1
		container.rect_size *= 2
		container.rect_position /= 2
	if button.name == "zoom_min":
		viewport.stretch_shrink -= 1
		container.rect_size /= 2
		container.rect_position *= 2
