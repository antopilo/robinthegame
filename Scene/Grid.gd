extends Control
var height = 23
var width = 40

var min_width = 40
var min_height = 23

var color = Color(1,0,0)

func _draw():
    
    draw_set_transform(Vector2(), 0, Vector2(8 , 8))
	
    for y in range(0, height + 1):
        if y > min_height:
            color = Color(0,1,1)
        else:
            color = Color(1,0,0)
        draw_line(Vector2(0, y), Vector2(width, y), color)

    for x in range(0, width + 1):
        if x > min_width:
            color = Color(0,1,1)
        else:
            color = Color(1,0,0)
        draw_line(Vector2(x, 0), Vector2(x, height), color)








