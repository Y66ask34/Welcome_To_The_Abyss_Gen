extends Node2D

onready var gen = preload("res://Worlds/Ira/Rooms/IraMapCreator/IraMapCreator.tscn").instance()

var rooms = []

func _ready():
	add_child(gen)

func _input(_event):
	if Input.is_action_just_pressed("Jump"):
		var sheets = gen.sheets
		var rooms = []
		for sheet in sheets:
			if sheet.haveRoom():
				rooms.append(sheet.getRoom())
				print(sheet.getRoom())
		#for room in rooms:
			#print(room.coordinates)
			#print(room.tiles)
		
