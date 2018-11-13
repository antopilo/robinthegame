extends Node2D
onready var hitBox = get_node("StaticBody2D/CollisionShape2D")
onready var collision = get_node("Area2D")
var arrowPresent = false
var angle
var direction = Vector2()
func _on_Area2D_body_entered(body):
	if body.is_in_group("arrow"):
		$AnimatedSprite.frame = 4
		$AnimatedSprite.stop()
		hitBox.disabled = false
		arrowPresent = true
		
	if body.is_in_group("Player") and not arrowPresent: 
	
		match(self.rotation_degrees):
			0.0:
				direction = Vector2(0,-1)
				
			90.0:
				direction = Vector2(1,0)
			180.0:
				direction = Vector2(0,1)
			270.0:
				direction = Vector2(-1,0)
			_:
				direction = Vector2(0,-1)
		body.jumpPad(direction)
		$AnimatedSprite.stop()
		$AnimatedSprite.frame = 0
		$AnimatedSprite.play("bounce")
		#$AnimatedSprite.stop()

func _on_Area2D_body_exited(body):
	if body.is_in_group("arrow"):
		arrowPresent = false
		$AnimatedSprite.play("bounce")
		hitBox.disabled = true
