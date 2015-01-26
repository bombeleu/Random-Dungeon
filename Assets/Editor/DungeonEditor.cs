using UnityEditor;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Dungeon))]
public class DungeonEditor : Editor {
    
    public Dungeon dungeon;
	public List<Vector2> cells =  new List<Vector2>();
	private SpriteProvider spriteProvider;
	public Vector2 lastMazeDir;
	private Dictionary<int,GameObject> tilePool = new Dictionary<int, GameObject>();
	public int windingPercent = 70;
	public int numRooms = 10;
	public int roomExtraSize = 2;
	public ArrayList rooms = new ArrayList();
	public Vector2 lastDir = new Vector2(-1,-1);
	public Stage stage;
	public List<Vector2> unmadeCells = new List<Vector2>();
	public float _mazeStartX = 1;
	public float _mazeStartY = 1;
	public List<Vector2> _mazeCells;

	public override void OnInspectorGUI(){

        GUILayout.BeginVertical();
        EditorGUILayout.Vector2Field("Dungeon Size: ", dungeon.size);
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Texture2D");
        dungeon.texture2D = (Texture2D)EditorGUILayout.ObjectField(dungeon.texture2D, typeof(Texture2D), false);
        GUILayout.EndHorizontal();

        if (dungeon.texture2D != null)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tile Size:", dungeon.tileSize.x + "x" + dungeon.tileSize.y);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Grid Size In Units:", dungeon.gridSize.x + "x" + dungeon.gridSize.y);
            GUILayout.EndHorizontal();

