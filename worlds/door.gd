extends Node2D

onready var collision = get_node("collision/box")
onready var sprite = get_node("sprDoor")

func open():
	collision.disabled = true
	sprite.visible = false

func _on_zone_body_entered(body):
	if body.name == "oKey":
		body.move_to(self)
		open()
