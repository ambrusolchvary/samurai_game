using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideScrolling : MonoBehaviour
{
    private Transform player;
    private Transform background;
    private Vector3 backgroundPos;

    private void Awake() {
        player = GameObject.FindWithTag("Player").transform;
        background = this.transform.GetChild(0);
        backgroundPos = background.position;
    }

    private void LateUpdate() {
        Vector3 cameraPosition = transform.position;
        cameraPosition.x = Mathf.Max(cameraPosition.x, player.position.x);       // Ha nem akarjuk, hogy a kamera hatrafele is kovesse a karaktert
        cameraPosition.y = player.position.y + 4f;
        transform.position = cameraPosition;
        background.position = backgroundPos;
    }
}
