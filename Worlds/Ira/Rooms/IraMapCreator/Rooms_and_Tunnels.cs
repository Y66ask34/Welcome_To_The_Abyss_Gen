using Godot;
using System;
using System.Collections.Generic;
using System.IO;


namespace SheetIncludes
{

	public struct VectorInteger2
	{
		public int x;
		public int y;

		public VectorInteger2 (int _x, int _y)
		{
		  x = _x;
		  y = _y;
		}
		

	}
	
	public class GodotTile 
	{
		public VectorInteger2 tileCoordinates;
		public int tileIndex;

		public GodotTile (VectorInteger2 _tileCoordinates, int _tileIndex)
		{
			tileCoordinates.x = _tileCoordinates.x;
			tileCoordinates.y = _tileCoordinates.y;
			tileIndex = _tileIndex;
		}
	}

	enum TileIndexes			// Виды тайлов
	{
		EMPTY_TILE = -1,
		MAIN_BUILD_TILE = 0,
		GATE_TILE = 1
	}


	public class TunnelGate
	{
		public List<GodotTile> tunnelGateTiles = new List<GodotTile> ();
		public int generalYCoordinate = 0;

		public TunnelGate ()
		{
		}

		public TunnelGate (GodotTile firstTile)
		{
			tunnelGateTiles.Add(firstTile);
			generalYCoordinate = firstTile.tileCoordinates.y;
		}
	}


	public class Room
	{
		public VectorInteger2 coordinates = new VectorInteger2(0,0);                          // Координаты спавна комнаты (Точка крайнего левого угла)
		public List<GodotTile> tiles = new List<GodotTile> ();                     			  // Список тайлов комнаты
		public List<TunnelGate> tunnelGates = new List<TunnelGate> ();			 			  // Список тайлов ворот комнаты
		public bool isConnected = false;
		public int currentHeight = 0;                                                         // Размеры комнаты
		public int currentWidth = 0;
		public int roomId = 0;
		
		public const int Minimum_Height = 10;                                                  // Минимальные Размеры комнаты
		public const int Minimum_Width = 15;
		
		public Room () {}

		public Room (Room anotherRoom) 
		{
			tiles = anotherRoom.tiles;
			currentHeight = anotherRoom.currentHeight;
			currentWidth = anotherRoom.currentWidth;
		}
		
		public void addTile(int xCoordinate, int yCoordinate, int tileIndex)           // Добавление определенного тайла
		{
			//if (tileIndex == (int)TileIndexes.EMPTY_TILE)
			//{
				tiles.Add(new GodotTile(new VectorInteger2(xCoordinate,yCoordinate), tileIndex));
			//}
			  if (tileIndex == (int)TileIndexes.GATE_TILE)
			  {
			 	//tiles.Add(new GodotTile(new VectorInteger2(xCoordinate,yCoordinate), tileIndex));
			  
			 	if (tunnelGates.Count != 0)
			 	{	
			 		bool haveTile = false;

			 		for (int gatesCounter = 0; gatesCounter < tunnelGates.Count; gatesCounter++)
			 		{
			 			if (tunnelGates[gatesCounter].generalYCoordinate == yCoordinate)
			 			{
			 				tunnelGates[gatesCounter].tunnelGateTiles.Add(new GodotTile(new VectorInteger2(xCoordinate,yCoordinate), tileIndex));
			 				haveTile = true;
			 			}
			 		}

			 		if (haveTile == false) 
			 		{
			 			TunnelGate tunnelGate = new TunnelGate (new GodotTile(new VectorInteger2(xCoordinate,yCoordinate), tileIndex));
			 			tunnelGates.Add(tunnelGate);	
			 		} 
			 	} 
			 	else
			 	{
			 		TunnelGate tunnelGate = new TunnelGate (new GodotTile(new VectorInteger2(xCoordinate,yCoordinate), tileIndex));
			 		tunnelGates.Add(tunnelGate);
			 	}
			  }
			}
	
		public void setSize (int width, int height)
		{
			currentWidth = width + 1;
			currentHeight = height + 1;
		}
	}

	

	public class Sheet : Node2D     // Область инициализации комнат, на которые будет делиться сцена 
	{
	private VectorInteger2 coordinates = new VectorInteger2(0,0);         // Координатные начала "листа"
	private int height = 0;                                               // Размеры текущего "листа" 
	private int width = 0;      
	public const int Minimum_Width = 65;								  // Минимальные размеры "листа"
	public const int Minimum_Height = 55;                                
	public int id = 0;
	private int RoomCounterrr = 0;
  
	public  Sheet leftSheet = null;                                       // Левый "дочерний" лист
	public  Sheet rightSheet = null;                                      // Правый "левый" лист 

	private Room room = null;                               			  // Комната для "листа"
	private List<VectorInteger2> tunnel = new List<VectorInteger2> ();    // Список тайлов туннеля "листа"

	private Random randomNumberGenerator = new Random();                  // Генератор случайных чисел 

