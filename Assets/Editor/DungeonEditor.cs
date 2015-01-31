using UnityEditor;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Dungeon))]
public class DungeonEditor : Editor {
    
    public Dungeon dungeon;

	private SpriteProvider spriteProvider;

	private Dictionary<int,GameObject> tilePool = new Dictionary<int, GameObject>();
	public int windingPercent = 70;
	public int numRooms = 20;
	public int roomExtraSize = 2;
	public ArrayList rooms = new ArrayList();

	public Stage stage;


	public int extraConnectorChance = 20;

	public Dictionary<IntVector2,int> _regions;
	public int currentRegion = -1;

	public List<IntVector2> connectors;

	public override void OnInspectorGUI(){
		GUILayout.BeginVertical();
		EditorGUILayout.Vector2Field("Dungeon Size: ", new Vector2((int)dungeon.size.x,(int)dungeon.size.y));    
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
	void carve(IntVector2 pos, TileType type) {
		stage.SetTile(pos,type);
		_regions[pos]= currentRegion;
	}

	bool canCarve(IntVector2 pos,IntVector2 dir){
		if(pos.x + dir.x * 3 > stage.size.x || pos.x + dir.x *3 < 0 || pos.y + dir.y * 3 > stage.size.y || pos.y + dir.y * 3< 0)
			return false;

		return stage.GetTile(pos + dir * 2) == TileType.Wall;
	}
	void growMaze (IntVector2 start)
	{
		List<IntVector2> cells = new List<IntVector2>();
		IntVector2 lastDir = new IntVector2(-10,-1);
		
		startRegion();
		carve(start,TileType.Ground);
		
		cells.Add(start);
		while (cells.Count != 0) {
			IntVector2 cell = cells[cells.Count-1];
			List<IntVector2> openDirs = new List<IntVector2>();
			IntVector2[] directions = Directions.cardinal;
			for (int i = 0; i < directions.Length; i++) {
					if(canCarve(cell,directions[i]))
						openDirs.Add(directions[i]);
			}
			if(openDirs.Count !=0){
				IntVector2 dir;
				if(openDirs.Contains(lastDir) && Random.Range(0,101)> windingPercent){
					dir = lastDir;
				}else{
					dir = openDirs[Random.Range(0,openDirs.Count)];
				}
				carve(cell + dir,TileType.Ground);
				carve(cell + dir * 2,TileType.Ground);
				cells.Add(cell + dir * 2);
				lastDir = dir;
			}else{
				cells.RemoveAt(cells.Count-1);

				lastDir =  new IntVector2(-10,-10);
			}

		}

	}


    void BuildDungeon()
    {
		float startTime = Time.realtimeSinceStartup;
		currentRegion = -1;
		rooms = new ArrayList();
		stage = new Stage(dungeon.size);
		stage.FillStage(TileType.Wall);

		_regions = new Dictionary<IntVector2,int >();

		AddRooms();



		for (int i = 1; i < dungeon.size.y; i+=2) {
			for (int j = 1; j < dungeon.size.x; j+=2) {
				if(stage.tileType[j,i]!=TileType.Wall)
					continue;
				growMaze(new IntVector2(j,i));
			}
		}

		connectRegions();
		removeDeadEnds();


		float endTime = Time.realtimeSinceStartup;
		Debug.Log ("End :"+ (endTime-startTime));

		int index = 0;
		float y = 0, x = 0;

		for (int i = 0; i < dungeon.size.y; i++) {
			for (int j = 0; j < dungeon.size.x; j++) {
				GameObject ob;
				tilePool.TryGetValue(index,out ob);
				if (ob==null)
				{
					Tile tile = Tile.CreateTile((TileType)stage.tileType[j,i],new Vector2(x, y),spriteProvider,index,(connectors.Contains(new IntVector2(j, i))));
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
		
		for (int i = 0; i < numRooms;) {
			var size = Random.Range(1, 3 + roomExtraSize) * 2 + 1;
			var rectangularity = Random.Range(0, 1 + size / 2) * 2;
			var width = size;
			var height = size;
			if (Random.Range(1,3)==2) {
				width += rectangularity;
			} else {
				height += rectangularity;
			}

			int x = Random.Range(0,((int)dungeon.size.x - width)/2)*2+1;
			int y = Random.Range(0,((int)dungeon.size.y - height)/2)*2+1;
			
			Room room = new Room(new IntVector2(x, y), width, height);

			bool overlaps = false;

			foreach (Room other in rooms) {
				if (room.Intersects(other)) {
					overlaps = true;
					break;
				}
			}

			
			if (overlaps) continue;
			i++;
			rooms.Add(room);
			startRegion();
			for (int j = y;j < y+ height; j++) {
				for (int ii = x; ii < x+ width; ii++) {
					carve(new IntVector2(ii,j),TileType.Ground);
				}
			}
			


		}
	}
	void startRegion() {
		currentRegion++;
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

	void addJunction(IntVector2 pos) {
		if (Random.Range(1,5)==1) {
			stage.SetTile(pos,Random.Range(1,4)==1 ? TileType.OpenDoor : TileType.Ground);
		} else {
			stage.SetTile(pos, TileType.ClosedDoor);
		}
	}






	
	
	void connectRegions() {


		Dictionary<IntVector2,List<int>> connectorRegions = new Dictionary<IntVector2, List<int>>();
		for (int i = 1; i < dungeon.size.y-1; i++) {
			for (int j = 1; j < dungeon.size.x-1; j++) {
				IntVector2 pos = new IntVector2(j,i);
				if(stage.GetTile(pos)!=TileType.Wall) continue;
				List<int> regions = new List<int>();

				foreach (IntVector2 dir in Directions.cardinal) {
					IntVector2 region = new IntVector2(pos.x+dir.x,pos.y+dir.y);
					if(_regions.ContainsKey(region)){
						if(!regions.Contains(_regions[region])){
							regions.Add(_regions[region]);
						}

					}

				}
				if(regions.Count < 2) continue;
				regions.Sort();
				connectorRegions[pos]=regions;

			}
		}
		connectors = connectorRegions.Select(x=>x.Key).ToList();

		int openRegions = currentRegion;

	


		while (openRegions >= 1) {
			IntVector2 connector = connectors[Random.Range(0, connectors.Count)];
			addJunction(connector);
			openRegions--;
		
			List<IntVector2> source = new List<IntVector2>();
			for (int i = 0; i < connectors.Count; i++) {
				if(connectorRegions[connector][0]==connectorRegions[connectors[i]][0]){
					source.Add(connectors[i]);
				}
			}

			foreach (IntVector2 item in source) {
				if(connectors.Count >= 2)
					connectors.Remove(item);
			}
		}

	}



	void removeDeadEnds() {
		bool done = false;

		while (!done) {
			done = true;
			
			for (int i = 1; i < dungeon.size.y-1; i++) {
				for (int j = 1; j < dungeon.size.x-1; j++) {
					IntVector2 pos = new IntVector2(j,i);
					if(stage.GetTile(pos)==TileType.Wall) continue;
				
				int exits = 0;
					foreach (IntVector2 dir in Directions.cardinal) {
						if(stage.GetTile(pos+dir)!=TileType.Wall) exits++;
				}
				
				if (exits != 1) continue;
				done = false;
					stage.SetTile(pos, TileType.Wall);
				}}
		}
	}



}