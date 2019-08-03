using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Unit : MonoBehaviour {
    /*
    public string sentName;
    public int count;

    public Vector3 destination;

    float deadzone = 0.25f;
    float minMovement = 1.25f;
    float speed = 5.0f;

    public Transform flag;

    public SpriteRenderer spriteRenderer;
    public Animator animationController;
    public TextMeshPro nameplate; 

    void Start() {
        //destination = transform.position;
    }

    void Update() {

        
        if(Input.GetKeyDown(KeyCode.Space)) {
           // networkObject.SendRpc(RPC_MOVE_UP, Receivers.AllBuffered);
        }

        


        #region Client Only
        if(!networkObject.IsServer) {
            
            if(Input.GetMouseButton(0)) {
                Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1f));
                networkObject.SendRpc(RPC_MOVE_TO, Receivers.All, new Vector2(point.x, point.y));
            } else {
                float mx = Input.GetAxis("Horizontal");
                float my = Input.GetAxis("Vertical");
                if(mx != 0f || my != 0f) {
                    Vector2 moveto = new Vector2(transform.position.x, transform.position.y) + (new Vector2(mx, my).normalized * minMovement);
                    networkObject.SendRpc(RPC_MOVE_TO, Receivers.All, moveto);
                }
            }

            // Sync position with server position.
            transform.position = networkObject.position;
            Animate();
            nameplate.text = networkObject.UniqueIdentity.ToString();
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
            
        }
        #endregion

        if(!networkObject.IsServer) {return;}

        #region Server Only
        if(Input.GetKeyDown(KeyCode.B)) {
            networkObject.SendRpc(RPC_DO_SOMETHING, Receivers.All, "Hunter", new Vector3(Random.value, Random.value, 0f) * 5f, ++count);
        }

        // Control movement input.
        Vector2 heading = new Vector2(destination.x - networkObject.position.x, destination.y - networkObject.position.y);
        if(heading.magnitude > deadzone && destination != networkObject.position) {
            transform.GetComponent<Rigidbody2D>().velocity = heading.normalized * speed;
        } else {
            transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            destination = networkObject.position;
        }
        
        
        networkObject.position = transform.position;
        networkObject.velocity = transform.GetComponent<Rigidbody2D>().velocity;
        #endregion
    }

    void FixedUpdate() {
        
    }

    public override void MoveTo(RpcArgs args) {
        Vector2 position = args.GetNext<Vector2>();
        Vector2 heading = new Vector2(position.x - networkObject.position.x, position.y - networkObject.position.y);

        // Raycast to terrain objects and other collision layers that obstruct movement.
        RaycastHit2D hit = Physics2D.Raycast(networkObject.position, heading.normalized, heading.magnitude, LayerMask.GetMask("Terrain Collision"));
        if(hit.collider != null) {
            destination = hit.point - (heading.normalized * transform.GetComponent<CircleCollider2D>().radius);
        } else {
            destination = position;
            
        }
        
        Debug.DrawLine(new Vector3(networkObject.position.x, networkObject.position.y, 1f), new Vector3(destination.x, destination.y, 1f), Color.yellow, 5f);
        
        flag.transform.position = destination;
    }

    public void Animate() {
        Vector2 velocity = networkObject.velocity / networkObject.velocity.magnitude;
        float deadzone = 0.125f;

        float absx = Mathf.Abs(velocity.x);
        float absy = Mathf.Abs(velocity.y);

        
        string[] stateNames = new string[] {
            "Walk Up",
            "Walk Down",
            "Walk Left",
            "Walk Right"
        };

        if(absx > absy) {
            // Face left or right
            if(velocity.x > deadzone) {
                animationController.SetInteger("Direction", 3);
            } else if(velocity.x < -deadzone) {
                animationController.SetInteger("Direction", 2);
            }
        } else if(absx < absy) {
            // Face up or down
            if(velocity.y > deadzone) {
                animationController.SetInteger("Direction", 0);
            } else if(velocity.y < -deadzone) {
                animationController.SetInteger("Direction", 1);
            }
        }

        int direction = animationController.GetInteger("Direction");
        if(networkObject.velocity.magnitude < deadzone) {
            animationController.Play(stateNames[direction], 0, 1f / 4f);
        }
        animationController.SetFloat("Speed", networkObject.velocity.magnitude);
    }

    public override void MoveUp(RpcArgs args) {
        transform.position += Vector3.up;
    }

    public override void DoSomething(RpcArgs args) {
        string name = args.GetNext<string>();
        Vector3 pos = args.GetNext<Vector3>();
        int counter = args.GetNext<int>();

        Debug.Log("Hello " + name);
        sentName = name;
        count = counter;
        transform.position = pos;
    }
    */
}
