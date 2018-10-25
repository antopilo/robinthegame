extends Control

const tile_size = 16
onready var container = get_node("Container")
onready var viewport = container.get_node("ViewportContainer")
# Called when the node enters the scene tree for the first time.
func _ready():
	for button in get_tree().get_nodes_in_group("btn_expand"):
		button.connect("pressed", self, "expand", [button])

func expand(button):
	if button.name == "xpd_right":
		container.rect_size.x += tile_size
		container.rect_position.x -= tile_size / 2
	if button.name == "xpd_left":
		container.rect_size.x += tile_size
		container.rect_position.x -= tile_size / 2
	if button.name == "xpd_top":
		container.rect_size.y += tile_size
		container.rect_position.y -= tile_size / 2
	if button.name == "xpd_bot":
		container.rect_size.y += tile_size
		container.rect_position.y -= tile_size / 2
	if button.name == "zoom_plus":
		viewport.stretch_shrink += 1
		container.rect_size *= 2
	if button.name == "zoom_min":
		viewport.stretch_shrink -= 1
		container.rect_size /= 2
		container.rect_position = get_parent().center()
