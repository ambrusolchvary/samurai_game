using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// kezeli a jatek vegeteresekor megjeleno paneleket
public class UIManager : MonoBehaviour
{

    [SerializeField] public GameObject gameOverPanel; // a panel ami megjelenik amikor elvesztjuk a jatekot, mert meghaltunk
    [SerializeField] private TextMeshProUGUI restartText; // a gameOverPanelen levo ujrainditas modjarol tajekoztato uzenet
    [SerializeField] public GameObject winnerPanel; // a panel ami megjelenik amikor megnyerjuk a jatekot, mert eleg dinnyet apritottunk es elertuk a celt
    [SerializeField] private TextMeshProUGUI nextLevelText; // a winnerPanel-en levo kovetkezo palyaralepes modjarol tajekoztato uzenet

    private bool isGameOver = false; // erteke true ha vege a jateknak

    void Start() {
        // kikapcsolja a game over panelt, ha aktiv
        gameOverPanel.SetActive(false);
        restartText.gameObject.SetActive(false);

        // kikapcsolja a winner panelt, ha aktiv
        winnerPanel.SetActive(false);
        nextLevelText.gameObject.SetActive(false);
    }

    void Update() {
        // manualisan trigger-elt game over, de csak ha jelenleg mar nincs epp game over true allapotban
        if (Input.GetKeyDown(KeyCode.G) && !isGameOver) {
            isGameOver = true; // a bool isGameOvert true - ra allitjuk, hogy csak egyszer hajtodjon vegre

            // kulon szalon elinditjuk a game over panel megjeleniteset, hogy a text uzenet kesobb jelenjen meg
            StartCoroutine(GameOverSequence());
        }

        // vege a jateknak
        if (isGameOver) {
            // ha aktiv game over panel kozben SPACE-t nyom a felhasznalo ujraindul a scene
            if (Input.GetButtonDown("Jump")) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            // ha aktiv game over panel kozben ESCAPE-et nyom a felhasznalo kilepunk a jatekbol
            if (Input.GetKeyDown(KeyCode.Escape)) {
                print("Application Quit");
                Application.Quit();
            }
        }
    }
    
    // game over panel-t hozza eloterbe es kesleltetve jeleniti meg a "press space to restart" uzenetet a game over felirathoz kepest
    public IEnumerator GameOverSequence() {
        gameOverPanel.SetActive(true);
        isGameOver = true;
        yield return new WaitForSeconds(1.0f);

        restartText.gameObject.SetActive(true);
    }

    // winner panel-t hozza eloterbe es kesleltetve jeleniti meg a "press space to next level" uzenetet a "you win!" felirathoz kepest
    public IEnumerator WinningSequence() {
        winnerPanel.SetActive(true);
        isGameOver = true;
        yield return new WaitForSeconds(1.0f);

        nextLevelText.gameObject.SetActive(true);
    }
}
