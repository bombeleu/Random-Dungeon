using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Dungeon : MonoBehaviour {
    
    private static Dungeon dungeon;

	public IntVector2 size = new IntVector2(65,65);
    public Texture2D texture2D;
   
    public Vector2 tileSize = new Vector2();
    public Vector2 gridSize;
	public Level level = new Level();


		
    void OnDrawGizmosSelected()
    {
        if (texture2D)
        {            
            Gizmos.color = Color.grey;
			var tile = new Vector2(tileSize.x / 100, tileSize.y / 100);
			float y = 0, x = 0;
			for (int i = 0; i < size.y; i++) {
				for (int j = 0; j < size.x; j++) {
					Gizmos.DrawWireCube(new Vector2(x, y), tile);
					x += tile.x;
					x = Mathf.Round(x*100)/100;
				}
				y += tile.y;
				y = Mathf.Round(y*100)/100;
				x = 0;
			}

        }
    }
    
}
