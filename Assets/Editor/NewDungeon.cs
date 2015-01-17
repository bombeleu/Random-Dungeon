using UnityEngine;
using UnityEditor;
using System.Collections;

public class NewDungeon : EditorWindow
{
    [MenuItem("GameObject/Dungeon Generator")]
    public static void Init()
    {
        GameObject go = new GameObject("Dungeon");
        go.AddComponent<Dungeon>();
        GameObject gi = new GameObject("Tiles");
        gi.transform.parent = go.transform;
    }    
}
