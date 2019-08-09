using System.Collections;
using UnityEngine;
using Mirror;
using TMPro;

public class PlayerView : MonoBehaviour {
    public PlayerData playerData;
    public Vector2 destination;
    public TextMeshPro nameplate;
    public SpriteRenderer spriteRenderer;
    float deadzone = 0.25f;
    float minMovement = 1.25f;
    float speed = 5.0f;

    float tickDelay = 0.1f;

    NetworkIdentity identity;
    Rigidbody2D rb;

    void Start() {
        identity = GetComponent<NetworkIdentity>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(Sync());
    }

    void Update() {
        if(destination != (Vector2)transform.position) {
            Vector2 heading = new Vector2(destination.x - transform.position.x, destination.y - transform.position.y);
            if(heading.magnitude > deadzone && destination != (Vector2)transform.position) {
                rb.velocity = heading.normalized * speed;
            } else {
                rb.velocity = Vector2.zero;
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
