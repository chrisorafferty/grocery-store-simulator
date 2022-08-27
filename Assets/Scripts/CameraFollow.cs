using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    [SerializeField]
    private Transform target;
    [SerializeField]
    private Vector2 optimalOffset = new Vector2(-5, 3);
    [SerializeField][Range(0, 1)]
    private float lerpAmount = 0.1f;
    [SerializeField]
    private float rotateSpeed = 100;

    void Update() {
        transform.RotateAround(target.position, Vector3.up, Input.GetAxisRaw("Mouse X") * rotateSpeed * Time.deltaTime);
    }

    void FixedUpdate() {
        if (target != null) {
            Vector3 targetDir = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
            Vector3 targetPos = target.position + targetDir * optimalOffset.x + Vector3.up * optimalOffset.y;

            transform.position = Vector3.Lerp(transform.position, targetPos, lerpAmount);    
        }
    }
}
