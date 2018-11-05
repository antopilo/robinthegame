extends Control

onready var consoleLine = get_node("LineEdit")
onready var game_node = get_node("../../game/Viewport/game_root")
onready var player_node = get_node("../../game/Viewport/game_root/Player")
onready var weapon_node = get_node("../../game/Viewport/game_root/Player/Weapon")
var toggle = false
var lastInput = ""

func _input(event):
	if event.is_action_released("console"):
		self.visible = !self.visible
		
		if self.visible == true: 
			$LineEdit.grab_focus()
			$LineEdit.clear()
		else: 
			$LineEdit.FOCUS_NONE
			$LineEdit.clear()
		

	if event.is_action_pressed("ui_up"):
		consoleLine.text = lastInput
		
		
func _on_LineEdit_text_entered(new_text):
	consoleLine.clear()
	lastInput = new_text
	
	match(new_text):
		# Quit the game.
		"quit": get_tree().quit()
		
		"showgrid": # Display the debug tile grid.
			game_node.showgrid = !game_node.showgrid
			game_node.update()
		
		"respawn": # Respawn the player.
			game_node.spawn()
		
		"controller": 
			weapon_node.is_controller = !weapon_node.is_controller
		"controller 1":
			weapon_node.is_controller = true
		"controller 0":
			weapon_node.is_controller = false
			
		#Window: Sets the window size
		#	1: 320x180	
		#	2: 640x360
		#	3: 1280x720
		#	4: 1920x1080
		"window", "window 0", "window 1": OS.set_window_size(Vector2(320, 180))
		"window 2": OS.set_window_size(Vector2(640, 360))		
		"window 3": OS.set_window_size(Vector2(1280, 720))
		"window 4": 
			OS.set_window_size(Vector2(1920, 1080))
			OS.window_position = Vector2()
			
		#Fullscreen command
		"fullscreen 1": OS.window_fullscreen = true
		"fullscreen 0": OS.window_fullscreen = false

		#Vsync Command
		"vsync 1": OS.vsync_enabled = true
		"vsync 0": OS.vsync_enabled = false

		"borderless 0": OS.window_borderless = false
		"borderless 1": OS.window_borderless = true
		"ledit": get_tree().change_scene("res://Scene/level_editor/level_editor_main.tscn")
	if new_text.begins_with("say"):
		get_node("../Dialog").show_message(new_text.right(3))
			