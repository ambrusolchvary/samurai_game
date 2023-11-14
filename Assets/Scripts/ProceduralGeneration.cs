using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    [SerializeField] int width, height;
    [SerializeField] GameObject dirt, grass;

    void Start() {
        Generation();
    }

    void Generation() {
        for(int x = 0; x < width; x++) {

            int minHeight = height - 1;
            int maxHeight = height + 1;

            height = Random.Range(minHeight, maxHeight);

            for (int y = 0; y < height; y++) {
                spawnObject(dirt, x, y);
            }
            spawnObject(grass, x, height);
        }
    }

    //Everything we spawn will be a child of our procedural generation GameObject
    void spawnObject(GameObject obj, int width, int height) {
        obj = Instantiate(obj, new Vector2(width, height), Quaternion.identity);
        obj.transform.parent = this.transform;
    }
}
