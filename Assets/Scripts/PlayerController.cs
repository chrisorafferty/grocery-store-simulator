using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    public float acceleration = 20;
    public float deceleration = 15;
    public float maxSpeed = 5;
    public float rotateSpeed = 20;
    public float maxTorque = 20;
    public Animator anim; 

    private Func<float> movementScaling = () => 1.0f;

    private Rigidbody rb;
    private Transform camTransform;

    private List<Interactable> interactables = new List<Interactable>();
    private Interactable prevClosestInteractable = null;

    private PushableController currentPushable = null;

    void Start() {
        rb = GetComponent<Rigidbody>();
        camTransform = Camera.main.transform;
    }

    void Update() {
        if (GameManager.gameState != GameState.NORMAL) {
            if (prevClosestInteractable != null) {
                prevClosestInteractable.OnExit();
                prevClosestInteractable = null;
            }
            return;
        }

        HandleInteractions();

        anim.SetFloat("Speed", rb.velocity.magnitude);
        anim.SetBool("Pushing", currentPushable != null);
    }

    // Put all physics related movement in FixedUpdate
    void FixedUpdate() {
        if (GameManager.gameState != GameState.NORMAL) return;

        HandleRotation();
        HandleMovement();
    }

    private void HandleMovement() {
        float scaledMaxSpeed = movementScaling() * maxSpeed;

        // Calculate Input forces
        Vector3 forwardsDir = new Vector3(camTransform.forward.x, 0, camTransform.forward.z).normalized;
        Vector3 rightDir = new Vector3(camTransform.right.x, 0, camTransform.right.z).normalized;
        Vector3 inputForce = forwardsDir * Input.GetAxisRaw("Vertical") + rightDir * Input.GetAxisRaw("Horizontal");
        Vector3 normalizedInputForce = inputForce.sqrMagnitude > 0.01f ? inputForce.normalized : Vector3.zero;
        Vector3 acceleratedInputForce = acceleration * normalizedInputForce;

        // Find deceleration if no input given
        Vector3 decelerationForce = GetDecelerationForce(normalizedInputForce, rb.velocity);
        rb.AddForce(decelerationForce, ForceMode.Acceleration);   

        // Find new speed ignoring max speed
        float newSpeed = (rb.velocity + acceleratedInputForce * Time.deltaTime).magnitude;

        // Use the acceleration force unless it will be larger than the max speed
        Vector3 forceToAdd = newSpeed < scaledMaxSpeed ? acceleratedInputForce : normalizedInputForce * (scaledMaxSpeed - rb.velocity.magnitude);
        rb.AddForce(forceToAdd, ForceMode.Acceleration);
    }

    private void HandleRotation() {
        float scaledRotateSpeed = movementScaling() * rotateSpeed;

        // Calculate torque to apply to rotate in line with camera
        Vector3 forwardsDir = new Vector3(camTransform.forward.x, 0, camTransform.forward.z).normalized;
        float angle = Vector3.SignedAngle(transform.forward, forwardsDir, Vector3.up);
        float torque = angle * scaledRotateSpeed;
        float degPerSec = rb.angularVelocity.y * Mathf.Rad2Deg;

        // Calculate braking torque to stop near the center
        if (angle > 0 && degPerSec / (scaledRotateSpeed * 3) > angle) {
            torque = -angle * scaledRotateSpeed;
        }

        if (angle < 0 && degPerSec / (scaledRotateSpeed * 3) < angle) {
            torque = -angle * scaledRotateSpeed;
        }

        // Apply torque
        rb.AddTorque(0, torque, 0, ForceMode.Acceleration);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    private Vector3 GetDecelerationForce(Vector3 inputForce, Vector3 curVel) {
        // No input provided
        if (inputForce.sqrMagnitude < 0.05f) {
            Vector3 rawDeceleration = -curVel.normalized * deceleration;
            // We are about to decelerate too much and start ping ponging
            if (curVel.magnitude < rawDeceleration.magnitude * Time.deltaTime) {
                // Just cancel out the current velocity (probably off as it will get modified by Time.deltaTime but oh well)
                return -curVel;
            }
            return rawDeceleration;
        }

        // Currently being accelerated via an input force. Decelerate any sideways velocity

        Vector3 sidewaysDir = Vector3.Cross(inputForce.normalized, Vector3.up);
        Vector3 sidewaysVel = sidewaysDir * Vector3.Dot(sidewaysDir, curVel);
        Vector3 sidewaysDeceleration = -sidewaysVel.normalized * deceleration;

        // We are about to decelerate too much and start ping ponging
        if (sidewaysVel.magnitude < sidewaysDeceleration.magnitude * Time.deltaTime) {
            // Just cancel out the current sideways velocity (probably off as it will get modified by Time.deltaTime but oh well)
            return -sidewaysVel;
        }
        return sidewaysDeceleration;
    }

    void HandleInteractions() {
        Interactable closest = null;
        float closestDist = float.MaxValue;
        foreach(Interactable interactable in interactables) {
            float dist = Vector3.Distance(transform.position, interactable.transform.position);
            if (dist < closestDist) {
                closestDist = dist;
                closest = interactable;
            }
        }

        if (closest == null && prevClosestInteractable != null) {
            prevClosestInteractable.OnExit();
            prevClosestInteractable = null;
        }

        if (closest != null && closest != prevClosestInteractable) {
            if (prevClosestInteractable != null) prevClosestInteractable.OnExit();

            closest.OnEnter();
            prevClosestInteractable = closest;
        }

        HandleShelfInteractions(closest);
        HandlePushableInteractions(closest);
        HandleGroceryOrderInteractions(closest);
    }

    void HandleGroceryOrderInteractions(Interactable closestInteractable) {
        if (closestInteractable != null && Input.GetButtonDown("Interact")) {
            GroceryListAssigner assigner = closestInteractable.GetComponent<GroceryListAssigner>();
            if (assigner != null) {
                assigner.AssignPlayerGroceryList();
            }

            GroceryOrderDropoff dropoff = closestInteractable.GetComponent<GroceryOrderDropoff>();
            if (dropoff != null) {
                dropoff.DropOffPlayerGroceryOrder();
            }
        }
    }

    void HandlePushableInteractions(Interactable closestInteractable) {
        if (currentPushable != null && Input.GetButtonDown("ControlPushable")) {
            currentPushable.LeavePushable();
            currentPushable = null;
            movementScaling = () => 1.0f;
        } else if (closestInteractable != null && currentPushable == null && Input.GetButtonDown("ControlPushable")) {
            PushableController pushable = closestInteractable.GetComponent<PushableController>();
            if (pushable != null) {
                pushable.ControlPushable(transform);
                currentPushable = pushable;
                movementScaling = pushable.getMovementScaling;
                interactables.Remove(pushable);
            }
        }
    }

    void HandleShelfInteractions(Interactable closestInteractable) {
        if (closestInteractable != null && currentPushable != null && Input.GetButtonDown("Interact")) {
            ShelfController shelf = closestInteractable.GetComponent<ShelfController>();
            if (shelf != null) {
                switch (currentPushable.pushableType) {
                    case PushableType.RESTOCKING:
                        AttemptRestockItem(shelf);
                        break;
                    case PushableType.PICKING:
                        AttemptPickItem(shelf);
                        break;
                } 
            }
        }
    }

    void AttemptRestockItem(ShelfController shelf) {
        if (shelf.itemData != null) {
            shelf.Restock();
        }
    }

    void AttemptPickItem(ShelfController shelf) {
        if (shelf.itemData == null) return;

        GroceryListItem item = GameManager.playerGroceryList?.GetItemOnList(shelf.itemData);
        if (item == null || item.IsFullyPicked) return;

        if (shelf.TakeItem()) {
            GameManager.playerGroceryList.pickItemOnList(item.itemData);
        }
    }

    void OnTriggerEnter(Collider other) {
        Interactable interactable = other.gameObject.GetComponent<Interactable>();
        if (interactable != null) {
            interactables.Add(interactable);
        }
    }

    void OnTriggerExit(Collider other) {
        Interactable interactable = other.gameObject.GetComponent<Interactable>();
        if (interactable != null) {
            interactables.Remove(interactable);
        }
    }
}
