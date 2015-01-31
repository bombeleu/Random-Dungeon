using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	public string name;
	public bool isPassable;
	public bool isTransparent;
	TileType opensTo;
	TileType closesTo;
	public bool connector = false;


  
    public Vector2 tileSize = new Vector2();   
    public SpriteRenderer renderer2D;
    public int id;
    public TileType tileType;



    public void OnDrawGizmos()
    {
		if (!connector)
        	Gizmos.color = Color.red;
		else 
			Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, tileSize);
    }    

    public static Tile CreateTile(TileType tileType,Vector2 position,SpriteProvider spriteProvider,int id,bool connector){
		Tile tile = null;
		GameObject go = new GameObject("Tile");

		tile = go.AddComponent<Tile>();
		tile.connector = connector;
		tile.id = id;
		tile.gameObject.transform.position = position;		
		tileType = tileType;   
		tile.renderer2D = go.AddComponent<SpriteRenderer>();            
		tile.SwitchTileState(tileType,spriteProvider);  
		return tile;
    }

    public void Destroy()
    {
        DestroyImmediate(gameObject);
    }

	public void SwitchTileState(TileType tileType,SpriteProvider spriteProvider){
		switch (tileType)
        {
            case TileType.Wall:
				name = "Wall "+id;
				this.gameObject.name = name;  
				renderer2D.sprite = spriteProvider.GetSprite("Wall");
				this.tileSize = new Vector3(renderer2D.sprite.textureRect.width/100, renderer2D.sprite.textureRect.height/100);           
				break;
            case TileType.Ground:
				name = "Ground "+id;
				this.gameObject.name = name;  
				renderer2D.sprite = spriteProvider.GetSprite("Ground");
				this.tileSize = new Vector3(renderer2D.sprite.textureRect.width/100, renderer2D.sprite.textureRect.height/100); 
				break;
			case TileType.Test:
				name = "Test "+id;
				this.gameObject.name = name;  
				renderer2D.sprite = spriteProvider.GetSprite("Test");
				this.tileSize = new Vector3(renderer2D.sprite.textureRect.width/100, renderer2D.sprite.textureRect.height/100); 
				break;
		case TileType.Empty:
			name = "Empty "+id;
			this.gameObject.name = name;  
			renderer2D.sprite = spriteProvider.GetSprite("Wall");
			this.tileSize = new Vector3(renderer2D.sprite.textureRect.width/100, renderer2D.sprite.textureRect.height/100); 
			break;
		case TileType.OpenDoor:
			name = "OpenDoor "+id;
			this.gameObject.name = name;  
			renderer2D.sprite = spriteProvider.GetSprite("OpenDoor");
			this.tileSize = new Vector3(renderer2D.sprite.textureRect.width/100, renderer2D.sprite.textureRect.height/100); 
			break;
		case TileType.ClosedDoor:
			name = "ClosedDoor"+id;
			this.gameObject.name = name;  
			renderer2D.sprite = spriteProvider.GetSprite("ClosedDoor");
			this.tileSize = new Vector3(renderer2D.sprite.textureRect.width/100, renderer2D.sprite.textureRect.height/100); 
			break;
		}            
    }
}
