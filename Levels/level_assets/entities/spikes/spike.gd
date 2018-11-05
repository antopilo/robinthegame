extends Area2D

func _on_oSpike_body_entered(body):
	if body.is_in_group("Player"): 
		body.spawn()


func _on_oSpike_body_exited(body):
	if body.is_in_group("Player"): 
		body.spawn()
