extends Node2D

onready var collision = get_node("Area2D")
var activated = false

func _on_Area2D_body_entered(body):
	if body.is_in_group("Player") and activated == false: 
			body.jumpPad()
			$AnimatedSprite.stop()
			$AnimatedSprite.frame = 0
			$AnimatedSprite.play("bounce")

		
		
	if body.is_in_group("arrow"):
		if activated == false:
			
			$AnimatedSprite.stop()
			$AnimatedSprite.frame = 4
			collision.position.y -= 12
			activated = true
		
	


func _on_Area2D_body_exited(body):
	pass # Replace with function body.
