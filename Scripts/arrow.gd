extends KinematicBody2D

# References
onready var weapon_node = get_node("../Player/Weapon")
onready var player = get_node("../Player")
onready var world_node = get_node("..")
onready var tween_node = get_node("Tween")
onready var tween2_node = get_node("Tween2")

# state
var controller_mode
var is_controlled = true
var frozen = false
var is_moving_back = false
var can_speed = true

# Fuel
const max_fuel = 100
var fuel = 100
var fuel_consum = 0.1

#Controller
var is_deadzone = false
const deadzone = 0.25

# Constants
const floor_normal = Vector2(0,1)
const ceilling_normal = Vector2(0,-1)
const wall_normal = Vector2(1,0)
const gravity = 2

# Movement
var speed = 0.5
var direction
var angle
var last_direction
var freeze_position

var stretch_factor = OS.window_size.x / 320

func _ready():
	last_direction = Vector2(player.last_direction_x, 0)
	weapon_node.can_shoot = false
	player.arrow_exist = true
	player.arrow_node = self
	name = "arrow"
	
func _input(event):
	if event.is_action_pressed("fire") and can_speed:
		speed = 3
		is_controlled = false
		can_speed = false
		
	elif event.is_action_pressed("fire") and frozen:
		move_back_to_player()
		
func _physics_process(delta):
	if is_moving_back:
		look_at(player.global_position)
		return
	
	if is_controlled and !frozen:
		if fuel <= 0:
			 is_controlled = false
			 
		if controller_mode: 
			joyStickControl()
			global_rotation = angle
		else: 
			mouseControl()
			
	if controller_mode and !frozen:
		fuel -= fuel_consum
	
	apply_gravity()
	check_collision()

func mouseControl():
	var mouse = get_global_mouse_position()
	var center = player.camera.get_camera_screen_center()
	var position = mouse / stretch_factor + (center - (get_viewport_rect().size / 2)) / 1.33
	look_at(position)

func joyStickControl():
	# Gets the joystick Vector2 and get the angle(rad) of the joystick.
	direction = Vector2(Input.get_joy_axis(0, JOY_AXIS_2), Input.get_joy_axis(0, JOY_AXIS_3))
	angle = direction.angle()
	
	# Checks if the joystick is inside the deadzone.
	if abs(direction.x) < deadzone and abs(direction.y) < deadzone: 
		angle = last_direction.angle()
	else: 
		last_direction = direction
	
func check_collision():
	var collision = move_and_collide(global_transform.x * speed)
	
	if !frozen and collision:	
		if collision.normal == floor_normal or collision.normal == ceilling_normal:
			if global_rotation > deg2rad(-180) and global_rotation < deg2rad(0): 
				global_rotation = deg2rad(-90)
			else: 
				global_rotation = deg2rad(90)

		elif collision.normal == wall_normal or collision.normal == -wall_normal:
			if global_rotation > deg2rad(-90) and global_rotation < deg2rad(90): 
				global_rotation = deg2rad(0)
			else: 
				global_rotation= deg2rad(180)
		freezeArrow()

	elif position.x <= world_node.current_room.levelPosition.x or position.y <= world_node.current_room.levelPosition.y or position.x >= (world_node.current_room.levelPosition.x + world_node.current_room.levelSize.x) or position.y >= (world_node.current_room.levelPosition.y + world_node.current_room.levelSize.y):
		move_back_to_player()
	elif frozen:
		position = freeze_position
	
func freezeArrow():
	freeze_position = global_position
	set_collision_layer_bit(0,true)

	speed = 0
	frozen = true
	can_speed = false
	
	$Particles2D.emitting = false
	
func move_back_to_player():
	var time = (position - player.position).length() / 300

	tween_node.follow_property(self, "global_position", global_position, player, "global_position", time, Tween.TRANS_QUINT, Tween.EASE_IN)
	tween2_node.interpolate_property(self, "scale", scale, Vector2(0,0), time ,Tween.TRANS_EXPO, Tween.EASE_IN)
	
	tween2_node.start()
	tween_node.start()
	
	player.arrow_exist = false
	is_moving_back = true 
	set_collision_layer_bit(0,false)

func _on_Tween_tween_completed(object, key):
	queue_free()
	weapon_node.can_shoot = true
	
func apply_gravity():
	if fuel <= 0:
		position.y += gravity
