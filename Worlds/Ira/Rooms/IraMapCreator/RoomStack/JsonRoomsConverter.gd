extends Node2D

const MAIN_BUILT_TILE : int = 0		# Основной строительный тайл в карте ( НЕ ДОЛЖЕН СЧИТЫВАТЬСЯ )

var rooms = []						# Комнаты, лежащие в bin файле
var currentRoom = {}				# Текущая комната, идущая в bin файл
var xCoordinates = []				# Массив для сбора X координат тайлов
var yCoordinates = []				# Массив для сбора Y координат тайлов
var tileNames = []					# Массив для сбора "имен" тайлов, из которых собраны комнаты
var tilesNumber = 0
export var readWidth = 0			# Размеры поля, в котором находятся комнаты:
export var readHeight = 0
onready var tileMap = get_node("TileMap")
export var clear_all = false 
export var clear_last = false

func scanTileMap():
	var heightCounter = 0
	var widthCounter = 0
	while widthCounter < readWidth:
		while heightCounter < readHeight:
			if tileMap.get_cell(widthCounter, heightCounter) != -1:
				if tileMap.get_cell(widthCounter, heightCounter) != MAIN_BUILT_TILE:
					tilesNumber += 1
					xCoordinates.append(widthCounter)
					yCoordinates.append(heightCounter)
					tileNames.append(tileMap.get_cell(widthCounter, heightCounter))
			elif tileMap.get_cell(widthCounter, heightCounter) == -1:
				tilesNumber += 1
				xCoordinates.append(widthCounter)
				yCoordinates.append(heightCounter)
				tileNames.append(tileMap.get_cell(widthCounter, heightCounter))
			heightCounter += 1
		heightCounter = 0
		widthCounter += 1


func _ready():
	var iraRooms = File.new()
	if clear_all:
		iraRooms.open("res://.gameConfigurations/Generation/Ira/rooms.bin", File.WRITE)
		print("cleared!")
	#elif clear_last:
	#	iraRooms.open("res://.gameConfigurations/Generation/Ira/rooms.bin", File.READ)
	#	rooms.append(iraRooms.get_buffer(iraRooms.get_len()))
	#	var lastRoom = rooms[len(rooms)]	# СДЕЛАТЬ ПЕРЕВОД ИЗ BIN В  INT 
	#	for counter in range(0, lastRoom):
	#		 rooms.pop_back()
	else:
		iraRooms.open("res://.gameConfigurations/Generation/Ira/rooms.bin", File.READ)
		rooms.append(iraRooms.get_buffer(iraRooms.get_len())) 
		if rooms != null:
			print("read successful!")
		else:
			print("nothing to read from file!")
		iraRooms.close()
		iraRooms.open("res://.gameConfigurations/Generation/Ira/rooms.bin", File.WRITE)
		scanTileMap()
		print(tilesNumber)
		
		while tilesNumber != 0:
			if tilesNumber > 127:
				rooms.append(PoolByteArray([127]))
				tilesNumber -= 127
			elif tilesNumber < 127:
				rooms.append(PoolByteArray([tilesNumber]))
				tilesNumber = 0
				
		for counter in range(tileNames.size()):
			var binCellByteArray = PoolByteArray([xCoordinates[counter], yCoordinates[counter], tileNames[counter]])
			rooms.append(binCellByteArray)
		for counter in range(rooms.size()):
			iraRooms.store_buffer(rooms[counter])
			
	iraRooms.close()
