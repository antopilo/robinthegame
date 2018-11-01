extends Node2D

onready var sprite = get_node("AnimatedSprite")
onready var tween = get_node("Tween")
onready var collision = get_node("collision")

func _on_Area2D_body_entered(body):
	if body.is_in_group("arrow"): 
		extend()

func _on_Area2D_body_exited(body): 
	if body.is_in_group("arrow"): 
		retract()
	
func extend():
	
	tween.interpolate_property(collision, "position", collision.position, Vector2(8, -4), 0.5, Tween.TRANS_LINEAR, Tween.EASE_IN_OUT)
	tween.start()
	sprite.animation = "extend"
	sprite.play()
	
func retract():
	
	tween.interpolate_property(collision, "position", collision.position, Vector2(8, 12), 0.5, Tween.TRANS_LINEAR, Tween.EASE_IN_OUT)
	tween.start()
	sprite.animation = "retract"
	sprite.play()