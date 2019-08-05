using System.Collections;
using UnityEngine;
using Mirror;
using TMPro;

public class PlayerView : MonoBehaviour {
    public Vector2 destination;
    public TextMeshPro nameplate;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    float deadzone = 0.25f;
    float minMovement = 1.25f;
    float speed = 5.0f;

    float tickDelay = 0.1f;

    NetworkIdentity identity;

    void Start() {
        identity = GetComponent<NetworkIdentity>();
        StartCoroutine(Sync());
    }

    public void MoveTo(Vector2 destination) {
        
    }

    void Update() {
        if(destination != (Vector2)transform.position) {
            Vector2 heading = new Vector2(destination.x - transform.position.x, destination.y - transform.position.y);
            if(heading.magnitude > deadzone && destination != (Vector2)transform.position) {
                transform.GetComponent<Rigidbody2D>().velocity = heading.normalized * speed;
            } else {
                transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                destination = transform.position;
            }
        }
    }

    IEnumerator Sync() {
        while(true) {
            PlayerPositionMessage playerPosition = new PlayerPositionMessage {
                networkId = identity.netId,
                position = (Vector2)transform.position
            };
            playerPosition.HandleRequest();
            yield return new WaitForSeconds(tickDelay);
        }
        yield break;
    }
}
