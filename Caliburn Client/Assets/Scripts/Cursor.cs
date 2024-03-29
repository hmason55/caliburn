﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Tilemaps;

public class Cursor : MonoSingleton<Cursor> {
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer boundsRenderer;
    Rigidbody2D rigidbody2D;
    List<GameObject> collisions;
    CircleCollider2D collider2D;
    public string usage;
    bool proximityCollisions = false;
    
    bool allowAction = false;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<CircleCollider2D>();
        collisions = new List<GameObject>();
    }

    void FixedUpdate() {
        EvaluateCollisions();
    }

    void EvaluateCollisions() {
        int soil = 0;
        int growable = 0;
        int terrain = 0;

        foreach(GameObject collision in collisions) {
            switch(LayerMask.LayerToName(collision.layer)) {
                case "Soil":
                    soil++;
                break;

                case "Growable":
                    growable++;
                break;

                case "Terrain Collision":
                    terrain++;
                break;
            }
        }

        switch(usage) {
            case "plant":
                gameObject.layer = LayerMask.NameToLayer("Growable");
                proximityCollisions = false;
                if( soil == 1 && growable + terrain == 0) {
                    allowAction = true;
                } else {
                    allowAction = false;
                }
            break;

            case "till":
                CheckProximityCollisions(1);
                gameObject.layer = LayerMask.NameToLayer("Soil");
                if(collisions.Count == 0 && !proximityCollisions) {
                    allowAction = true;
                } else {
                    allowAction = false;
                }
            break;

            case "water":
                gameObject.layer = LayerMask.NameToLayer("Growable");
                proximityCollisions = false;
                if( growable == 1 && terrain == 0) {
                    allowAction = true;
                } else {
                    allowAction = false;
                }
            break;
        }

        if(allowAction) {
            boundsRenderer.color = new Color(0f, 1f, 0f, 0.4f);
        } else {
            boundsRenderer.color = new Color(1f, 0f, 0f, 0.4f);
        }
    }

    public void CheckProximityCollisions(int radius) {
        int layer = 0;
        switch(LayerMask.LayerToName(gameObject.layer)) {
            case "Soil":
                layer = 0;
            break;
        }

        int[] proximityCollisionLayers = new int[] {
            6,
        };

        Vector3Int gridPos = new Vector3Int((int)transform.position.x, (int)transform.position.y, 0);
        for(int x = gridPos.x-radius; x <= gridPos.x+radius; x++) {
            for(int y = gridPos.y-radius; y <= gridPos.y+radius; y++) {
                Tile tile = World.Instance.tilemaps[layer].GetTile(new Vector3Int(x, y, 0)) as Tile;
                RuleTile ruleTile = World.Instance.tilemaps[layer].GetTile(new Vector3Int(x, y, 0)) as RuleTile;
                if(tile == null && ruleTile == null) {

                    proximityCollisions = true;
                    //Debug.Log("Can't place soil without terrain."); 
                    return;
                }

                for(int i = 0; i < proximityCollisionLayers.Length; i++) {
                    Tile collision = World.Instance.tilemaps[proximityCollisionLayers[i]].GetTile(new Vector3Int(x, y, 0)) as Tile;
                    if(collision != null) {
                        proximityCollisions = true;
                        //Debug.Log("There is a collision here."); 
                        return;
                    }
                }
            }
        }

        proximityCollisions = false;
    }

    public void Water() {
        if(!allowAction) {
            Debug.Log("Can't water here.");
            return;
        }

        foreach(GameObject go in collisions) {
            PlantView plantView = go.GetComponent<PlantView>();
            if(plantView != null) {
                plantView.Water();
            }
        }
    }

    public void Plant(Item item) {
        if(!allowAction) {
            Debug.Log("Can't plant here.");
            return;
        }

        PlayerSpawnGrowableRequest spawnGrowableRequest = new PlayerSpawnGrowableRequest {
            growableId = "Daisy",
            position = (Vector2)transform.position,
        };

        spawnGrowableRequest.HandleRequest();
    }

    public void Till() {
        if(!allowAction) {
            Debug.Log("Can't till here.");
            return;
        }

        PlayerSpawnSoilRequest spawnSoilRequest = new PlayerSpawnSoilRequest {
            position = (Vector2)transform.position,
        };

        spawnSoilRequest.HandleRequest();
    }


    public void Destroy() {
        if(collisions.Count != 1) {
            Debug.Log("Nothing to destroy.");
            return;
        }
        
        PlantView plantView = collisions[0].GetComponent<PlantView>();
        if(plantView == null) {return;}

        PlayerUnSpawnGrowableRequest unSpawnGrowableRequest = new PlayerUnSpawnGrowableRequest {
            networkId = plantView.GetComponent<NetworkIdentity>().netId,
        };
        unSpawnGrowableRequest.HandleRequest();
    }

    public void SnapTo(Vector3 position) {
        //transform.position = position;
        transform.position = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), -1f);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        collisions.Add(collision.gameObject);
    }

    void OnCollisionExit2D(Collision2D collision) {
        collisions.Remove(collision.gameObject);
    }
}
