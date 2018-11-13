extends CanvasLayer

onready var animationPlayer = get_node("Transition/AnimationPlayer")

var fullscreen = false
# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func fadeIn():
	animationPlayer.play("FadeIn")
	
func fadeOut():
	animationPlayer.play("FadeOut")
	
func fadeFull():
	fullscreen = false;
	animationPlayer.play("FadeIn")

func _on_AnimationPlayer_animation_finished(anim_name):
	if anim_name == "FadeIn":
		animationPlayer.play("FadeOut")
		fullscreen = true
