using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] public GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI restartText;
    //[SerializeField] public GameObject gameView;

    private bool isGameOver = false;

    void Start() {
        //Disables panel if active
        gameOverPanel.SetActive(false);
        restartText.gameObject.SetActive(false);
    }

    void Update() {
        //Trigger game over manually and check with bool so it isn't called multiple times
        if (Input.GetKeyDown(KeyCode.G) && !isGameOver) {
            isGameOver = true;

            StartCoroutine(GameOverSequence());
        }

        //If game is over
        if (isGameOver) {
            //If SPACE is hit, restart the current scene
            if (Input.GetButtonDown("Jump")) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            //If Q is hit, quit the game
            if (Input.GetKeyDown(KeyCode.Q)) {
                print("Application Quit");
                Application.Quit();
            }
        }


    }

    //controls game over canvas and there's a brief delay between main Game Over text and option to restart/quit text
    public IEnumerator GameOverSequence() {
        gameOverPanel.SetActive(true);
        isGameOver = true;
        yield return new WaitForSeconds(1.0f);

        restartText.gameObject.SetActive(true);
        //gameView.transform.position -= new Vector3(-10f, 10f, 0f);
    }
}
