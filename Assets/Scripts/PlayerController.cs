using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float acceleration = 5;
    [SerializeField]
    private float maxSpeed = 5;
    [SerializeField]
    private float rotateSpeed = 20;

    [SerializeField]
    private float maxTorque = 20;

    private Rigidbody rb;
    private Transform camTransform;

    void Start() {
        rb = GetComponent<Rigidbody>();
        camTransform = Camera.main.transform;
    }

    // Put all physics related movement in FixedUpdate
    void FixedUpdate() {
        // Add input forces
        Vector3 forwardsDir = new Vector3(camTransform.forward.x, 0, camTransform.forward.z).normalized;
        Vector3 rightDir = new Vector3(camTransform.right.x, 0, camTransform.right.z).normalized;
        Vector3 forceInput = acceleration * (forwardsDir * Input.GetAxisRaw("Vertical") + rightDir * Input.GetAxisRaw("Horizontal"));

        float newSpeed = (rb.velocity + forceInput * Time.deltaTime).magnitude;

        Vector3 forceToAdd = newSpeed < maxSpeed ? forceInput : forceInput.normalized * (maxSpeed - rb.velocity.magnitude);

        rb.AddForce(forceToAdd, ForceMode.Acceleration);

        float angle = Vector3.SignedAngle(transform.forward, forwardsDir, Vector3.up);
        float torque = angle * rotateSpeed;
        float degPerSec = rb.angularVelocity.y * Mathf.Rad2Deg;
/*
        if (curTorque > 0 && curTorque + rotateAmount * Time.deltaTime > maxTorque) {
            rotateAmount = maxTorque - curTorque;
        } else if (curTorque < 0 && curTorque - rotateAmount * Time.deltaTime < -maxTorque) {
            rotateAmount = -maxTorque - curTorque;
        }        
*/
        if (angle > 0 && degPerSec / (rotateSpeed * 3) > angle) {
            torque = -angle * rotateSpeed;
        }

        if (angle < 0 && degPerSec / (rotateSpeed * 3) < angle) {
            torque = -angle * rotateSpeed;
        }

        rb.AddTorque(0, torque, 0, ForceMode.Acceleration);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
}
