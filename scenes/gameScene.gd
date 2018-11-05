extends Node2D

# This script is on the World itself.
onready var player = get_node("Player") 
onready var current_room = get_node("level1")
var showgrid = false # Toggle the debug grid.

func _ready():
	spawn()
	tween_camera_to_area(current_room)

func _physics_process(_delta):
	updateRoom()

# Update the current room.
func updateRoom():
	for room in get_children():
		if room.is_in_group("level"):
			var x = player.position.x # Current position of the player.
			var y = player.position.y
			var x_min = room.levelPosition.x # Current Room position(globally)
			var y_min = room.levelPosition.y
			var x_max = room.levelPosition.x + room.levelSize.x # Current Room limits(globally)
			var y_max = room.levelPosition.y + room.levelSize.y

			if (x > x_min) and (y > y_min) and (x < x_max) and (y < y_max) and current_room != room:
				current_room = room
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
	
	var camera = player.camera
	var center = camera.get_camera_screen_center()
	# Set limits to current position to make transition smooth
	# Otherwise if left limit is really far away and we transition to the right, it barely pushes 
	# the screen right until the last milliseconds and janks it into position 
	camera.limit_right = center.x + get_viewport().size.x / 2 * camera.zoom.x
	camera.limit_left = center.x - get_viewport().size.x / 2 * camera.zoom.x
	camera.limit_top = center.y - get_viewport().size.y / 2 * camera.zoom.y
	camera.limit_bottom = center.y + get_viewport().size.y / 2 * camera.zoom.y

	# Tween variables
	var time = 0.4
	var trans = Tween.TRANS_LINEAR
	var tease = Tween.EASE_IN_OUT
	
	tween.interpolate_property(camera, "limit_right", camera.limit_right, new_limit_right, time, trans, tease)
	tween.interpolate_property(camera, "limit_left", camera.limit_left, new_limit_left , time, trans, tease)
	tween.interpolate_property(camera, "limit_top", camera.limit_top, new_limit_top, time, trans, tease)
	tween.interpolate_property(camera, "limit_bottom", camera.limit_bottom, new_limit_bottom, time, trans, tease)
	tween.interpolate_property(camera, "zoom", camera.zoom, new_camera_zoom, 0.6, trans, Tween.EASE_OUT)	
	tween.start()
	
	if player.arrow_exist == true:
		get_node("arrow").move_back_to_player()

func _draw():
	if showgrid:
		draw_set_transform(Vector2(), 0, Vector2(8 , 8))
		
		for y in range(0, 500):
			draw_line(Vector2(0, y), Vector2(500, y),Color(1,0,0), 1.0)
	
		for x in range(0, 500):
			draw_line(Vector2(x, 0), Vector2(x, 500),Color(1,0,0), 1.0)

func spawn():
	# Create tween is none exists
	var spawn_tween
	if !has_node("Tween"):
		spawn_tween = Tween.new()
		spawn_tween.name = "spawnTween"
		add_child(spawn_tween)
		spawn_tween.connect("tween_completed", self, "_on_Tween_tween_completed")
	else:
		spawn_tween = get_node("spawnTween")

	spawn_tween.remove_all()
	
	# Set up the animation.
	var start_pos = player.position
	var end_pos = current_room.spawnPosition
	spawn_tween.interpolate_property(player,"position", start_pos, end_pos, 0.5, Tween.TRANS_EXPO, Tween.EASE_OUT)

	# Disable input of the player.
	player.collision.disabled = true
	player.can_control = false
	player.sprite_node.play("idle")

	# Start the tween!
	spawn_tween.start()
	tween_camera_to_area(current_room)
 
func _on_Tween_tween_completed(_object, _key):
	player.can_control = true
	player.collision.disabled = false

	
