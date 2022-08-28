using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public static GameState gameState { get; protected set; }

    public delegate void GameStateChanged(GameState newGameState, GameState oldGameState);
    public static event GameStateChanged GameStateChangedEvent;

    public ItemData[] items;
    public LayerMask managementSelectionMask;
    private int curItemSelection = -1;
    private ShelfController prevShelfSelection;

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(instance);
        }

        instance = this;
    }

    void Update() {
        if (Input.GetButtonDown("ManagementSwitch")) {
            GameState prevGameState = gameState;
            if (gameState == GameState.MANAGEMENT) gameState = GameState.NORMAL;
            else if (gameState == GameState.NORMAL) gameState = GameState.MANAGEMENT;

            if (prevGameState != gameState) {
                GameStateChangedEvent?.Invoke(gameState, prevGameState);
            }
        }

        if (gameState == GameState.MANAGEMENT) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            HandleManagementState();
        } else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (prevShelfSelection != null) {
                prevShelfSelection.OnExit();
                prevShelfSelection = null;
            }
        }
    }

    void HandleManagementState() {
        if (Input.GetKeyDown(KeyCode.Alpha0) && items.Length > 0) curItemSelection = 0;
        if (Input.GetKeyDown(KeyCode.Alpha1) && items.Length > 1) curItemSelection = 1;
        if (Input.GetKeyDown(KeyCode.Alpha2) && items.Length > 2) curItemSelection = 2;
        if (Input.GetKeyDown(KeyCode.Alpha3) && items.Length > 3) curItemSelection = 3;
        if (Input.GetKeyDown(KeyCode.Alpha4) && items.Length > 4) curItemSelection = 4;
        if (Input.GetKeyDown(KeyCode.Alpha5) && items.Length > 5) curItemSelection = 5;
        if (Input.GetKeyDown(KeyCode.Alpha6) && items.Length > 6) curItemSelection = 6;
        if (Input.GetKeyDown(KeyCode.Alpha7) && items.Length > 7) curItemSelection = 7;
        if (Input.GetKeyDown(KeyCode.Alpha8) && items.Length > 8) curItemSelection = 8;
        if (Input.GetKeyDown(KeyCode.Alpha9) && items.Length > 9) curItemSelection = 9;

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        ShelfController shelf = null;
        if (Physics.Raycast(mouseRay, out hit, 1000, managementSelectionMask)) {
            shelf = hit.transform.GetComponent<ShelfController>();
            if (shelf != null) {
                shelf.OnEnter();
            }
        }

        if (prevShelfSelection != null && prevShelfSelection != shelf) {
            prevShelfSelection.OnExit();
        }
        prevShelfSelection = shelf;

        if (shelf != null && Input.GetButtonDown("Fire1")) {
            if (curItemSelection >= 0) {
                shelf.SetItemType(items[curItemSelection]);
            }
        }
    }
}
