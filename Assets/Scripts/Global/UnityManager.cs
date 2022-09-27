using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public enum SpriteFolder
{
    [StringValue("Terrain")]    Terrain,
    [StringValue("Towers")]     Towers,
    [StringValue("UI")]         UI,
    [StringValue("")]           All,
}


public static class UnityManager
{
    public static GameObject GetPrefab(string name) => 
        Resources.Load<GameObject>($"Prefabs/{name}");

    public static IEnumerable<GameObject> GetAllPrefabs() =>
        Resources.LoadAll<GameObject>("Prefabs/");

    public static IEnumerable<GameObject> GetAllPrefabsOfTag(string tag) =>
        GetAllPrefabs().Where(x => x.tag == tag);

    public static IEnumerable<Sprite> GetAllSprites(SpriteFolder folder) =>
        Resources.LoadAll<Sprite>($"Sprites/{folder.GetStringValue()}");
    public static IEnumerable<Sprite> GetAllSprites(string path) =>
        Resources.LoadAll<Sprite>($"Sprites/{path}");

    public static Sprite GetSprite(SpriteFolder folder, string name) =>
        folder == SpriteFolder.All
        ? GetAllSprites(SpriteFolder.All).First(x => x.name == name)
        : Resources.Load<Sprite>($"Sprites/{folder.GetStringValue()}/{name}");
    public static Sprite GetSprite(string folder, string name) =>
        folder == ""
        ? GetAllSprites(SpriteFolder.All).First(x => x.name == name)
        : Resources.Load<Sprite>($"Sprites/{folder}/{name}");
}
