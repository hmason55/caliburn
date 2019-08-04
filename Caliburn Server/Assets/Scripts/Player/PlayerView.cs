
using UnityEngine;

public class PlayerView : MonoBehaviour {
    public Vector3 destination;
    float deadzone = 0.25f;
    float minMovement = 1.25f;
    float speed = 5.0f;

    public void MoveTo(Vector2 destination) {
        
    }

    void Update() {
        Vector2 heading = new Vector2(destination.x - transform.position.x, destination.y - transform.position.y);
        if(heading.magnitude > deadzone && destination != transform.position) {
            transform.GetComponent<Rigidbody2D>().velocity = heading.normalized * speed;
        } else {
            transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            destination = transform.position;
        }
    }
}
