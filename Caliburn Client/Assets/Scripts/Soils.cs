using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Tilemaps;

public class Soils : MonoSingleton<Soils> {
    public List<GameObject> soils;
    public List<RuleTile> soilTiles;
    public Dictionary<int, Soil> soilData;
    
    void Awake() {
        LoadSoils();

        foreach(GameObject soil in soils) {
            ClientScene.RegisterPrefab(soil);
        }
    }

    void LoadSoils() {
        // Hardcoded for now
        soilData = new Dictionary<int, Soil> {
            { 0, new Soil { name = "Dirt Soil", soilId = 0, layer = 0, radius = 0} },
        };
    }
}
