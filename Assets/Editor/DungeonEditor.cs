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
   
	private Dictionary<int,GameObject> tilePool = new Dictionary<int, GameObject>();
	public int windingPercent = 70;
	public int numRooms = 10;
	public int roomExtraSize = 2;
	public ArrayList rooms = new ArrayList();

	public Stage stage;
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

		stage.tileType[(int)pos.x,(int)pos.y] = type;
		}
		//_regions[pos] = _currentRegion;
	}
	bool canCarve(Vector2 pos,Vector2 dir){
		if(pos.x + dir.x * 3 >= stage.size.x || pos.x + dir.x *3 <= 0 || pos.y + dir.y * 3 >= stage.size.y || pos.y + dir.y * 3<= 0)
			return false;

		return stage.tileType[(int)(pos.x + dir.x +dir.x),(int)(pos.y + dir.y +dir.y)] == TileType.Empty;


	}
	void growMaze (Vector2 start)
	{
		 Vector2 lastDir = new Vector2(-1,-1);

		
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
				Debug.Log(2);
				Debug.Log(unmadeCells.Contains(lastDir));
				if(unmadeCells.Contains(lastDir) && Random.Range(0,101)> windingPercent){
					dir = lastDir;
				}else{
					dir = unmadeCells[Random.Range(0,unmadeCells.Count)];
				}
				carve(new Vector2(cell.x + dir.x,cell.y + dir.y),TileType.Ground);
				carve(new Vector2(cell.x + dir.x + dir.x ,cell.y + dir.y + dir.y),TileType.Ground);
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
		rooms = new ArrayList();

		SpriteProvider spriteProvider = new SpriteProvider(AssetDatabase.GetAssetPath(dungeon.texture2D));
		stage = new Stage(dungeon.size);
		stage.FillStage();

		AddRooms();
		foreach (Room room in rooms) {
			stage.AddRoom(room);
		}

		for (int i = 0; i < dungeon.size.y; i++) {
			for (int j = 0; j < dungeon.size.x; j++) {
				if(stage.tileType[j,i]!=TileType.Test)
					continue;
				growMaze(new Vector2(j,i));

			}
		}






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
}
