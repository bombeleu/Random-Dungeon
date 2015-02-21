using UnityEngine;
using System.Collections;
using Assets.Scripts;
using System.Collections.Generic;
using UnityEditor;

public class GameBuilder : MonoBehaviour {

	public Dungeon dungeon;

	public Stage stage;

	public Transform hero;
	private SpriteProvider spriteProvider;
	
	private Dictionary<int,GameObject> tilePool = new Dictionary<int, GameObject>();

	// Use this for initialization
	void Start () {
		dungeon = GameObject.FindGameObjectWithTag("Dungeon").GetComponent<Dungeon>();
		spriteProvider = new SpriteProvider(AssetDatabase.GetAssetPath(dungeon.texture2D));
		stage = BuildStage();

		GameObject go = Instantiate(Resources.Load("Hero")) as GameObject; 
		Hero hero = go.GetComponent<Hero>();
		IntVector2 heroPos = stage.findOpenTile();
		go.transform.position = heroPos;
		this.hero = go.transform;
		hero.pos = heroPos;
		stage.actors.Add(heroPos,hero);
		stage.hero = hero;
		
		
		
		Display();
	}
	

	public Stage BuildStage(){
		Level level = new Level();
		StageGenerator generator = new StageGenerator(level);
		float startTime = Time.realtimeSinceStartup;
		Stage stage = generator.GenerateStage();
		dungeon.stage = stage;
		float endTime = Time.realtimeSinceStartup;
		//Debug.Log ("End :"+ (endTime-startTime));
		return stage;
	}
	public void Display(){
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
				if(stage.hero.pos == new IntVector2(j,i))
					hero.position = new Vector2(x, y);
				
				
				x += dungeon.tileSize.x/100;
				x = Mathf.Round(x*100)/100;
				
				index++;
			}
			
			y += dungeon.tileSize.y/100;
			y = Mathf.Round(y*100)/100;
			x = 0;
		}

		AstarPath.active.Scan();
	}
}
