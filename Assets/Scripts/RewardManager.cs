using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{
    public int watermelonCount = 0;
    public Text watermelonText;
    public Tilemap door;
    private int minScoreToOpen = 3;
    private bool isOpen = false; // door

    // Start is called before the first frame update
    void Start()
    {        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(watermelonCount);
        watermelonText.text = ": " + watermelonCount.ToString();
        if(watermelonCount >= minScoreToOpen && !isOpen) {
            door.transform.position += new Vector3(0, 15, 0);
            isOpen = true;
        }
    }
}
