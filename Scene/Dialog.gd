extends Control

onready var label_node = get_node("label")
onready var tween = get_node("Tween")
	
func show_message(string):
	label_node.clear()
	var time = string.length() / 10
	
	#label_node.visible_characters = 0
	label_node.text = string
	
	tween.interpolate_property(label_node, "visible_characters", 0, string.length() , time, Tween.TRANS_LINEAR,Tween.EASE_OUT)
	tween.start()
