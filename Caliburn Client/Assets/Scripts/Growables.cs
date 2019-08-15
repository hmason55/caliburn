using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Growables : MonoSingleton<Growables> {
    public List<GameObject> prefabs;
    public Dictionary<string, GameObject> prefabsById;
    public Dictionary<string, Growable> growablesById;
    
    void Awake() {
        LoadGrowables();
        LoadPrefabs();
    }

    void LoadGrowables() {
        // Hardcoded for now, load this later from the database.
        growablesById = new Dictionary<string, Growable> {
            { "Daisy", new Growable { growableId = "Daisy", duration = 60, growthStages = 2 } },
            { "other", new Growable { growableId = "other", duration = 60, growthStages = 2 } },
        };
    }
    
    void LoadPrefabs() {
        prefabsById = new Dictionary<string, GameObject>();
        foreach(GameObject prefab in prefabs) {
            if(growablesById.ContainsKey(prefab.name)) {
                prefabsById.Add(prefab.name, prefab);
                ClientScene.RegisterPrefab(prefab); 
            }
        }
    }
}
