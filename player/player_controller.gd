extends KinematicBody2D

# Nodes References
onready var collision = get_node("collisionBox")
onready var death_particle = get_node("death_particle")
onready var sprite_node = get_node("AnimatedSprite")
onready var camera = get_node("Camera2D")
var arrow_node 

# Constants
const GRAVITY = 4
const ACCELERATION = 5
const DECELERATION = 4
const JUMP_FORCE = 190
const SUPER_JUMP_FORCE = 220
const WALL_JUMP_HEIGHT = 140
const WALL_JUMP_FORCE = 120
const JUMP_PAD_FORCE = 260
const MAX_SPEED = 90
const MAX_AIR_SPEED = 100
const MAX_FALL_SPEED = 300

# Physics related variables
var current_max_speed = MAX_SPEED
var gravity_multiplier = 1
var velocity = Vector2() # This is the movement of the player.

# Input related variables
var can_control = true
var last_direction_x = 0
var input_direction_x = 0
var input_direction_y = 0

# Player state
var state
var isCrouching = false
var is_wall_jumping = false
var was_on_ground = false
var can_wall_jump = false
var arrow_exist = false
var is_ceilling = false

# Inventory
var following = []

func _input(event):
	# Here is where Jump inputs are handled. 
	if event.is_action_pressed("jump"):
		if state == "Ground":
			was_on_ground = true
			
			if sprite_node.animation == "Crouch":
				superJump()
			else:
				Jump()
				
				
				
		elif can_wall_jump:
			was_on_ground = false
			wallJump()
	
	# If the jump button is released early, break the jump momentum. Mario jump style.
	if event.is_action_released("jump") and velocity.y < 0 and was_on_ground: 
		velocity.y /= 1.75

func _physics_process(delta):
	#Main Loop :
	get_input() 
	update_velocity() 
	update_state() 
	can_walljump()
	speed_limits() 
	move_and_slide(velocity) 
	apply_gravity() 
	get_arrow() 

func get_input():
	if can_control:
		# Left 
		if Input.is_action_pressed("ui_left"):
			sprite_node.play("Running") 
			sprite_node.flip_h = true
			input_direction_x = -1
			last_direction_x = -1
		# Right
		elif Input.is_action_pressed("ui_right"):
			sprite_node.play("Running")
			sprite_node.flip_h = false
			input_direction_x = 1
			last_direction_x = 1
			
		# None
		else: 
			input_direction_x = 0
			if state == "Ground": 
				sprite_node.play("idle")

		# Crouching
		if Input.is_action_pressed("ui_down") and state == "Ground": 
			isCrouching = true
			input_direction_y = 1 
			if input_direction_x == 0: 
				sprite_node.play("crouch")
		else: 
			input_direction_y = 0
			
	# If controls are disabled, zero out the directions.
	else: 
		input_direction_x = 0
		input_direction_y = 0

func update_velocity():
	# Updates the Vector2(velocity) responsible for moving the player. (passing velocity in move_and_slide)
	velocity.x += input_direction_x * ACCELERATION
	# Slow down.
	if abs(velocity.x) < 10 and input_direction_x == 0: velocity.x = 0

# States
func update_state():
	var ground = Vector2(0,-1)
	var left_wall = Vector2(1,0)
	var right_wall = Vector2(-1,0)
	var ceilling = Vector2(0,1)

	# Get collisions
	var collision
	var collision_count = get_slide_count() - 1
	
	if collision_count > -1:
		collision = get_slide_collision(collision_count)
		
		if collision.normal == ground and state != "Ground": 
			enterGroundState()		
		if (collision.normal == right_wall or collision.normal == left_wall):
			enterWallState()
		if collision.normal == ceilling:
			velocity.y = 0
		
	else: 
		enterAirState()
	print(state)
	
func enterGroundState():
	if state == "Air": 
		velocity.y = 3
	
	state = "Ground"
	current_max_speed = MAX_SPEED
	can_control = true
	is_ceilling = false

func enterWallState():
	state = "Wall"
	gravity_multiplier = 1
	if state == "Ground":
		can_wall_jump = true
	is_ceilling = false
	
	if velocity.y > 0: 
		velocity.y /= 1.1
		sprite_node.play("Wall")
	else:
		sprite_node.play("falling")

func enterAirState():
	
	# The air state.
	if velocity.y > 0: 
		sprite_node.play("falling")
	else: 
		sprite_node.play("Jump")
		
	state = "Air"
# Physics
func apply_gravity():
	# If the player is on the ground, gravity is 0.
	if state == "Ground": 
		gravity_multiplier = 0
	else: 
		gravity_multiplier = 1

	velocity.y += GRAVITY * gravity_multiplier
	
func speed_limits():
	# Increase max Speed limit when doing a walljump.
	if is_wall_jumping == true and abs(velocity.x) > MAX_AIR_SPEED: 
		velocity.x = MAX_AIR_SPEED * sign(velocity.x)
		
	# If the player is over the current MAX_SPEED. reduce the player Speeds to the limit.
	if abs(velocity.x) > current_max_speed: 
		velocity.x = MAX_SPEED * sign(velocity.x)
		
	# Falling max speed.
	if abs(velocity.y) > MAX_FALL_SPEED: 
		velocity.y = MAX_FALL_SPEED * sign(velocity.y)
		
	# Zero out the velocity if the player isnt pressing anything u kno what i mean.
	if input_direction_x == 0 and can_control:  
		velocity.x -= DECELERATION * sign(velocity.x)
		
func can_walljump():
	# Condition if player can walljump.
	var colliding = $WallCheck_Left.is_colliding() or $WallCheck_right.is_colliding()
	
	if (colliding and state != "Ground") or state == "Wall": 
		can_wall_jump = true
	else: 
		can_wall_jump = false
		
#	--	Abilities	--	
func Jump(): 
	if state == "Ground":
		current_max_speed = MAX_SPEED
		velocity.y = -JUMP_FORCE
		
func superJump(): 
	if state == "Ground":
		current_max_speed = MAX_SPEED
		velocity.y = -SUPER_JUMP_FORCE

func wallJump():
	var jumpDirection = 0
	var collision
	is_wall_jumping = true
	current_max_speed = MAX_AIR_SPEED
	can_control = false
	$DisableInput.start()
	
	var collision_count = get_slide_count() - 1
	if collision_count > -1:
		collision = get_slide_collision(collision_count)
		
		if collision.normal == Vector2(1,0): 
			jumpDirection = 1
			sprite_node.flip_h = false
		if collision.normal == Vector2(-1,0): 
			jumpDirection = -1
			sprite_node.flip_h = true
			
	#If the player is NOT colliding. checks the closest wall.
	else:
		if $WallCheck_Left.is_colliding() and state != "Ground": 
			jumpDirection = 1
			sprite_node.flip_h = false	
		elif $WallCheck_right.is_colliding() and state != "Ground": 
			jumpDirection = -1
			sprite_node.flip_h = true	
	
	velocity.x = WALL_JUMP_FORCE * jumpDirection
	velocity.y = -WALL_JUMP_HEIGHT

func jumpPad():
	velocity.y = -JUMP_PAD_FORCE
	was_on_ground = false
	
func spawn():
	get_parent().spawn()
	velocity = Vector2()

func get_arrow():
	if arrow_exist and has_node("new_arrow"):
		arrow_node = get_node("../new_arrow")
	else:
		arrow_node = null
		
# Signals
func _on_DisableInput_timeout():
	can_control = true
	is_wall_jumping = false

func _on_Area2D_body_entered(body):
	if body.is_in_group("Player"):
		jumpPad()
	
