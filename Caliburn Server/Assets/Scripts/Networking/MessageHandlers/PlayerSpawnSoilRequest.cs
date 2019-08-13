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
        Vector3Int gridPos = new Vector3Int((int)position.x, (int)position.y, 0);

        // Check 8 neighbor tiles for a collision layer
        //if(HasProximityCollision(gridPos, 1, new int[] {6})) {return;}

        GameObject go = NetworkManager.Instantiate(Soils.Instance.soils[soilId], position, Quaternion.identity) as GameObject;
        SoilView soilView = go.GetComponent<SoilView>();

        //soilView.Validate((valid) => {
        //    if(valid) {
                NetworkManager.Destroy(go.GetComponent<Rigidbody2D>());
                NetworkIdentity identity = go.GetComponent<NetworkIdentity>();
                System.Guid prefabAssetId = identity.assetId;
                
                soilView.HandleRequest(this);
                ClientScene.RegisterSpawnHandler(prefabAssetId, OnSpawnSoil, OnUnSpawnSoil);
                NetworkServer.Spawn(go, prefabAssetId);

                Server.Instance.soilDataByNetId.Add(identity.netId, CreateSoil());

                SpawnSoilMessage spawnMessage = new SpawnSoilMessage {
                    networkId = identity.netId,
                    soilId = soilId,
                    soilLayer = soilLayer,
                    position = position,
                };
                spawnMessage.HandleRequest();

                PlaceProximityTiles(gridPos, 1);

        //        complete = true;
        //    } else {
        //       NetworkManager.Destroy(go);
        //        complete = true;
        //    }
        //});
    }

    public void HandleRequestReceived() {
        Vector3Int gridPos = new Vector3Int((int)position.x, (int)position.y, 0);

        // Check 8 neighbor tiles for a collision layer
        if(HasProximityCollision(gridPos, 1, new int[] {6})) {return;}

        GameObject go = NetworkManager.Instantiate(Soils.Instance.soils[soilId], position, Quaternion.identity) as GameObject;
        SoilView soilView = go.GetComponent<SoilView>();

        soilView.Validate((valid) => {
            if(valid) {

                ProcessSoil.Instance.SpawnRequest(this, (requestCode) => {
                    Debug.Log("Request completed with a request code: " + requestCode);
                    switch(requestCode) {
                        case 0:
                            NetworkManager.Destroy(go.GetComponent<Rigidbody2D>());
                            NetworkIdentity identity = go.GetComponent<NetworkIdentity>();
                            System.Guid prefabAssetId = identity.assetId;
                            
                            soilView.HandleRequest(this);
                            ClientScene.RegisterSpawnHandler(prefabAssetId, OnSpawnSoil, OnUnSpawnSoil);
                            NetworkServer.Spawn(go, prefabAssetId);

                            Server.Instance.soilDataByNetId.Add(identity.netId, CreateSoil());

                            SpawnSoilMessage spawnMessage = new SpawnSoilMessage {
                                networkId = identity.netId,
                                soilId = soilId,
                                soilLayer = soilLayer,
                                position = position,
                            };
                            spawnMessage.HandleRequest();

                            PlaceProximityTiles(gridPos, 1);
                        break;
                    }
                });
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

    bool HasProximityCollision(Vector3Int pos, int radius, int[] collisionLayers) {
        for(int x = pos.x-radius; x <= pos.x+radius; x++) {
            for(int y = pos.y-radius; y <= pos.y+radius; y++) {
                Tile tile = World.Instance.tilemaps[soilLayer].GetTile(new Vector3Int(x, y, 0)) as Tile;
                RuleTile ruleTile = World.Instance.tilemaps[soilLayer].GetTile(new Vector3Int(x, y, 0)) as RuleTile;
                if(tile == null && ruleTile == null) {Debug.Log("Can't place soil without terrain."); return true;}

                for(int i = 0; i < collisionLayers.Length; i++) {
                    Tile collision = World.Instance.tilemaps[collisionLayers[i]].GetTile(new Vector3Int(x, y, 0)) as Tile;
                    if(collision != null) {Debug.Log("There is a collision here."); return true;}
                }
            }
        }

        return false;
    }

    void PlaceProximityTiles(Vector3Int pos, int radius) {
        // Place proximity tiles
        for(int x = pos.x-radius; x <= pos.x+radius; x++) {
            for(int y = pos.y-radius; y <= pos.y+radius; y++) {
                World.Instance.tilemaps[soilLayer].SetTile(new Vector3Int(x, y, 0), Soils.Instance.soilTiles[soilId]); 
            }
        }
    }
}
