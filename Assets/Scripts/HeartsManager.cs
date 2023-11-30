using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartsManager : MonoBehaviour
{
    public GameObject[] hearts;
    public int life = 5;
    private bool dead;
    // Start is called before the first frame update
    void Start()
    {
        life = hearts.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if(dead == true) {

        }
    }

    public void TakeDamage(int damage) {
        if (life >= 1) {
            life -= damage;
            //Destroy(hearts[life].gameObject);
            hearts[life].GetComponent<Animator>().SetTrigger("HeartLostTriggered");
        }
        else {
            dead = true;
        }
    }
}
