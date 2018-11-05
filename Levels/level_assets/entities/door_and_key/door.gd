extends Node2D

onready var key = get_node("../oKey")

onready var collision = get_node("collision/box")
onready var sprite = get_node("sprDoor")

const DETECT_RANGE = 40

var distance = Vector2()
var is_opened = false

func _physics_process(_delta):
	if key == null:
		key = get_node("../key")
	else:
		distance = abs((self.position - key.position).length())
		
		if distance <= DETECT_RANGE:
			key.move_to(self)
			open()
	print(distance)

func open():
	key.move_to(self)
	collision.disabled = true
	sprite.visible = false

