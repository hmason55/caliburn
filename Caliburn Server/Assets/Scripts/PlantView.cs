using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantView : MonoBehaviour {

    public string ownerId;
    public Vector2 position;
    public int creationDate;
    public int completionDate;
    public Growable growable;

    CircleCollider2D circleCollider2D;

    bool valid = true;

    void Awake() {
        circleCollider2D = GetComponent<CircleCollider2D>();
    }
    
    void OnCollisionEnter2D(Collision2D collision) {
        valid = false;
        Debug.Log("Can't spawn here.");
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
        growable = Growables.Instance.growableData[request.growableId];
        ownerId = request.ownerId;
        position = request.position;
        creationDate = request.creationDate;
        completionDate = request.completionDate;
    }
}
