using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	public string name;
	public bool isPassable;
	public bool isTransparent;
	TileType opensTo;
	TileType closesTo;



  
    public Vector2 tileSize = new Vector2();   
    public SpriteRenderer renderer2D;
    public int id;
    public TileType tileType;



    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, tileSize);
    }    

    public static Tile CreateTile(TileType tileType,Vector2 position,SpriteProvider spriteProvider){
		Tile tile = null;
		GameObject go = new GameObject("Tile");                         
		tile = go.AddComponent<Tile>();
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
				name = "Wall";
				this.gameObject.name = name;  
				renderer2D.sprite = spriteProvider.GetSprite(name);
				this.tileSize = new Vector3(renderer2D.sprite.textureRect.width/100, renderer2D.sprite.textureRect.height/100);           
				break;
            case TileType.Ground:
				name = "Ground";
				this.gameObject.name = name;  
				renderer2D.sprite = spriteProvider.GetSprite(name);
				this.tileSize = new Vector3(renderer2D.sprite.textureRect.width/100, renderer2D.sprite.textureRect.height/100); 
				break;
		}            
    }
}
