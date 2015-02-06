using UnityEngine;
using System.Collections;


public class Tile : MonoBehaviour {

	public string tileName;
	public Vector2 tileSize = new Vector2();   
    public SpriteRenderer renderer2D;
    public int id;
    public TileType tileType;


 

    public static Tile CreateTile(TileType tileType,Vector2 position,SpriteProvider spriteProvider,int id){
		Tile tile = null;
		GameObject go = new GameObject("Tile");

		tile = go.AddComponent<Tile>();

		tile.id = id;
		tile.gameObject.transform.position = position;		
		tile.tileType = tileType;   
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
				tileName = "Wall "+id;
				this.gameObject.name = tileName;  
				renderer2D.sprite = spriteProvider.GetSprite("Wall");
				this.tileSize = new Vector3(renderer2D.sprite.textureRect.width/100, renderer2D.sprite.textureRect.height/100);           
				break;
            case TileType.Ground:
				tileName = "Ground "+id;
				this.gameObject.name = tileName;  
				renderer2D.sprite = spriteProvider.GetSprite("Ground");
				this.tileSize = new Vector3(renderer2D.sprite.textureRect.width/100, renderer2D.sprite.textureRect.height/100); 
				break;
			case TileType.Test:
				tileName = "Test "+id;
				this.gameObject.name = tileName;  
				renderer2D.sprite = spriteProvider.GetSprite("Test");
				this.tileSize = new Vector3(renderer2D.sprite.textureRect.width/100, renderer2D.sprite.textureRect.height/100); 
				break;
		case TileType.Empty:
			tileName = "Empty "+id;
			this.gameObject.name = tileName;  
			renderer2D.sprite = spriteProvider.GetSprite("Wall");
			this.tileSize = new Vector3(renderer2D.sprite.textureRect.width/100, renderer2D.sprite.textureRect.height/100); 
			break;
		case TileType.OpenDoor:
			tileName = "OpenDoor "+id;
			this.gameObject.name = tileName;  
			renderer2D.sprite = spriteProvider.GetSprite("OpenDoor");
			this.tileSize = new Vector3(renderer2D.sprite.textureRect.width/100, renderer2D.sprite.textureRect.height/100); 
			break;
		case TileType.ClosedDoor:
			tileName = "ClosedDoor"+id;
			this.gameObject.name = tileName;  
			renderer2D.sprite = spriteProvider.GetSprite("ClosedDoor");
			this.tileSize = new Vector3(renderer2D.sprite.textureRect.width/100, renderer2D.sprite.textureRect.height/100); 
			break;
		}            
    }
}
