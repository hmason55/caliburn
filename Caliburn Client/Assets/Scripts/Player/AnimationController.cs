using System;
using UnityEngine;

public class AnimationController : MonoBehaviour {

    public enum ActionState {Idle, Walk}
    public enum DirectionState {Up, Down, Left, Right}
    string[] animationStates = {
        "Idle Up",
        "Idle Down",
        "Idle Left",
        "Idle Right",
        "Walk Up",
        "Walk Down",
        "Walk Left",
        "Walk Right"
    };

    Animator animator;
    ActionState action = ActionState.Walk;
    DirectionState direction;
    DirectionState previousDirection;
    Vector2 velocity;
    float deadzone = 0.125f;

    void Awake() {
        animator = GetComponent<Animator>();
    }
    public void Animate(Vector2 velocity) {
        this.velocity = velocity;
        if(velocity.magnitude > deadzone) {
            HandleDirectionState();
        }
        HandleAnimationSpeed();
    }

    void HandleDirectionState() {
        float absx = Mathf.Abs(velocity.x);
        float absy = Mathf.Abs(velocity.y);

        if(absx > absy) {
            // Face left or right
            if(velocity.x > deadzone) {
                direction = DirectionState.Right;
            } else if(velocity.x < -deadzone) {
                direction = DirectionState.Left;
            }
        } else if(absx < absy) {
            // Face up or down
            if(velocity.y > deadzone) {
                direction = DirectionState.Up;
            } else if(velocity.y < -deadzone) {
                direction = DirectionState.Down;
            }
        }
        
        if(previousDirection != direction) {
            previousDirection = direction;
            animator.SetInteger("Direction", (int)direction);
        }
    }

    void HandleAnimationSpeed() {
        if(velocity.magnitude < deadzone) {
            string stateName = animationStates[(int)action * Enum.GetValues(typeof(DirectionState)).Length + (int)direction];
            animator.Play(stateName, 0, 1f / 4f);
        }
        animator.SetFloat("Speed", velocity.magnitude);
    }
}
