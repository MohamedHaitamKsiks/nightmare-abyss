using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Recipe
{
    public HashSet<string> Items;
    public string Result;
}


public class CraftManager : MonoBehaviour
{

    public static CraftManager Instance{get; private set;} = null;

    [SerializeField] private Recipe[] recipes;
    private readonly Dictionary<HashSet<string>, string> crafts = new();

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        foreach (var recipe in recipes)
        {
            crafts[recipe.Items] = recipe.Result;
        }
    }

    private static string GetCraftOf(HashSet<string> items)
    {
        return Instance.crafts[items];
    }

    private static Recipe[] GetRecipes()
    {
        return Instance.recipes;
    }
}
