using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class SoilView : MonoBehaviour {
    public string ownerId;
    public Vector2 position;
    public Soil soil;

    CircleCollider2D circleCollider2D;

    bool valid = true;

    bool render = true;

    void Awake() { 
        circleCollider2D = GetComponent<CircleCollider2D>();
        Destroy(GetComponent<Rigidbody2D>());
    }

    void Start() {
        if(render) {
            RequestSoilData();
        }
    }

    void RequestSoilData() {
        SoilDataRequest soilData = new SoilDataRequest {
            networkId = GetComponent<NetworkIdentity>().netId
        };
        soilData.HandleRequest();
    }

    public void UpdateSoilData(SoilDataRequest request) {
        soil = Soils.Instance.soilData[request.soilId];
        position = request.position;

        Vector3Int gridPos = new Vector3Int((int)position.x, (int)position.y, 0);
        for(int x = gridPos.x-1; x <= gridPos.x+1; x++) {
            for(int y = gridPos.y-1; y <= gridPos.y+1; y++) {
                World.Instance.tilemaps[soil.layer].SetTile(new Vector3Int(x, y, 0), Soils.Instance.soilTiles[soil.soilId]);
            }
        }
    }
}
