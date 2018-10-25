extends KinematicBody2D

onready var weapon_node = get_node("../Player/Weapon")
onready var player_node = get_node("../Player")
onready var world_node = get_node("..")
onready var tween_node = get_node("Tween")
onready var tween2_node = get_node("Tween2")

var is_controller
var is_controlled = true
var is_frozen = false
var is_moving_back = false
var can_speed = true
var fuel = 100
var fuel_consum = 0.1

var is_deadzone = false
const deadzone = 0.25
const max_fuel = 100
const floor_normal = Vector2(0,1)
const ceilling_normal = Vector2(0,-1)
const wall_normal = Vector2(1,0)
const gravity = 2
var speed = 0.5
var direction
var angle
var last_direction
var freeze_position

var stretch_factor = OS.window_size.x / 320

func _ready():
	last_direction = Vector2(player_node.last_direction_x, 0)
	weapon_node.can_shoot = false
	player_node.arrow_exist = true
	player_node.arrow_node = self
	self.name = "arrow"
	
func _input(event):
	if event.is_action_pressed("fire") and can_speed:
		speed = 3
		is_controlled = false
		can_speed = false
		
	elif event.is_action_pressed("fire") and is_frozen:
		move_back_to_player()
		
func _physics_process(delta):
	
	if is_moving_back:
		look_at(player_node.global_position)
		return
	
	#if Arrow is Controlled.
	if is_controlled and !is_frozen:
		if fuel <= 0:
			 is_controlled = false
			 
		if is_controller: 
			joyStickControl()
			global_rotation = angle
		else: 
			mouseControl()
	if is_controller and !is_frozen:
		fuel -= fuel_consum
	
	apply_gravity()
	check_collision()

func mouseControl():
	look_at(get_global_mouse_position() / stretch_factor + (player_node.camera_node.get_camera_screen_center() - (get_viewport_rect().size / 2)) / 1.33)

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
	
	if !is_frozen and collision:	
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
	elif position.x <= world_node.currentLevel.levelPosition.x or position.y <= world_node.currentLevel.levelPosition.y or position.x >= (world_node.currentLevel.levelPosition.x + world_node.currentLevel.levelSize.x) or position.y >= (world_node.currentLevel.levelPosition.y + world_node.currentLevel.levelSize.y):
		move_back_to_player()
	elif is_frozen:
		position = freeze_position
	
func freezeArrow():
	freeze_position = global_position
	set_collision_layer_bit(0,true)

	speed = 0
	is_frozen = true
	can_speed = false
	
	$Particles2D.emitting = false
	
func move_back_to_player():
	var time = (self.position - player_node.position).length() / 300

	tween_node.follow_property(self, "global_position", global_position, player_node, "global_position", time, Tween.TRANS_QUINT, Tween.EASE_IN)
	tween2_node.interpolate_property(self, "scale", scale, Vector2(0,0), time ,Tween.TRANS_EXPO, Tween.EASE_IN)
	
	tween2_node.start()
	tween_node.start()
	
	player_node.arrow_exist = false
	is_moving_back = true 
	set_collision_layer_bit(0,false)

func _on_Tween_tween_completed(object, key):
	self.queue_free()
	weapon_node.can_shoot = true
	
func apply_gravity():

	if fuel <= 0:
		self.position.y += gravity
		look_at(player_node.position)