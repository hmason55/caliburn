using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Growables : MonoSingleton<Growables> {

    public List<GameObject> plants;
    public Dictionary<int, Growable> growableData;

    void Awake() {
        LoadGrowables();

        foreach(GameObject plant in plants) {
            ClientScene.RegisterPrefab(plant);
        }
    }

    void LoadGrowables() {
        // Hardcoded for now
        growableData = new Dictionary<int, Growable> {
            { 0, new Growable { name = "Daisy", growableId = 0, radius = 0, duration = 1800 } },
            { 1, new Growable { name = "Plant2", growableId = 1, radius = 0, duration = 1800 } },
        };
    }
}
