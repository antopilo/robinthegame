extends Node2D

#THIS SCRIPT IS RESPONSIBLE FOR THE "SHOWGRID" COMMAND.
#IT DRAWS A GRID OF 8 PIXELS BY 8 PIXELS
var toggle = false
onready var player = get_parent().get_node("Player")
	
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
	var new_limit_right = new_area.position.x + new_area.bounds_width
	var new_limit_top = new_area.position.y
	var new_limit_bottom = new_area.position.y + new_area.bounds_height
	var new_camera_zoom = Vector2(new_area.camera_zoom, new_area.camera_zoom)

	var camera = player.camera

	# Set limits to current position to make transition smooth
	# Otherwise if left limit is really far away and we transition to the right, it barely pushes 
	# the screen right until the last milliseconds and janks it into position 
	camera.limit_right = camera.get_camera_screen_center().x + get_viewport().size.x / 2 * camera.zoom.x
	camera.limit_left = camera.get_camera_screen_center().x - get_viewport().size.x / 2 * camera.zoom.x
	camera.limit_top = camera.get_camera_screen_center().y - get_viewport().size.y / 2 * camera.zoom.y
	camera.limit_bottom = camera.get_camera_screen_center().y + get_viewport().size.y / 2 * camera.zoom.y

	# Tween variables
	var time = 0.2
	var trans = Tween.TRANS_SINE
	var tease = Tween.EASE_IN

	tween.interpolate_property(camera, "limit_left", camera.limit_left, new_limit_left, time, trans, tease)
	tween.interpolate_property(camera, "limit_right", camera.limit_right, new_limit_right, time, trans, tease)
	tween.interpolate_property(camera, "limit_top", camera.limit_top, new_limit_top, time, trans, tease)
	tween.interpolate_property(camera, "limit_bottom", camera.limit_bottom, new_limit_bottom, time, trans, tease)
	tween.interpolate_property(camera, "zoom", camera.zoom, new_camera_zoom, 0.6, trans, Tween.EASE_OUT)	

	tween.start()



func _draw():
	if toggle:
	    draw_set_transform(Vector2(), 0, Vector2(8 , 8))
		
		
	    for y in range(0, 256):
	        draw_line(Vector2(0, y), Vector2(48, y), Color(1,0,0))
	
	    for x in range(0, 256):
	        draw_line(Vector2(x, 0), Vector2(x, 48), Color(1,0,0))