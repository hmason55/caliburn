using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Tilemaps;

public class PlayerSpawnSoilRequest : MessageBase {
    public string ownerId;
    public uint networkId;
    public int soilId;
    public int soilLayer;
    public Vector2 position;
    public bool complete = false;

    public void HandleRequest() {
        //GameObject go = NetworkManager.Instantiate(Soils.Instance.soils[soilId], position, Quaternion.identity) as GameObject;

        //TileBase tile = World.Instance.tilemaps[terrainLayer].GetTile(new Vector3Int((int)position.x, (int)position.y, 0));
        //if(tile == null) {return;}

        //TileBase.GetTileData(new Vector3Int((int)position.x, (int)position.y, 0), World.Instance.tilemaps[terrainLayer], ref tileData);
        //PlantView plantView = go.GetComponent<PlantView>();
        /*
        plantView.Validate((valid) => {
            if(valid) {
                NetworkManager.Destroy(go.GetComponent<Rigidbody2D>());
                NetworkIdentity identity = go.GetComponent<NetworkIdentity>();
                System.Guid prefabAssetId = identity.assetId;
                
                plantView.HandleRequest(this);
                ClientScene.RegisterSpawnHandler(prefabAssetId, OnSpawnGrowable, OnUnSpawnGrowable);
                NetworkServer.Spawn(go, prefabAssetId);

                Server.Instance.soilDataByNetId.Add(identity.netId, CreateSoil());
                complete = true;
            } else {
                NetworkManager.Destroy(go);
                complete = true;
            }
        });
        */
    }

    public void HandleRequestReceived() {
        Vector3Int gridPos = new Vector3Int((int)position.x, (int)position.y, 0);

        int[] proximityCollisionLayers = new int[] {
            6,
        };

        // Check 8 neighbor tiles for a collision layer
        for(int x = gridPos.x-1; x <= gridPos.x+1; x++) {
            for(int y = gridPos.y-1; y <= gridPos.y+1; y++) {
                Tile tile = World.Instance.tilemaps[soilLayer].GetTile(new Vector3Int(x, y, 0)) as Tile;
                RuleTile ruleTile = World.Instance.tilemaps[soilLayer].GetTile(new Vector3Int(x, y, 0)) as RuleTile;
                if(tile == null && ruleTile == null) {Debug.Log("Can't place soil without terrain."); return;}

                for(int i = 0; i < proximityCollisionLayers.Length; i++) {
                    Tile collision = World.Instance.tilemaps[proximityCollisionLayers[i]].GetTile(new Vector3Int(x, y, 0)) as Tile;
                    if(collision != null) {Debug.Log("There is a collision here."); return;}
                }
            }
        }

        GameObject go = NetworkManager.Instantiate(Soils.Instance.soils[soilId], position, Quaternion.identity) as GameObject;
        SoilView soilView = go.GetComponent<SoilView>();

        soilView.Validate((valid) => {
            if(valid) {
                NetworkManager.Destroy(go.GetComponent<Rigidbody2D>());
                NetworkIdentity identity = go.GetComponent<NetworkIdentity>();
                System.Guid prefabAssetId = identity.assetId;
                
                soilView.HandleRequest(this);
                ClientScene.RegisterSpawnHandler(prefabAssetId, OnSpawnSoil, OnUnSpawnSoil);
                NetworkServer.Spawn(go, prefabAssetId);

                Server.Instance.soilDataByNetId.Add(identity.netId, CreateSoil());

                SpawnSoilMessage spawnMessage = new SpawnSoilMessage {
                    networkId = identity.netId,
                    soilLayer = 0,
                    position = position,
                    soilId = 0,
                };
                spawnMessage.HandleRequest();

                // Place proximity tiles
                for(int x = gridPos.x-1; x <= gridPos.x+1; x++) {
                    for(int y = gridPos.y-1; y <= gridPos.y+1; y++) {
                        World.Instance.tilemaps[soilLayer].SetTile(new Vector3Int(x, y, 0), Soils.Instance.soilTiles[soilId]); 
                    }
                }

                complete = true;
            } else {
                NetworkManager.Destroy(go);
                complete = true;
            }
        });


    }

    GameObject OnSpawnSoil(Vector3 position, System.Guid assetId) {
        return null;
    }

    void OnUnSpawnSoil(GameObject spawned) {

    }

    SoilData CreateSoil() {
        return new SoilData { 
            soilId = soilId,
            layer = soilLayer,
            posX = (int)position.x,
            posY = (int)position.y,
        };
    }
}
