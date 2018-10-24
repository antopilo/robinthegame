extends Node2D

onready var player_node = get_node("..")
onready var timer_node = get_node("Timer")
onready var arrow_origin = get_node("Arrow_origin")
var current_arrow
var is_controller = true
var arrow_scene = preload("res://Scene/Mechanics/arrow.tscn")
var can_shoot = true

func _input(event):
	if event.is_action_pressed("fire") and can_shoot:
		can_shoot = false
		timer_node.start()
		shoot_arrow()

func shoot_arrow():
		current_arrow = arrow_scene.instance()
		var final_position = arrow_origin.global_position
		
		#player_node.arrow_node = new_arrow
		current_arrow.global_position = final_position
		current_arrow.is_controller = is_controller
		current_arrow.name = "arrow"

		get_node("../..").add_child(current_arrow)
		
func can_shoot(pCan_shoot):
	can_shoot = pCan_shoot
	
