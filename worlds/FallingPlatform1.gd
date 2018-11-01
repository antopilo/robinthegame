extends Node2D

onready var nodePlatform = get_node("Platform")
onready var nodeTween = get_node("TweenElevator")
onready var nodeTimer = get_node("RespawnCooldown")
onready var nodeShakeTimer = get_node("ShakeTimer")
onready var nodeCollision = get_node("Platform/Collision")

var init_pos = Vector2()
var fallDistance = 300

var isAvailable = true
var hasMovedUp = true

func _ready():
	init_pos = nodePlatform.position

func fall():
	#Make the block fall and start the cooldown timer.
	nodeTween.interpolate_property(nodePlatform, "position",
                init_pos, Vector2(init_pos.x, fallDistance), 6,
                Tween.TRANS_EXPO, Tween.EASE_OUT)
	nodeTween.start()
	nodeTimer.start()
	nodeCollision.disabled = true
	
func _on_Area2D_body_entered(body):
	if isAvailable and hasMovedUp and body.velocity.y >= 0: 
		# TO-DO: shaking effect
		nodeShakeTimer.start()
		isAvailable = false
		hasMovedUp = false
		
func _on_RespawnCooldown_timeout():
	#Move up the platform.
	nodeTween.interpolate_property(nodePlatform, "position",
                Vector2(init_pos.x, fallDistance), init_pos, 2,
                Tween.TRANS_EXPO, Tween.EASE_IN_OUT)
	nodeTween.start()

func _on_Tween_tween_completed(_object, _key):
	# If the platform is going down, then its not available yet.
	if hasMovedUp:
		isAvailable = true
		nodeCollision.disabled = false
	else: 
		hasMovedUp = true
		
func _on_ShakeTimer_timeout():
	fall()
