extends Node2D

onready var key = get_node("../key")
onready var collision = get_node("collision/box")
onready var sprite = get_node("sprDoor")
var detection_range = 15
var distance = Vector2()
var opened = false

func open():
	collision.disabled = true
	sprite.visible = false

func _on_zone_body_entered(body):
	if body.name == "oKey":
		body.move_to(self)
		open()
	print(body.name)
