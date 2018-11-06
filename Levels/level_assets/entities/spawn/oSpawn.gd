extends Position2D

var is_active = false

func _physics_process(_delta):
	if is_active:
		$fire/flame.emitting = true
		$fire/smoke.emitting = true
	else:
		$fire/flame.emitting = false
		$fire/smoke.emitting = false

func _on_Area2D_body_entered(body):
	if body.is_in_group("Player"):
		is_active = true
		get_node("../../").choose_spawn()
