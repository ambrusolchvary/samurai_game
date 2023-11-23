using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundOffset : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 0.01f;
    
    void Update() {
        Vector3 pos = transform.position;
        Material mat = GetComponent<Renderer>().material;
        mat.SetTextureOffset("_MainTex", new Vector2(pos.x * moveSpeed, 0));
        Debug.Log("Eyyyyyyyyyy: " + mat.GetTextureOffset("_MainTex"));
    }

}
