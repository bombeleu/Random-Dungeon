using UnityEditor;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;

[CustomEditor(typeof(Dungeon))]
public class DungeonEditor : Editor {
    
    public Dungeon dungeon;

	private SpriteProvider spriteProvider;

	private Dictionary<int,GameObject> tilePool = new Dictionary<int, GameObject>();


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
	void OnEnable()
	{
		dungeon = target as Dungeon;
		dungeon.level = new Level();
		Tools.current = Tool.View;
		tilePool = new Dictionary<int, GameObject>();
		if (dungeon.texture2D)
		{
			spriteProvider = new SpriteProvider(AssetDatabase.GetAssetPath(dungeon.texture2D));
		}
		
	}

	void BuildDungeon()
	{
		Level level = new Level();
		StageGenerator generator = new StageGenerator(level);
		float startTime = Time.realtimeSinceStartup;
		Stage stage = generator.GenerateStage();
		dungeon.stage = stage;
		
		float endTime = Time.realtimeSinceStartup;
		Debug.Log ("End :"+ (endTime-startTime));
		
		int index = 0;
		float y = 0, x = 0;

		for (int i = 0; i < dungeon.level.levelSize.y; i++) {
			for (int j = 0; j < dungeon.level.levelSize.x; j++) {
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


}