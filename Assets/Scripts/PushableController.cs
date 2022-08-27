using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableController : Interactable {

    public Collider triggerCollider;

    public void ControlPushable(Transform parent) {
        triggerCollider.enabled = false;
        transform.position = parent.position;
        transform.rotation = parent.rotation;
        transform.parent = parent;
    }

    public void LeavePushable() {
        triggerCollider.enabled = true;
        transform.parent = null;
    }

    public override void OnEnter() {}
    public override void OnExit() {}
}
