using Godot;
using System;
using SheetIncludes;
using System.Collections.Generic;
using System.IO;

public class MapGenerator : Node2D
{
  public List<SheetIncludes.Sheet> sheetsList = new List<SheetIncludes.Sheet> ();                       // Список "листов"
  public List<SheetIncludes.Sheet> sheetsListWithRooms = new List<SheetIncludes.Sheet> ();              // Список "листов"
  public List<SheetIncludes.Room> avaliableIraRooms = new List<SheetIncludes.Room> ();                  // Список доступных комнат
  private SheetIncludes.Sheet rootSheet;                                                                // Корневой (начальный) "лист"
  private Random randomNumberGenerator = new Random();        // Генератор случайных чисел
  
  public MapGenerator(SheetIncludes.Sheet _rootSheet) => rootSheet = _rootSheet; 
  
  public void readRooms()                     // Считывание комнат для вставки в листы (из .bin файла)
  {
	int tilesNumber = 0;
	int partOfTilesNumber = 0;
	byte[] tilesByteArray = new byte[3];
	int[] tilesNumArray = new int[3];
	FileStream roomFileStream = new FileStream (".gameConfigurations/Generation/Ira/rooms.bin", FileMode.Open, FileAccess.Read);
	BinaryReader binaryReader = new BinaryReader (roomFileStream);

	int roomCellCounter = 0;

	do
	{  
	   do
	   {
				
		partOfTilesNumber = binaryReader.Read();                                       // Читает размер комнаты
		tilesNumber += partOfTilesNumber;

	   } while ( partOfTilesNumber == 127);
	   if (tilesNumber > 0)
	   {
		SheetIncludes.Room readedRoom = new SheetIncludes.Room ();
		for (int readCounter = 0; readCounter < tilesNumber; readCounter ++)
		{
			binaryReader.Read(tilesByteArray, 0, 3);                                       // Читает Тайл
			readedRoom.addTile(tilesByteArray[0], tilesByteArray[1], tilesByteArray[2]);                 // Читает Тайл
		}
		readedRoom.setSize(tilesByteArray[0], tilesByteArray[1]);
		avaliableIraRooms.Add(readedRoom);                                                            // Комната добавляется в пулл
		roomCellCounter += tilesNumber;
		tilesNumber = 0;
		partOfTilesNumber = 0;
	   }
			
	}
	while (roomFileStream.Position != roomFileStream.Length);     
  }

  public List<SheetIncludes.Sheet> generateMap()   // Генерация карты 
  {
	SheetIncludes.Sheet currentSheet;           // Текущий "лист"
	SheetIncludes.Room room = new SheetIncludes.Room();
	bool isGenerating = true;
	int sheetIdCounter = 0;
	sheetsList.Add(rootSheet);                  // Добавление корневого "листа"
	readRooms();

	GD.Print("Rooms -> ", avaliableIraRooms.Count);

	while (isGenerating)
	{
	  isGenerating = false;     
	  for (int counter = 0; counter < sheetsList.Count; counter ++)
	  {
		currentSheet = sheetsList[counter];   // Текущий элемент массива "листов"
		currentSheet.id = sheetIdCounter;
		sheetIdCounter++;
		if (currentSheet.split() == true)
		{
			sheetsList.Add(currentSheet.getLeftSheet());    // Добавление дочерних "листов"
			sheetsList.Add(currentSheet.getRightSheet());
			isGenerating = true;
		}
		else if (currentSheet.split() == false) 
		{
			room = avaliableIraRooms[randomNumberGenerator.Next(0, avaliableIraRooms.Count)];
			currentSheet.generateRoom(room);

		}

	  }

	  
  	}
	
	return sheetsList;
  }
}

public class Painter 
{
	SheetIncludes.VectorInteger2 startCoordinates = new SheetIncludes.VectorInteger2(0,0);
	int height = 0;
	int width = 0;
	
	public Painter(int _x, int _y, int _width, int _height)
	{
		startCoordinates.x = _x;
		startCoordinates.y = _y;
		width = _width;
		height = _height;
	} 

	public void paintScene(ref List<SheetIncludes.Sheet> sheets, ref TileMap tileMap)
	{
		for (int widthCounter = 0; widthCounter < width + 2; widthCounter ++)
		{
			for (int heightCounter = 0; heightCounter < height + 2; heightCounter ++)
			{
				int tileWidthCoords = startCoordinates.x + widthCounter;
				int tileHeightCoords = startCoordinates.y + heightCounter; 
				tileMap.SetCell(tileWidthCoords, tileHeightCoords, (int)TileIndexes.MAIN_BUILD_TILE);
			}
		}

		int roomCounter = 0; 

		for (int sheetCounter = 0; sheetCounter < sheets.Count; sheetCounter ++)		// Рисование комнаты
		{
			if ( sheets[sheetCounter].getRoom() != null )
			{
				roomCounter ++;
				int tilesCount = sheets[sheetCounter].getRoom().tiles.Count;
				for (int tileCounter = 0; tileCounter < tilesCount; tileCounter ++)
				{
					int tileCoordsX = sheets[sheetCounter].getRoom().tiles[tileCounter].tileCoordinates.x + sheets[sheetCounter].getRoom().coordinates.x;
					int tileCoordsY = sheets[sheetCounter].getRoom().tiles[tileCounter].tileCoordinates.y + sheets[sheetCounter].getRoom().coordinates.y;
					int tileDrawIndex = sheets[sheetCounter].getRoom().tiles[tileCounter].tileIndex;
					tileMap.SetCell(tileCoordsX, tileCoordsY, tileDrawIndex);
				}
			}
		}
	} 
}


public class IraMapCreator : Node2D
{
  MapGenerator generator = new MapGenerator (new Sheet (0, 0, 280, 200));
  List<SheetIncludes.Sheet> sheets;
  Painter painter = new Painter(0, 0, 280, 200);
  public override void _Ready()
  {
	TileMap tileMap = GetNode<TileMap>("TileMap");

	sheets = generator.generateMap();
	painter.paintScene(ref sheets, ref tileMap);


	// for (int inCounter = 0; inCounter < sheets.Count; inCounter ++)
	// {      
	//   for (int counter = 0; counter < sheets.Count; counter ++)   // Проба пера
	//   {
	// 	currSheet = sheets[counter];
	// 	for(int hCounter = 0; hCounter < currSheet.getHeight(); hCounter++) 
	// 	{
	// 		tileMap.SetCell(currSheet.getCoordinates().x, currSheet.getCoordinates().y + hCounter, 0);
	// 	}
	// 	for(int wCounter = 0; wCounter < currSheet.getWidth(); wCounter++) 
	// 	{
	// 		tileMap.SetCell(currSheet.getCoordinates().x + wCounter, currSheet.getCoordinates().y, 0);
	// 	}


	// 	for(int hCounter = 0; hCounter < currSheet.getHeight(); hCounter++) 
	// 	{
	// 		tileMap.SetCell(currSheet.getCoordinates().x + currSheet.getWidth(), currSheet.getCoordinates().y + hCounter, 0);
	// 	}
	// 	for(int wCounter = 0; wCounter < currSheet.getWidth(); wCounter++) 
	// 	{
	// 		tileMap.SetCell(currSheet.getCoordinates().x + wCounter, currSheet.getCoordinates().y + currSheet.getHeight(), 0);
	// 	}
	//   }
  	// }

	tileMap.UpdateBitmaskRegion();

  }
}

