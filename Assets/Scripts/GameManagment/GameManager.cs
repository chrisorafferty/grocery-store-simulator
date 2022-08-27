using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public static GameState gameState { get; protected set; }

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(instance);
        }

        instance = this;
    }

    void Update() {
        if (Input.GetButtonDown("ManagementSwitch")) {
            if (gameState == GameState.MANAGEMENT) gameState = GameState.NORMAL;
            else if (gameState == GameState.NORMAL) gameState = GameState.MANAGEMENT;
        }
    }
}
