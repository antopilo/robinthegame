extends Node2D

onready var player_node = get_node("..")
onready var timer_node = get_node("Timer")
onready var arrow_origin = get_node("Arrow_origin")
var current_arrow
var is_controller = false
var arrow_scene = preload("res://player/arrow.tscn")
var can_shoot = true

func _input(event):
	
	if event.is_action_pressed("fire") and can_shoot:
		can_shoot = false
		timer_node.start()
		shoot_arrow()
	if event.is_action_pressed("right_click") and can_shoot:
		can_shoot = false
		timer_node.start()
		shoot_arrow()
		current_arrow.dash()

func shoot_arrow():
	var final_position = arrow_origin.global_position
	current_arrow = arrow_scene.instance()
	
	current_arrow.global_position = final_position
	current_arrow.controller_mode = is_controller
	current_arrow.name = "arrow"

	get_node("../..").add_child(current_arrow)
		
	
