extends Node2D
# This script is on the World itself.
onready var player_node = get_node("Player")
onready var camera_node = get_node("Player/Camera2D")
onready var currentLevel = get_node("level1")
onready var spawn_tween_node = get_node("Tween")

var toggle = false

func _ready():
	spawn()
	tween_camera_to_area(currentLevel)
	_physics_process(true)

func _physics_process(delta):
	updateRoom()

func updateRoom():
	for room in get_children():
		if room.is_in_group("level"):
			if (player_node.position.x > room.levelPosition.x) and (player_node.position.y > room.levelPosition.y) and (player_node.position.x < (room.levelPosition.x + room.levelSize.x)) and (player_node.position.y < (room.levelPosition.y + room.levelSize.y)):
				if currentLevel != room:
					currentLevel = room
					tween_camera_to_area(room)
				
# Moves the camera to a given area
func tween_camera_to_area(new_area):
	var tween
	
	if !has_node("CameraAreaTween"):
		tween = Tween.new()
		tween.name = "CameraAreaTween"
		add_child(tween)
		
	else:
		tween = get_node("CameraAreaTween")

	# Stop any current tweens
	tween.remove_all()	
	
	# New camera limits
	var new_limit_left = new_area.position.x
	var new_limit_right = new_area.position.x + new_area.levelSize.x
	var new_limit_top = new_area.position.y
	var new_limit_bottom = new_area.position.y + new_area.levelSize.y
	var new_camera_zoom = Vector2(new_area.camera_zoom, new_area.camera_zoom)
	
	var camera = camera_node

	# Set limits to current position to make transition smooth
	# Otherwise if left limit is really far away and we transition to the right, it barely pushes 
	# the screen right until the last milliseconds and janks it into position 
	camera.limit_right = camera.get_camera_screen_center().x + get_viewport().size.x / 2 * camera.zoom.x
	camera.limit_left = camera.get_camera_screen_center().x - get_viewport().size.x / 2 * camera.zoom.x
	camera.limit_top = camera.get_camera_screen_center().y - get_viewport().size.y / 2 * camera.zoom.y
	camera.limit_bottom = camera.get_camera_screen_center().y + get_viewport().size.y / 2 * camera.zoom.y

	# Tween variables
	var time = 0.5
	var trans = Tween.TRANS_EXPO
	var tease = Tween.EASE_IN_OUT

	tween.interpolate_property(camera, "limit_left", camera.limit_left, new_limit_left , time, trans, tease)
	tween.interpolate_property(camera, "limit_right", camera.limit_right, new_limit_right, time, trans, tease)
	tween.interpolate_property(camera, "limit_top", camera.limit_top, new_limit_top, time, trans, tease)
	tween.interpolate_property(camera, "limit_bottom", camera.limit_bottom, new_limit_bottom, time, trans, tease)
	tween.interpolate_property(camera, "zoom", camera.zoom, new_camera_zoom, 0.6, trans, Tween.EASE_OUT)	

	tween.start()
	
	if player_node.arrow_exist == true:
		#pass
		get_node("arrow").move_back_to_player()

func _draw():
	if toggle:
		draw_set_transform(Vector2(), 0, Vector2(8 , 8))
		
		for y in range(0, 500):
			draw_line(Vector2(0, y), Vector2(500, y),Color(1,0,0), 1.0)
	
		for x in range(0, 500):
			draw_line(Vector2(x, 0), Vector2(x, 500),Color(1,0,0), 1.0)

func spawn():
	spawn_tween_node.interpolate_property(player_node,"position",player_node.position, currentLevel.spawnPosition,0.5,Tween.TRANS_EXPO,Tween.EASE_OUT)
	player_node.collision.disabled = true
	#player_node.death_particle.emitting = true
	player_node.can_control = false
	player_node.sprite_node.play("idle")
	spawn_tween_node.start()

	tween_camera_to_area(currentLevel)
 
func _on_Tween_tween_completed(object, key):
	player_node.death_particle.emitting = false
	player_node.can_control = true
	player_node.collision.disabled = false
	
	