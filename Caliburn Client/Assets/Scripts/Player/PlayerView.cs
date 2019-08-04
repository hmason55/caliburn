
using UnityEngine;
using Mirror;

public class PlayerView : MonoBehaviour {
    float minMovement = 1.25f;

    void Update() {
        if(Input.GetMouseButton(0)) {
                Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1f));
                MoveTo(new Vector2(point.x, point.y));
        } else {
            float mx = Input.GetAxis("Horizontal");
            float my = Input.GetAxis("Vertical");
            if(mx != 0f || my != 0f) {
                Vector2 destination = new Vector2(transform.position.x, transform.position.y) + (new Vector2(mx, my).normalized * minMovement);
                MoveTo(destination);
            }
        }
    }
    
    void FixedUpdate() {
        
    }

    void MoveTo(Vector2 destination) {
        PlayerMoveToRequest moveTo = new PlayerMoveToRequest { position = destination };
        moveTo.HandleRequest();
    }
}
