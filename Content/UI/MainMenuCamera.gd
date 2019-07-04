extends Camera2D

# Small scripts that creates a parallax effects
# It moves the parallax layers with the distance
# from the mouse to the center on the screen.
# Used on the mainmenu.
# Made by : Antoine Pilote
# On date : 2019-06-30

var mouse_position = Vector2()
var center = Vector2()
var mouse_distance_from_center = Vector2()

func _process(delta):
	mouse_position = get_global_mouse_position()
	center = Vector2(1920, 1080) / 2
	mouse_distance_from_center = (mouse_position - center) / 6
	self.position = mouse_distance_from_center
	
