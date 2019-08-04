using UnityEngine;
using TMPro;
using Mirror;

public class Player : NetworkBehaviour {
    // Inventory
    // abilities
    // stats, levels
    Vector2 destination;
    float deadzone = 0.25f;
    float speed = 5.0f;
    float minMovement = 1.25f;

    public SpriteRenderer spriteRenderer;
    public Animator animationController;
    public TextMeshPro nameplate; 

    void Start() {
        destination = (Vector2)transform.position;
    }

    void Update() {
        if(isLocalPlayer) {
            transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f) * Time.deltaTime * speed;

            if(Input.GetKeyDown(KeyCode.Space)) {
                //CmdTakeDamage(10);
            }
        }
    }

    public override void OnStartLocalPlayer() {
        spriteRenderer.color = Color.blue;
    }

    /*
    [Command]
    void CmdTakeDamage(int amount) {
        if(!isServer) {return;}
        
        // Server Only
        hitPoints -= amount;
        if(hitPoints <= 0) {
            RpcOnPlayerDeath();
            hitPoints = 100;
        }
    }

    void OnChangedHitPoints(int hp) {
        hitPoints = hp;
    }

    [ClientRpc]
    void RpcOnPlayerDeath() {
        if(isLocalPlayer) {
            transform.position = Vector3.zero;
        }
    }
    */
}
