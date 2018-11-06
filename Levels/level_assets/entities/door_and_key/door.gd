extends Node2D

onready var player = get_node("../../../Player")
onready var collision = get_node("collision/box")
onready var sprite = get_node("sprDoor")

const DETECT_RANGE = 15

var distance = Vector2()
var is_opened = false

func _physics_process(_delta):
	
	distance = abs((self.global_position - player.position).length())
	
	
	if distance <= DETECT_RANGE and player.following.size() >= 1 and !is_opened:
		open()

func open():
	is_opened = true
	collision.disabled = true
	sprite.visible = false
	player.following.back().kill()
	
