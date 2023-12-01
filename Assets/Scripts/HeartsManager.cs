using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartsManager : MonoBehaviour
{
    public GameObject[] hearts;
    public int life = 5;

    public UIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        life = hearts.Length;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void TakeDamage(int damage) {
        if (life >= 1) {
            for(int x = 0; x < damage; x++) {
                life--;
                hearts[life].GetComponent<Animator>().SetTrigger("HeartLostTriggered");
            }
            if (life == 0) StartCoroutine(uiManager.GameOverSequence());
        }
    }
}
