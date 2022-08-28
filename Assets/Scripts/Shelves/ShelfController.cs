using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShelfController : Interactable {

    public ItemData itemData { get; protected set; }
    public ShelfLocation[] shelfLocations;
    public Material highlightMat;
    public MeshRenderer meshRenderer;
    public Image managementIconDisplay;
    public Image shelfIconDisplay;

    private Material normalMat;

    void Start() {
        normalMat = meshRenderer.sharedMaterial;
        SetIconDisplayFromGameState(GameManager.gameState);
    }

    public bool SetItemType(ItemData itemType) {        
        foreach(ShelfLocation location in shelfLocations) {
            if (location.Filled) return false;
        }
        itemData = itemType;
        managementIconDisplay.sprite = itemType.icon;
        shelfIconDisplay.sprite = itemType.icon;
        return true;
    }

    public bool Restock() {
        if (itemData == null) return false;

        foreach(ShelfLocation location in shelfLocations) {
            if (!location.Filled) {
                location.SetItem(itemData);
                return true;
            }
        }
        return false;
    }

    public bool TakeItem() {
        bool tookItem = false;
        bool allEmpty = true;

        foreach(ShelfLocation location in shelfLocations) {
            if (!tookItem && location.Filled) {
                location.RemoveItem();
                tookItem = true;
            }

            if (location.Filled) allEmpty = false;
        }

        if (allEmpty) itemData = null;

        return tookItem;
    }

    private void SetIconDisplayFromGameState(GameState gameState) {
        managementIconDisplay.enabled = gameState == GameState.MANAGEMENT;
    }

    private void OnGameStateChanged(GameState newGameState, GameState oldGameState) {
        SetIconDisplayFromGameState(newGameState);
    }

    public override void OnEnter() {
        meshRenderer.sharedMaterial = highlightMat;
    }
    public override void OnExit() {
        meshRenderer.sharedMaterial = normalMat;
    }

    void OnEnable() {
        GameManager.GameStateChangedEvent += OnGameStateChanged;
    }

    void OnDisable() {
        GameManager.GameStateChangedEvent -= OnGameStateChanged;
    }
}
