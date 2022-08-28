using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    public float acceleration = 5;
    public float maxSpeed = 5;
    public float rotateSpeed = 20;
    public float maxTorque = 20;
    private Func<float> movementScaling = () => 1.0f;

    public Wallet wallet;

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
        if (GameManager.gameState != GameState.NORMAL) return;

        HandleInteractions();
    }

    // Put all physics related movement in FixedUpdate
    void FixedUpdate() {
        if (GameManager.gameState != GameState.NORMAL) return;

        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement() {
        float scaledMaxSpeed = movementScaling() * maxSpeed;

        // Calculate Input forces
        Vector3 forwardsDir = new Vector3(camTransform.forward.x, 0, camTransform.forward.z).normalized;
        Vector3 rightDir = new Vector3(camTransform.right.x, 0, camTransform.right.z).normalized;
        Vector3 forceInput = movementScaling() * acceleration * (forwardsDir * Input.GetAxisRaw("Vertical") + rightDir * Input.GetAxisRaw("Horizontal"));

        // Add movement force
        float newSpeed = (rb.velocity + forceInput * Time.deltaTime).magnitude;
        Vector3 forceToAdd = newSpeed < scaledMaxSpeed ? forceInput : forceInput.normalized * (scaledMaxSpeed - rb.velocity.magnitude);
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

        GroceryListItem item = GameManager.playerGroceryList.GetItemOnList(shelf.itemData);
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
