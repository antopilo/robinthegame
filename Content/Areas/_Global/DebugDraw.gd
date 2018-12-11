tool
extends Node2D

# This node just draws a colored rectangle in the editor. 
# The color changes if the size is big enough
# Red means Normal size or 1 screen big
# Green means bigger than the screen(The camera will follow the player)

export(int, 5) var corner_size = 2
export(int, 5) var line_size = 3
export(Color, RGBA) var expand_color = ColorN("green", 1)
export(Color, RGBA) var minimum_color = ColorN("red", 1)



func _process(delta):
	if Engine.editor_hint:
		if has_node("../Layers/fg_tile") && get_node("../Layers/fg_tile") != null:
			update()
		else:
			print("Error the debug renderer cant find fg_tile in:", get_node("../").name)
		

		if !has_node("LevelName"):
			var lbl = Label.new()
			lbl.add_font_override("normal", load("res://Content/UI/Fonts/Pixeled.tres"))
			lbl.rect_global_position = Vector2(25, 25)
			lbl.name = "LevelName"
			add_child(lbl)
		else:
			$LevelName.text = get_node("../").name
			$LevelName.add_font_override("normal", load("res://Content/UI/Fonts/Pixeled.tres"))
	else:
		self.queue_free()
	
	
func _draw():
	if !Engine.editor_hint:
		return
		
	var rect = Rect2(Vector2(), get_node("../Layers/fg_tile").get_used_rect().size * 8)
	if rect.size.x <= 320 && rect.size.y <= 184:
		draw_line(Vector2(0, 0), Vector2(320, 0), minimum_color, line_size)
		draw_line(Vector2(0, 184), Vector2(320, 184), minimum_color, line_size)
		
		draw_line(Vector2(320, 0), Vector2(320, 184), minimum_color, line_size)
		draw_line(Vector2(0, 0), Vector2(0, 184), minimum_color, line_size)
		
		draw_circle(Vector2(), corner_size, minimum_color)
		draw_circle(Vector2(320, 0), corner_size, minimum_color)
		draw_circle(Vector2(0, 184), corner_size, minimum_color)
		draw_circle(Vector2(320, 184), corner_size, minimum_color)
		
	elif rect.size.x > 320 && rect.size.y <= 184:
		draw_line(Vector2(0, 0), Vector2(rect.size.x, 0), expand_color, line_size)
		draw_line(Vector2(0, 184), Vector2(rect.size.x, 184), expand_color, line_size)
		
		draw_line(Vector2(rect.size.x, 0), Vector2(rect.size.x, 184), minimum_color, line_size)
		draw_line(Vector2(0, 0), Vector2(0, 184), minimum_color, line_size)
		
		draw_circle(Vector2(), corner_size, minimum_color)
		draw_circle(Vector2(rect.size.x, 0), corner_size, minimum_color)
		draw_circle(Vector2(0, 184), corner_size, minimum_color)
		draw_circle(Vector2(rect.size.x, 184), corner_size, minimum_color)
		
	elif rect.size.x <= 320 && rect.size.y > 184:
		draw_line(Vector2(0, 0), Vector2(320, 0), minimum_color, line_size)
		draw_line(Vector2(0, rect.size.y), Vector2(320, rect.size.y), minimum_color, line_size)
		
		draw_line(Vector2(320, 0), Vector2(320, rect.size.y), expand_color, line_size)
		draw_line(Vector2(0, 0), Vector2(0, rect.size.y), expand_color, line_size)
		
		draw_circle(Vector2(), corner_size, minimum_color)
		draw_circle(Vector2(320, 0), corner_size, minimum_color)
		draw_circle(Vector2(0, rect.size.y), corner_size, minimum_color)
		draw_circle(Vector2(320, rect.size.y), corner_size, minimum_color)
	else:
		draw_line(Vector2(0, 0), Vector2(rect.size.x, 0), expand_color, line_size)
		draw_line(Vector2(0, rect.size.y), Vector2(rect.size.x, rect.size.y), expand_color, line_size)
		
		draw_line(Vector2(rect.size.x, 0), Vector2(rect.size.x, rect.size.y), expand_color, line_size)
		draw_line(Vector2(0, 0), Vector2(0, rect.size.y), expand_color, line_size)
		
		draw_circle(Vector2(), corner_size, expand_color)
		draw_circle(Vector2(rect.size.x, 0), corner_size, expand_color)
		draw_circle(Vector2(0, rect.size.y), corner_size, expand_color)
		draw_circle(Vector2(rect.size.x, rect.size.y), corner_size, expand_color)
		