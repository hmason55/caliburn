using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class World : MonoSingleton<World> {
    public List<Tilemap> tilemaps;

    void Awake() {
        tilemaps = new List<Tilemap>(GetComponentsInChildren<Tilemap>());
    }
}
