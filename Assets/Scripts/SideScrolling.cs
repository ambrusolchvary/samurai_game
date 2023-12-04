using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideScrolling : MonoBehaviour
{
    public float followspeed = 2.0f;

    private Transform player;
    private Transform background;
    private Vector3 backgroundPos;
    private Vector3 cameraStartPos;

    private void Awake() {
        cameraStartPos = transform.position;
        player = GameObject.FindWithTag("Player").transform;
        background = this.transform.GetChild(0);
        backgroundPos = background.position;
    }

    void LateUpdate() {
        float dt = Time.deltaTime;
        float fspeed = Mathf.Min(1.0f, followspeed * dt);
        Vector3 oldPos = transform.position;
        Vector3 newPos = oldPos;
        newPos.x = Mathf.Max(cameraStartPos.x, player.position.x);
        newPos.y = player.position.y + 4f;
        newPos = oldPos + (newPos - oldPos) * fspeed;
        transform.position = newPos;
        background.position = backgroundPos;
    }
    
}
