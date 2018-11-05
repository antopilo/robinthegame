extends Node2D
onready var hitBox = get_node("StaticBody2D/CollisionShape2D")
onready var collision = get_node("Area2D")
var arrowPresent = false


func _on_Area2D_body_entered(body):
	if body.is_in_group("arrow"):
		$AnimatedSprite.frame = 4
		$AnimatedSprite.stop()
		hitBox.disabled = false
		arrowPresent = true
		
	if body.is_in_group("Player") and not arrowPresent: 
		body.jumpPad()
		$AnimatedSprite.stop()
		$AnimatedSprite.frame = 0
		$AnimatedSprite.play("bounce")
		#$AnimatedSprite.stop()

func _on_Area2D_body_exited(body):
	if body.is_in_group("arrow"):
		arrowPresent = false
		$AnimatedSprite.play("bounce")
		hitBox.disabled = true