            var sprite = spriteProvider.GetSprite("Wall");
            var width = sprite.textureRect.width;
            var height = sprite.textureRect.height;
            dungeon.tileSize = new Vector2(width, height);
            dungeon.gridSize = new Vector2((width / 100) * dungeon.size.x, (height / 100) * dungeon.size.y);
        }

        if (GUILayout.Button("Generate"))
        {
            BuildDungeon();
        }

        if (GUILayout.Button("Clear Tiles"))
        {
            if (EditorUtility.DisplayDialog("Clear Map's Tiles?","Are you sure?", "Clear", "Do Not Clear"))
            {
                ClearMap();
            }
        }
    }

    void ClearMap(){
        for(int i = 0; i < dungeon.transform.childCount; i++){
            Transform tra = dungeon.transform.GetChild(i);
            DestroyImmediate(tra.gameObject);
            i--;
        }
    }
	void carve(Vector2 pos, TileType type) {
		if (type == TileType.Ground){ 
			type = TileType.Ground;

		
		}
		stage.tileType[(int)pos.x,(int)pos.y] = type;
		//_regions[pos] = _currentRegion;
	}
	bool canCarve(Vector2 pos,Vector2 dir){
		if(pos.x + dir.x * 3 >= stage.size.x || pos.x + dir.x *3 <= 0 || pos.y + dir.y * 3 >= stage.size.y || pos.y + dir.y * 3<= 0)
			return false;

		return stage.tileType[(int)(pos.x + dir.x +dir.x),(int)(pos.y + dir.y +dir.y)] == TileType.Empty;


	}
	void growMaze (Vector2 start)
	{
		 

		
		//_startRegion();
		carve(start,TileType.Ground);
		
		cells.Add(start);
		while (cells.Count != 0) {
			Vector2 cell = cells[cells.Count-1];

			// See which adjacent cells are open.
			List<Vector2> unmadeCells = new List<Vector2>();
			Vector2[] directions = Directions.cardinal;
			for (int i = 0; i < directions.Length; i++) {
					if(canCarve(cell,directions[i]))
						unmadeCells.Add(directions[i]);
			}
			//Debug.Log(1);
			if(unmadeCells.Count !=0){
				Vector2 dir;


				if(unmadeCells.Contains(lastDir) && Random.Range(0,101)> windingPercent){
					dir = lastDir;
				}else{
					dir = unmadeCells[Random.Range(0,unmadeCells.Count)];
				}
				_carve(new Vector2(cell.x + dir.x,cell.y + dir.y),TileType.Undifine);
				_carve(new Vector2(cell.x + dir.x + dir.x ,cell.y + dir.y + dir.y),TileType.Undifine);
				cells.Add(new Vector2(dir.x+dir.x,dir.y+dir.y));
				lastDir = dir;
			}else{

				cells.RemoveAt(cells.Count-1);


				lastDir =  new Vector2(-1,-1);
			}

		}

	}

    void BuildDungeon()
    {
		_mazeCells = new List<Vector2>();
		rooms = new ArrayList();
		cells =  new List<Vector2>();
		unmadeCells = new List<Vector2>();
		SpriteProvider spriteProvider = new SpriteProvider(AssetDatabase.GetAssetPath(dungeon.texture2D));
		stage = new Stage(dungeon.size);
		stage.FillStage();

		AddRooms();
		foreach (Room room in rooms) {
			stage.AddRoom(room);
		}

		for (int i = 0; i < dungeon.size.y; i++) {
			for (int j = 0; j < dungeon.size.x; j++) {
				if((j == 0)||(dungeon.size.x-1 == j)||(i == 0)||(dungeon.size.y-1== i)){
					stage.tileType[j,i]=TileType.Wall;
				}
					//stage.tileType[j,i]!=TileType.Empty
				//if(stage.tileType[j,i]!=TileType.Empty)
				//	continue;
				//growMaze(new Vector2(j,i));

			}
		}
		/*
		for (int ii = 1; ii < dungeon.size.y-1; ii++) {
			for (int jj = 1; jj < dungeon.size.x-1; jj++) {
				if(stage.tileType[jj,ii]!=TileType.Empty)
					continue;
				growMaze(new Vector2(jj,ii));
				
			}
		}
		*/

		_startMaze();
		_growMaze();
		findOpenCells();




		int index = 0;
		float y = 0, x = 0;
		for (int i = 0; i < dungeon.size.y; i++) {
			for (int j = 0; j < dungeon.size.x; j++) {
				GameObject ob;
				tilePool.TryGetValue(index,out ob);
				if (ob==null)
				{
					Tile tile = Tile.CreateTile((TileType)stage.tileType[j,i],new Vector2(x, y),spriteProvider,index);
					tile.gameObject.transform.parent = dungeon.transform;
					tilePool.Add(tile.id,tile.gameObject);
				}else{
					Tile obTile = ob.GetComponent<Tile>();

					obTile.SwitchTileState(stage.tileType[j,i],spriteProvider);
				}

				x += dungeon.tileSize.x/100;
				x = Mathf.Round(x*100)/100;

				index++;
			}
			y += dungeon.tileSize.y/100;
			y = Mathf.Round(y*100)/100;
			x = 0;
		}

	}

	void _startMazeCell ()
	{
		Vector2 pos = new Vector2(_mazeStartX, _mazeStartY);
		_mazeCells.Add(pos);
		//_currentRegion++;
		_carve(pos,TileType.Undifine);
	}

	bool _startMaze() {
		if (_mazeStartY >= dungeon.size.y - 1) 
			return false;
		
		// Find the next solid place to start a maze.
		while (stage.tileType[(int)_mazeStartX,(int) _mazeStartY] != TileType.Empty) {
			_mazeStartX += 2;
			if (_mazeStartX >= dungeon.size.x - 1) {
				_mazeStartX = 1;
				_mazeStartY += 2;
				
				// Stop if we've scanned the whole dungeon.
				if (_mazeStartY >= dungeon.size.y - 1) {
					//_findConnectors();
					return false;
				}
			}
		}
		
		_startMazeCell();
		return true;
	}

	bool _growMaze() { 

		if (_mazeCells.Count <= 0)
			return false;

		while (_mazeCells.Count > 0) {

			List<Vector2>  openDirs = new List<Vector2>();
			Vector2 cell = _mazeCells[_mazeCells.Count-1];

			Vector2[] dirs = Directions.cardinal;

			for (int i = 0; i < dirs.Length; i++) {
				if(cell.x + dirs[i].x * 3 >= stage.size.x ||  cell.x + dirs[i].x *3 <= 0 || cell.y + dirs[i].y * 3 >= stage.size.y || cell.y + dirs[i].y * 3<= 0)
					continue;
				if(stage.tileType[(int)(cell.x + dirs[i].x *2),(int)(cell.y + dirs[i].y * 2)]!= TileType.Empty )
					continue;
				openDirs.Add(dirs[i]);
			}

			if(openDirs.Count ==0){
				_mazeCells.RemoveAt(_mazeCells.Count -1);
				continue;
			}

			Vector2 dir;

			if (openDirs.Contains(lastMazeDir) && Random.Range(0,101)> windingPercent) {
				dir = lastMazeDir;
			} else {
				dir = openDirs[Random.Range(0, openDirs.Count-1)];
			}

			lastMazeDir = dir;

			_carve(cell + dir,TileType.Undifine);
			_carve(cell + dir * 2,TileType.Undifine);

			_mazeCells.Add(cell + dir + dir);
				

		}
		return false;

	}
	void findOpenCells() {
		unmadeCells = new List<Vector2>();
		for (int ii = 2; ii < dungeon.size.y-2; ii++) {
			for (int jj = 2; jj < dungeon.size.x-2; jj++) {
				if(stage.tileType[jj,ii]!=TileType.Wall && stage.tileType[jj,ii]!=TileType.Ground)
					unmadeCells.Add(new Vector2(jj,ii));
				
				
			}
		}

	}
	void AddRooms(){
		
		for (int i = 0; i < numRooms; i++) {



			var size = Random.Range(1, 3 + roomExtraSize) * 2 + 1;
			var rectangularity = Random.Range(0, 1 + size / 2) * 2;
			var width = size;
			var height = size;
			if (Random.Range(1,3)==2) {
				width += rectangularity;
			} else {
				height += rectangularity;
			}

			var x = Random.Range(0,(dungeon.size.x - width)-2);
			var y = Random.Range(0,(dungeon.size.y - height)-2);
			
			Room room = new Room(new Vector2(x, y), width, height);

			bool overlaps = false;

			foreach (Room other in rooms) {
				if (room.Intersects(other)) {
					overlaps = true;
					break;
				}
			}

			
			if (overlaps) continue;

			rooms.Add(room);
			
			//_startRegion();

		}





	}



		void OnEnable()
    {
        dungeon = target as Dungeon;
        Tools.current = Tool.View;
		tilePool = new Dictionary<int, GameObject>();
        if (dungeon.texture2D)
        {

            spriteProvider = new SpriteProvider(AssetDatabase.GetAssetPath(dungeon.texture2D));

		}
    }

	void _carve (Vector2 pos,TileType type)
	{
		if(type == TileType.Empty){
			if (pos.y < dungeon.size.y - 1 && stage.tileType[(int)pos.x,(int) pos.y + 1] != TileType.Empty) {

				stage.tileType[(int)pos.x,(int) pos.y ] = TileType.Wall;
			} else{
				stage.tileType[(int)pos.x,(int) pos.y ] = TileType.Empty;
			}
			if (pos.y > 0 && stage.tileType[(int)pos.x,(int) pos.y - 1] == TileType.Empty) {
				stage.tileType[(int)pos.x,(int) pos.y - 1] = TileType.Empty;
			}
		}else{
			stage.tileType[(int)pos.x,(int) pos.y] = TileType.Ground;
			if (pos.y > 0 && stage.tileType[(int)pos.x,(int) pos.y - 1] == TileType.Empty) {
				stage.tileType[(int)pos.x,(int) pos.y - 1] = TileType.Wall;
			}
		}
	}
}
