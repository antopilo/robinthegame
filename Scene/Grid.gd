extends Node2D

func _draw():
    print("DEBUG : Grid is on!")

    draw_set_transform(Vector2(), 0, Vector2(8 , 8))
	
    for y in range(0, 128):
        draw_line(Vector2(0, y), Vector2(128, y), Color(1,0,0))

    for x in range(0, 128):
        draw_line(Vector2(x, 0), Vector2(x, 128), Color(1,0,0))