	public Sheet (int _initCoord_X, int _initCoord_Y, int _width, int _height)
	{
		coordinates.x = _initCoord_X;
		coordinates.y = _initCoord_Y;
		height = _height;
		width = _width;
	}

	public VectorInteger2 getCoordinates() 					{ return coordinates; }
	public int            getHeight()      					{ return height; }
	public int            getWidth()       					{ return width; }
	public Sheet          getLeftSheet()   					{ return leftSheet; }
	public Sheet          getRightSheet()  					{ return rightSheet; }
	public Room			  getRoom() 						{ return room; } 

	public bool split ()
	{
		if (leftSheet != null && rightSheet != null)   // Лист уже разделен
		{
		return false;
		}

				if (height > width)         // Разделение по горизонтали 
				{
				
				if(Minimum_Height > (height - Minimum_Height))
				{
					return false;
				}

				int heightSlice = randomNumberGenerator.Next(Minimum_Height, height - Minimum_Height);   // Разрез по горизонтали (разрез через высоту). От половины текущей высоты, до 1/4 этой высоты
				leftSheet = new Sheet (coordinates.x, coordinates.y, width, heightSlice);                            // Верхний лист
				rightSheet = new Sheet (coordinates.x, coordinates.y + heightSlice, width, height - heightSlice);    // Нижний лист
				return true;
				}
				else if (height < width)    // Разделение по вертикали
				{
				
				if (Minimum_Width > (width - Minimum_Width))
				{
					return false;
				}

				int widthSlice = randomNumberGenerator.Next(Minimum_Width, width - Minimum_Width);      // Разрез по вертикали (разрез через ширину). От половины текущей ширины, до 1/4 этой ширины
				leftSheet = new Sheet (coordinates.x, coordinates.y, widthSlice, height);                          // Левый лист
				rightSheet = new Sheet (coordinates.x + widthSlice, coordinates.y, width - widthSlice, height);    // Правый лист
				return true;
				}

				if (height > width)         // Разделение по горизонтали 
				{
				
				if(Minimum_Height > (height - Minimum_Height))
				{
					return false;
				}

				int heightSlice = randomNumberGenerator.Next(Minimum_Height, height - Minimum_Height);   // Разрез по горизонтали (разрез через высоту). От половины текущей высоты, до 1/4 этой высоты
				leftSheet = new Sheet (coordinates.x, coordinates.y, width, heightSlice);                            // Верхний лист
				rightSheet = new Sheet (coordinates.x, coordinates.y + heightSlice, width, height - heightSlice);    // Нижний лист
				return true;
				}
				else if (height < width)    // Разделение по вертикали
				{
				
				if (Minimum_Width > (width - Minimum_Width))
				{
					return false;
				}

				int widthSlice = randomNumberGenerator.Next(Minimum_Width, width - Minimum_Width);      // Разрез по вертикали (разрез через ширину). От половины текущей ширины, до 1/4 этой ширины
				leftSheet = new Sheet (coordinates.x, coordinates.y, widthSlice, height);                          // Левый лист
				rightSheet = new Sheet (coordinates.x + widthSlice, coordinates.y, width - widthSlice, height);    // Правый лист
				
				return true;
				}
		
	return false;
	}

	public bool generateRoom (Room newRoom) 
	{   
		if ( leftSheet == null && rightSheet == null )
		{
			RoomCounterrr += 1;

			GD.Print(" => ", RoomCounterrr);

			if (width > newRoom.currentWidth && height > newRoom.currentHeight)
			{
				room = new Room (newRoom);
				
				VectorInteger2 currentRoomCoordinates = new VectorInteger2 (randomNumberGenerator.Next(coordinates.x + 5, coordinates.x + width - room.currentWidth),         // Генерация координат
																			randomNumberGenerator.Next(coordinates.y + 5, coordinates.y + height - room.currentHeight));
				
				room.coordinates = currentRoomCoordinates;
				

				return true;
			}
		}
		return false;
	}

	// public bool generateTunnels()
	// {
	
	// 	if (leftSheet.getRoom() != null && rightSheet.getRoom() != null)
	// 	{	
	// 		//int rand = randomNumberGenerator.Next(0, leftSheet.getRoom().tunnelGates.Count);
	// 		//TunnelGate tunnelStartPoints = new TunnelGate (leftSheet.getRoom().tunnelGates[]);
	// 		//TunnelGate tunnelEndPoints = new TunnelGate (/*rightSheet.getRoom().tunnelGates[randomNumberGenerator.Next(0, rightSheet.getRoom().tunnelGates.Count)] */);	
			
	// 	}
	// 	else 
	// 	{
	// 		if (leftSheet.getRoom() == null)
	// 		{
	// 			leftSheet.generateTunnels();
	// 			// *************************************
	// 		}
	// 		else if (rightSheet.getRoom() == null)
	// 		{
	// 			rightSheet.generateTunnels();
	// 			// *************************************
	// 		}
	// 	}
	// }

	}
}
