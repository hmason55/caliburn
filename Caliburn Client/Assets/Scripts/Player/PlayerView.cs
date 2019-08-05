using UnityEngine;
using Mirror;
using TMPro;

public class PlayerView : MonoBehaviour {

    public Vector2 localDestination;
    public TextMeshPro nameplate;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    NetworkIdentity identity;

    float deadzone = 0.25f;
    float minMovement = 1.25f;
    float speed = 5.0f;
    
    void Awake() {
        identity = GetComponent<NetworkIdentity>();
    }

    void Update() {
        if(!identity.isLocalPlayer) {return;}

        if(Input.GetMouseButton(0)) {
            Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1f));
            localDestination = new Vector2(point.x, point.y);
            MoveTo();
        } else {
            float mx = Input.GetAxis("Horizontal");
            float my = Input.GetAxis("Vertical");
            if(mx != 0f || my != 0f) {
                localDestination = new Vector2(transform.position.x, transform.position.y) + (new Vector2(mx, my).normalized * minMovement);
                MoveTo();
            }
        }
    }
    
    void FixedUpdate() {
        //if(identity) {return;}
        Vector2 heading = new Vector2(localDestination.x - transform.position.x, localDestination.y - transform.position.y);
        if(heading.magnitude > deadzone && localDestination != (Vector2)transform.position) {
            transform.GetComponent<Rigidbody2D>().velocity = heading.normalized * speed;
        } else {
            transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            localDestination = transform.position;
        }
        
        //localDestination = destination;
    }

    void MoveTo() {
        Vector2 heading = new Vector2(localDestination.x - transform.position.x, localDestination.y - transform.position.y);

        // Raycast to terrain objects and other collision layers that obstruct movement.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, heading.normalized, heading.magnitude, LayerMask.GetMask("Terrain Collision"));
        if(hit.collider != null) {
            localDestination = hit.point - (heading.normalized * transform.GetComponent<CircleCollider2D>().radius);
        }

        Debug.DrawLine(new Vector3(transform.position.x, transform.position.y, 1f), new Vector3(localDestination.x, localDestination.y, 1f), Color.red, 5f);

        PlayerMoveToRequest moveTo = new PlayerMoveToRequest {
            networkId = identity.netId,
            destination = localDestination 
        };

        moveTo.HandleRequest();
    }
}
