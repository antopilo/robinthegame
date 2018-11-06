extends Node2D

onready var player = get_node("../../../Player")
onready var sprite = get_node("sprKey")
onready var tween = get_node("Tween")


var detection_range = 20
var distance
var grabbed = false
var used = false
var delete_door = false
var index 
var target

func _physics_process(_delta):
	if grabbed == false:
		distance = abs((player.global_position - global_position).length())
		
		if distance <= detection_range:
			player.following.append(self)
			index = player.following.find(self,0)
			
			if index == 0:
				target = player
			else:
				target = player.following[index - 1]
				
			grabbed = true
			
	elif used == false and grabbed == true:
		
		tween.follow_property(self, 'global_position', global_position, target, 'global_position', 0.3, tween.TRANS_LINEAR, tween.EASE_OUT)
		tween.start()
		
func kill():
	tween.stop_all()
	sprite.visible = false
	
	player.following.pop_back()

func move_to(target):
	var destination = target.position
	tween.stop_all()
	tween.interpolate_property(self, "position", self.position, destination, 0.08,Tween.TRANS_EXPO,Tween.EASE_IN_OUT)
	tween.start()

func _on_Tween_tween_completed(_object, _key):
	if used == true:
		player.following.remove(self)

		kill()
func use():
	self.queue_free()
	
func change_target(target):
	tween.stop_all()
	tween.follow_property(self, 'global_position', global_position, target, 'position', 0.75, tween.TRANS_LINEAR, tween.EASE_OUT)
	tween.start()
	