extends Camera2D

var is_shaking = false
var shake_amount = 2.0

func _process(delta):
	if is_shaking:
	    set_offset(Vector2( \
	        rand_range(-1.0, 1.0) * shake_amount, \
	        rand_range(-1.0, 1.0) * shake_amount \
	    ))