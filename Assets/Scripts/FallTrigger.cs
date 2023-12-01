using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FallTrigger : MonoBehaviour
{
    public GameObject player;
    public HeartsManager heartsManager;

    void Start() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update() {
        Vector3 playerPos = player.transform.position;
        Vector3 ownPos = transform.position;
        transform.position = new Vector3(playerPos.x, ownPos.y, ownPos.z);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject == player) {
            //SceneManager.LoadScene("SampleScene");
            heartsManager.TakeDamage(heartsManager.life);
            player.GetComponent<Animator>().SetTrigger("TakeHitTriggered"); // Ezt lehet at kene tenned a SamuraiMovementbe a collision detekciohoz, ahol a dinnyeket is csekkolod
            player.GetComponent<SamuraiMovement>().PlayerDead();
        }
    }

}
