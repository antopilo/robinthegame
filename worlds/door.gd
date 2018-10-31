extends Node2D

onready var key = get_node("../key")
onready var collision = get_node("collision/box")
onready var sprite = get_node("sprDoor")
var detection_range = 15
var distance = Vector2()
var opened = false
# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func _physics_process(delta):
	if opened == false:
		distance = abs((global_position - key.position).length())

	if distance <= 40:
		opened = true
		open()

func open():
	collision.disabled = true
	sprite.visible = false
	key.kill()
	