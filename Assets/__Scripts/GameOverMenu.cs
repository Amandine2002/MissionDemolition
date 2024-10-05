using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour {

    public void RestartGame() {
        SceneManager.LoadScene("_Scene_0");
    }

    public void QuitGame() {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
