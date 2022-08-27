using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    [Header("Normal GameState")]
    public Transform target;
    public Vector2 optimalOffset = new Vector2(-5, 3);
    public float lookHeightOffset = 1.5f;
    public float lerpAmount = 0.1f;
    public float rotateSpeed = 100;

    [Header("Management GameState")]
    public float targetManagementHeight = 20;
    public float managementMoveSpeed = 50;
    public float managementLerpAmount = 0.1f;

    void Update() {
        switch(GameManager.gameState) {
            case GameState.NORMAL:
                handleNormalGameStateMovement();
                break;
            case GameState.MANAGEMENT:
                handleManagementGameStateMovement();
                break;
        }
    }

    void FixedUpdate() {
        if (GameManager.gameState != GameState.NORMAL) return;

        if (target != null) {
            Vector3 targetDir = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
            Vector3 targetPos = target.position + targetDir * optimalOffset.x + Vector3.up * optimalOffset.y;

            transform.position = Vector3.Lerp(transform.position, targetPos, lerpAmount);

            Vector3 dirToTarget = target.position - transform.position + Vector3.up * lookHeightOffset;
            float angle = Vector3.SignedAngle(dirToTarget, new Vector3(dirToTarget.x, 0, dirToTarget.z), transform.right); 
            Quaternion targetRot = Quaternion.Euler(-angle, transform.localEulerAngles.y, transform.localEulerAngles.z);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, lerpAmount);
        }
    }

    void handleManagementGameStateMovement() {
        Vector3 targetPos = new Vector3(transform.position.x, targetManagementHeight, transform.position.z);
        targetPos += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * managementMoveSpeed * Time.deltaTime;
        
        transform.position = Vector3.Lerp(transform.position, targetPos, managementLerpAmount);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.down, Vector3.forward), managementLerpAmount);
    }

    void handleNormalGameStateMovement() {
        if (target == null) return;

        transform.RotateAround(target.position, Vector3.up, Input.GetAxisRaw("Mouse X") * rotateSpeed * Time.deltaTime);
    }
}
