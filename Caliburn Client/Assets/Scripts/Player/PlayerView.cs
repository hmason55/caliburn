using UnityEngine;
using Mirror;
using TMPro;


public class PlayerView : MonoBehaviour {

    public PlayerData playerData;
    public Vector2 localDestination;
    public Vector2 actualPosition;
    public TextMeshPro nameplate;
    public AnimationController animationController;
    
    NetworkIdentity identity;
    Rigidbody2D rb;

    Vector2 heading;

    float deadzone = 0.25f;
    public float minMovement = 1.25f;
    float speed = 5.0f;
    
    void Awake() {
        identity = GetComponent<NetworkIdentity>();
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Start() {
        if(!identity.isLocalPlayer) {
            Debug.Log("Removing controller...");
            Destroy(GetComponent<PlayerController>());
        }
    }

    void FixedUpdate() {
        heading = new Vector2(localDestination.x - transform.position.x, localDestination.y - transform.position.y);
        if(heading.magnitude > deadzone) {
            rb.velocity = heading.normalized * speed;
        } else {
            rb.velocity = Vector2.zero;
            localDestination = actualPosition;
        }

        transform.position = Vector3.Lerp(transform.position, actualPosition, Time.deltaTime * 3f);
        animationController.Animate(rb.velocity);
    }

    public void MoveTo(Vector2 target) {
        
        //heading = new Vector2(localDestination.x - transform.position.x, localDestination.y - transform.position.y);
    
        // Raycast to terrain objects and other collision layers that obstruct movement.
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, heading.normalized, heading.magnitude, LayerMask.GetMask("Terrain Collision"));
        //if(hit.collider != null) {
            //localDestination = hit.point - (heading.normalized * transform.GetComponent<CircleCollider2D>().radius);
        //}

       //Debug.DrawLine(new Vector3(transform.position.x, transform.position.y, 1f), new Vector3(localDestination.x, localDestination.y, 1f), Color.red, 5f);


        
    }

    public void HandlePlayerData() {
        rb.velocity = Vector2.zero;

        nameplate.text = playerData.username;
        actualPosition = playerData.position;
        localDestination = playerData.destination;
    }
}
