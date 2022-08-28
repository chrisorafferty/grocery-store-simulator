using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightableInteractable : Interactable {

    public Material highlightMat;
    public MeshRenderer meshRenderer;

    protected Material normalMat;

    protected virtual void Start() {
        normalMat = meshRenderer.sharedMaterial;
    }

    public override void OnEnter() {
        meshRenderer.sharedMaterial = highlightMat;
    }
    
    public override void OnExit() {
        meshRenderer.sharedMaterial = normalMat;
    }
}
