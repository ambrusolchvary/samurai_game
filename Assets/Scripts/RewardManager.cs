using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{
    public int watermelonCount = 0;
    public TextMeshProUGUI watermelonScore;
    public Tilemap door;
    private int minScoreToOpen = 3;
    private bool isOpen = false; // door
    private float openedDoorHeight = 10.0f; // milyen magasra nyiljon fel az ajto
    private Vector3 startDoorPos;
    
    void Start()
    {
        startDoorPos = door.transform.position;
    }
    
    void Update()
    {
        watermelonScore.text = ": " + watermelonCount.ToString();
        if(watermelonCount >= minScoreToOpen && !isOpen) {
            door.gameObject.GetComponent<Rigidbody2D>().velocity += new Vector2(0, 2*openedDoorHeight);
            isOpen = true;
        }
        if (door.transform.position.y >= openedDoorHeight) {
            door.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
    }
}
