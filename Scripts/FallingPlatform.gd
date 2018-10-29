extends Node2D

onready var nodePlatform = get_node("/Platform")
onready var nodeTween = get_node("TweenElevator")
onready var nodeTimer = get_node("RespawnCooldown")
onready var nodeShakeTween = get_node("TweenShake")
onready var nodeShakeTimer = get_node("ShakeTimer")
onready var nodeCollision = get_node("Platform/Collision")

#var collision
#var fallDistance = 100

var isAvailable = true
var hasMovedUp = true

func fall():
	#Make the block fall and start the cooldown timer.

	nodeTween.interpolate_property(nodePlatform, "position",
                Vector2(0, 0), Vector2(0, 300), 3,
                Tween.TRANS_QUAD, Tween.EASE_OUT_IN)
	nodeTween.start()
	nodeTimer.start()
	nodeCollision.disabled = true
	
func _on_Area2D_body_entered(body):
	if isAvailable and hasMovedUp and body.velocity.y >= 0: 
		#DO THE SHAKING HERE
		nodeShakeTimer.start()
		isAvailable = false
		hasMovedUp = false
		
func _on_RespawnCooldown_timeout():
	#Move up the platform.
	nodeTween.interpolate_property(nodePlatform, "position",
                Vector2(0, 300), Vector2(0, 0), 2,
                Tween.TRANS_EXPO, Tween.EASE_IN_OUT)
	nodeTween.start()

func _on_Tween_tween_completed(object, key):
	#If the platform is going down, then its not available yet.
	if hasMovedUp:
		isAvailable = true
		nodeCollision.disabled = false
	else: hasMovedUp = true
		
func _on_ShakeTimer_timeout():
	fall()
