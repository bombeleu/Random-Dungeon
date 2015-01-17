using UnityEditor;
using System.Linq;
using UnityEngine;
using System.Collections;
[CustomEditor(typeof(Dungeon))]
public class DungeonEditor : Editor {
    
    public Dungeon dungeon;

	private SpriteProvider spriteProvider;
   
	public int numRooms = 10;
	public int roomExtraSize = 2;
	public ArrayList rooms = new ArrayList();

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


    void BuildDungeon()
    {
		SpriteProvider spriteProvider = new SpriteProvider(AssetDatabase.GetAssetPath(dungeon.texture2D));
		Stage stage = new Stage(dungeon.size);
		stage.FillStage();

		AddRooms();
		foreach (Room room in rooms) {
			stage.AddRoom(room);
		}













		int index = 0;
		float y = 0, x = 0;
		for (int i = 0; i < dungeon.size.y; i++) {
			for (int j = 0; j < dungeon.size.x; j++) {
				Tile tile = Tile.CreateTile((TileType)stage.tileType[i,j],new Vector2(x, y),spriteProvider);
				tile.id = index++;
				x += tile.tileSize.x;
				x = Mathf.Round(x*100)/100;
				tile.gameObject.transform.parent = dungeon.transform;
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

			var x = Random.Range(0,(dungeon.size.x - width) / 2) * 2 + 1;
			var y = Random.Range(0,(dungeon.size.y - height) / 2) * 2 + 1;
			
			Room room = new Room(new Vector2(x, y), width, height);
			
			bool overlaps = false;
			/*
			foreach (Room room in rooms) {
				if (room.distanceTo(other) <= 0) {
					overlaps = true;
					break;
				}
			}

			
			if (overlaps) continue;
			*/
			rooms.Add(room);
			
			//_startRegion();

		}





	}



		void OnEnable()
    {
        dungeon = target as Dungeon;
        Tools.current = Tool.View;

        if (dungeon.texture2D)
        {

            spriteProvider = new SpriteProvider(AssetDatabase.GetAssetPath(dungeon.texture2D));

		}
    }
}
