using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;


public class SpriteProvider{


	public Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();


	public SpriteProvider(string path){
		Sprite[] spriteReferences = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();
		for (int i = 0; i < spriteReferences.Length; i++) {
			sprites.Add(spriteReferences[i].name,spriteReferences[i]);
		}

	}
	public Sprite GetSprite(string name){
		return sprites[name];
	}

}
