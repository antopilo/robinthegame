extends Node2D
onready var sprite = get_node("Platform/Sprite")
onready var nodePlatform = get_node("Platform")
onready var nodeTween = get_node("TweenElevator")
onready var nodeTimer = get_node("RespawnCooldown")
onready var nodeShakeTimer = get_node("ShakeTimer")
onready var nodeCollision = get_node("Platform/Collision")

var init_pos = Vector2()
var fallDistance = 300
var deltaTime = 0
var isAvailable = true
var hasMovedUp = true
var is_shaking = false
var original_color = self.modulate
func _ready():
	init_pos = nodePlatform.position

func _physics_process(_delta):
	
	if is_shaking:
		sprite.rotation_degrees = cos(deltaTime * 20) * 25
		deltaTime += _delta
	
func fall():
	
	#Make the block fall and start the cooldown timer.
	nodeTween.interpolate_property(nodePlatform, "position",
                init_pos, Vector2(init_pos.x, fallDistance), 0.8,
                Tween.TRANS_EXPO, Tween.EASE_IN)
	nodeTween.interpolate_property(nodePlatform, "modulate",
                original_color, Color(1,1,1,0), 3,
                Tween.TRANS_LINEAR, Tween.EASE_OUT_IN)
	nodeTween.start()
	nodeTimer.start()
	nodeCollision.disabled = true
	
func _on_Area2D_body_entered(body):
	if isAvailable and hasMovedUp and body.velocity.y >= 0: 
		is_shaking = true
		nodeShakeTimer.start()
		
		isAvailable = false
		hasMovedUp = false
		
func _on_RespawnCooldown_timeout():
	#Move up the platform.
	nodeTween.interpolate_property(nodePlatform, "position",
                Vector2(init_pos.x, fallDistance), init_pos, 2,
                Tween.TRANS_EXPO, Tween.EASE_OUT_IN)
	nodeTween.interpolate_property(nodePlatform, "modulate",
               Color(1,1,1,0), original_color, 3,
                Tween.TRANS_LINEAR, Tween.EASE_OUT_IN)
	nodeTween.start()

func _on_Tween_tween_completed(object, key):
	# If the platform is going down, then its not available yet.
	if hasMovedUp:
		isAvailable = true
		nodeCollision.disabled = false
		sprite.rotation_degrees = 0
	else: 
		hasMovedUp = true
		is_shaking = false
		sprite.rotation_degrees = 0
		
func _on_ShakeTimer_timeout():
	fall()
