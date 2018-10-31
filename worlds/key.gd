extends Node2D

onready var player = get_node("../../../Player")
onready var sprite = get_node("sprKey")
onready var tween = get_node("Tween")
var detection_range = 20
var distance
var grabbed = false
# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func _physics_process(delta):
	if grabbed == false:
		distance = abs((player.position - position).length())

		if distance <= detection_range:
			grabbed = true
	else:
		tween.follow_property(self, 'position', position, player, 'position', 0.75, tween.TRANS_LINEAR, tween.EASE_OUT)
		tween.start()
func kill():
	tween.stop_all()
	sprite.visible = false
	
