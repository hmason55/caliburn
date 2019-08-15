using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantView : MonoBehaviour {
    public int uniqueId;
    public string ownerId;
    public Vector2 position;
    public int creationDate;
    public int completionDate;
    public int waterDate;
    public Growable growable;


    CircleCollider2D circleCollider2D;

    List<GameObject> collisions;

    bool valid = true;

    void Awake() {
        circleCollider2D = GetComponent<CircleCollider2D>();
        collisions = new List<GameObject>();
    }
    
    void OnCollisionEnter2D(Collision2D collision) {
        collisions.Add(collision.gameObject);
    }

    void OnCollisionExit2D(Collision2D collision) {
        collisions.Remove(collision.gameObject);
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

        if(soil == 1 && growable + terrain == 0) {
            valid = false;
        }
    }

    public void Validate(Action<bool> isValid = null) {
        StartCoroutine(CollisionValidation(isValid));
    }

    IEnumerator CollisionValidation(Action<bool> isValid = null) {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        if(isValid != null) {
            isValid(valid);
        }
        yield break;
    }

    public void HandleRequest(PlayerSpawnGrowableRequest request) {
        growable = Growables.Instance.growablesById[request.growableId];
        uniqueId = request.uniqueId;
        ownerId = request.ownerId;
        position = request.position;
        creationDate = request.creationDate;
        completionDate = request.completionDate;
        waterDate = request.waterDate;
    }
}
