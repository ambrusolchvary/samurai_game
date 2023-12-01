using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{
    public int watermelonCount = 0;
    public Text watermelonText;
    //public GameObject door;
    private int minScoreToOpen = 20;
    private bool isOpen; // door

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
            // TODO: open the door
            isOpen = true;
        }
    }
}
