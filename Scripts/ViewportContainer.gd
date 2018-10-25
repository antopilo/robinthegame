extends ViewportContainer

#THIS SCRIPT IS RESPONSIBLE FOR THE "SHOWGRID" COMMAND.
#IT DRAWS A GRID OF 8 PIXELS BY 8 PIXELS
var showgrid = false
	
func _draw():
	if showgrid:
	    draw_set_transform(Vector2(0, 0), 0, Vector2(16 , 16))
		
	    for y in range(0, 48):
	        draw_line(Vector2(0, y), Vector2(48, y), Color(2,0,0))
	
	    for x in range(0, 48):
	        draw_line(Vector2(x, 0), Vector2(x, 48), Color(2,0,0))