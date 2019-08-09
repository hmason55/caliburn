using UnityEngine;
using Mirror;
using TMPro;

public class PlayerView : MonoBehaviour {

    public enum State {
        Movement,
        Plant,
        Destroy,
    }

    State state = State.Movement;

    public PlayerData playerData;
    public Vector2 localDestination;
    public Vector2 actualPosition;
    public TextMeshPro nameplate;
    public SpriteRenderer spriteRenderer;
    public AnimationController animationController;
    
    NetworkIdentity identity;
    Rigidbody2D rb;

    Vector2 heading;

    float deadzone = 0.25f;
    float minMovement = 1.25f;
    float speed = 5.0f;

    bool plantMode = false;
    
    void Awake() {
        identity = GetComponent<NetworkIdentity>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        if(!identity.isLocalPlayer) {return;}
        
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            state = State.Movement;
        } else if(Input.GetKeyDown(KeyCode.Alpha2)) {
            state = State.Plant;
        } else if(Input.GetKeyDown(KeyCode.Alpha3)) {
            state = State.Destroy;
        }

        Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));

        switch(state) {
            case State.Movement:
                if(Input.GetMouseButton(0)) {
                    Vector2 target = new Vector2(point.x, point.y);
                    MoveTo(target);
                }
            break;

            case State.Plant:
                GrowableCursor.Instance.SnapTo(point);
                if(Input.GetMouseButtonDown(0)) {
                    GrowableCursor.Instance.Plant();
                }
            break;

            case State.Destroy:
                GrowableCursor.Instance.SnapTo(point);
                if(Input.GetMouseButtonDown(0)) {
                    GrowableCursor.Instance.Destroy();
                }
            break;
        }

        float mx = Input.GetAxis("Horizontal");
        float my = Input.GetAxis("Vertical");
        if(mx != 0f || my != 0f) {
            Vector2 target = new Vector2(transform.position.x, transform.position.y) + (new Vector2(mx, my).normalized * minMovement);
            MoveTo(target);
        }
    }
    
    void FixedUpdate() {
        //if(identity) {return;}
        heading = new Vector2(localDestination.x - transform.position.x, localDestination.y - transform.position.y);
        if(heading.magnitude > deadzone) {
            rb.velocity = heading.normalized * speed;
            //Debug.Log("Moving.");
        } else {
            rb.velocity = Vector2.zero;
            //Debug.Log("stopped.");
            localDestination = actualPosition;
        }

        transform.position = Vector3.Lerp(transform.position, actualPosition, Time.deltaTime * 3f);
        animationController.Animate(rb.velocity);
        //localDestination = destination;
    }

    void MoveTo(Vector2 target) {
        localDestination = target;
        //heading = new Vector2(localDestination.x - transform.position.x, localDestination.y - transform.position.y);
    
        // Raycast to terrain objects and other collision layers that obstruct movement.
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, heading.normalized, heading.magnitude, LayerMask.GetMask("Terrain Collision"));
        //if(hit.collider != null) {
            //localDestination = hit.point - (heading.normalized * transform.GetComponent<CircleCollider2D>().radius);
        //}

       //Debug.DrawLine(new Vector3(transform.position.x, transform.position.y, 1f), new Vector3(localDestination.x, localDestination.y, 1f), Color.red, 5f);


        PlayerMoveToRequest moveTo = new PlayerMoveToRequest {
            networkId = identity.netId,
            destination = target 
        };

        moveTo.HandleRequest();
    }



    public void HandlePlayerData() {
        rb.velocity = Vector2.zero;

        nameplate.text = playerData.username;
        actualPosition = playerData.position;
        localDestination = playerData.destination;
    }
}
