tool
extends Sprite

export(int, 64) var SizeX = 8
export(int, 64) var SizeY = 8
export var color = Color(1,0,0)
export var filled = false

func _process(delta):
	update()
	
func _draw():
	var offset = position
	var Size = Vector2(SizeX, SizeY)
	if(centered):
		offset = position - (Size / 2)
	draw_rect(Rect2(offset, Size),color,filled)

