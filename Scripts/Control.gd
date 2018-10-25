extends Control

onready var consoleLine = get_node("LineEdit")
onready var player_node = get_node("../ViewportContainer/Viewport/World/Player")

var toggle = false
var lastInput = ""

func _input(event):
	if event.is_action_pressed("console"):
		self.visible = !self.visible
		
		if self.visible == true: 
			$LineEdit.grab_focus()
			
		else: 
			$LineEdit.FOCUS_NONE
			
		$LineEdit.clear()

	if event.is_action_pressed("ui_up"):
		consoleLine.text = lastInput
		
func _on_LineEdit_text_entered(new_text):
	consoleLine.clear()
	lastInput = "new_text"
	
	match(new_text):
		"quit": 
			get_tree().quit()
		"showgrid":
			get_parent().get_node("ViewportContainer/Viewport/World").toggle = !get_parent().get_node("ViewportContainer/Viewport/World").toggle
			get_parent().get_node("ViewportContainer/Viewport/World").update()
		"respawn":
			get_parent().get_node("ViewportContainer/Viewport/World").spawn()
		"controller 1":
			get_parent().get_node("ViewportContainer/Viewport/World/Player/Weapon").is_controller = true
		"controller 0":
			get_parent().get_node("ViewportContainer/Viewport/World/Player/Weapon").is_controller = false
			
		#Window: Sets the window size
		#	1: 320x180	
		#	2: 640x480
		#	3: 1280x720
		#	4: 1920x1080
		"window", "window 0", "window 1": OS.set_window_size(Vector2(320, 180))
		"window 2": OS.set_window_size(Vector2(640, 480))		
		"window 3": OS.set_window_size(Vector2(1280, 720))
		"window 4": OS.set_window_size(Vector2(1920, 1080))
			
		#Fullscreen command
		"fullscreen 1": OS.window_fullscreen = true
		"fullscreen 0": OS.window_fullscreen = false

		#Vsync Command
		"vsync 1": OS.vsync_enabled = true
		"vsync 0": OS.vsync_enabled = false
			
		#"say": get_parent().get_node("Dialog").show_message("salut")
	if new_text.begins_with("say"):
		get_node("../Dialog").show_message(new_text.right(3))
			