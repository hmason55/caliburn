using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GrowableCursor : MonoSingleton<GrowableCursor> {
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigidbody2D;
    List<GameObject> collisions;
    

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        collisions = new List<GameObject>();
    }

    void FixedUpdate() {
        if(collisions.Count > 0) {
            spriteRenderer.color = Color.red;
        } else {
            spriteRenderer.color = Color.green;
        }
    }

    public void Plant() {
        if(collisions.Count > 0) {
            Debug.Log("Can't plant here.");
            return;
        }

        PlayerSpawnGrowableRequest spawnGrowableRequest = new PlayerSpawnGrowableRequest {
            growableId = 0,
            position = (Vector2)transform.position,
        };

        spawnGrowableRequest.HandleRequest();
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
