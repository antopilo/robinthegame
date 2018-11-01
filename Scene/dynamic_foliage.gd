extends Node2D

onready var player_node = get_node("../../../Player")
onready var tween = get_node("Tween")

var detection_distance = 10
var distance 
var side = 0
var is_down = false
var state = "down"
var t = 0
var force = 0.2

func _physics_process(delta):
	distance = (player_node.position - self.position).length()
	
	if distance <= detection_distance and state == "down":
		if player_node.position.x - self.position.x < 0:
			side = 1
		
		else:
			side = -1

		tween.interpolate_property(self, "force", 0.8, 0.2, 0.5 ,Tween.TRANS_ELASTIC, Tween.EASE_OUT)
		t = side
		tween.start()

		
		
	elif distance > detection_distance and is_down == true and state == "up":
		tween.interpolate_property(self, force, 0, deg2rad(0), 0.5, Tween.TRANS_ELASTIC, Tween.EASE_OUT)
		tween.start()
		
		is_down = false

	t = t + delta
	self.rotation = sin(t) * force
func _on_Tween_tween_completed(object, key):

	if is_down == true:
		state = "up"
		
	elif is_down == false:
		state = "down"

		
