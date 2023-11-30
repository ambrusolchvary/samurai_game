using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlatformGeneration : MonoBehaviour
{
    [SerializeField] int minWidth, maxWidth;
    //[SerializeField] int minHeight, maxHeight;
    [Range(0, 100)]
    [SerializeField] float heightValue, smoothness;
    [SerializeField] int repeatNum;
    //[SerializeField] GameObject grass, dirt;
    [SerializeField] Tilemap collidableObjectTilemap, generableObjectTilemap;
    [SerializeField] Tile grassLeft, grassMiddle, grassRight, dirtLeft, dirtMiddle, dirtRight;

    int width;
    int height;
    float seed;
    

    void Start() {
        int platformOffset = 0;

        width = 30;
        height = 3;
        seed = 0;
        Generation(platformOffset, true); // kiindulasi platform, ahova a jatekos megjelenik inditaskor
        for (int i = 0; i < 10; i++) {
            platformOffset += width + 10;
            width = Random.Range(minWidth, maxWidth);
            seed = Random.Range(-1000000, 1000000);
            Generation(platformOffset, false);
        }
    }

    void Generation(int platformOffset, bool startPlatform) {
        int repeatValue = 0;
        int prevHeight = height;
        for (int x = platformOffset; x < platformOffset + width; x++) {
            if (repeatValue == 0 && !startPlatform) {
                height = Mathf.RoundToInt(heightValue * Mathf.PerlinNoise(x / smoothness, seed));
                GenerateFlatPlatform(x, platformOffset);
                repeatValue = repeatNum;
            } else {
                GenerateFlatPlatform(x, platformOffset); // az enumot a lepcsozetes megjeleniteshez a tilemappek miatt hoztad letre de lehet eleg Rule tile/t hasznalni
                repeatValue--;
            }
        }
    }

    void GenerateFlatPlatform(int x, int platformOffset) {
        for (int y = 0; y < height; y++) {
            //spawnObject(dirt, x, y);
            if(x == platformOffset) generableObjectTilemap.SetTile(new Vector3Int(x, y, 0), dirtLeft);
            else if (x == platformOffset + width-1) generableObjectTilemap.SetTile(new Vector3Int(x, y, 0), dirtRight);
            else generableObjectTilemap.SetTile(new Vector3Int(x, y, 0), dirtMiddle);
        }
        //spawnObject(grass, x, height);
        if (x == platformOffset) collidableObjectTilemap.SetTile(new Vector3Int(x, height, 0), grassLeft);
        else if (x == platformOffset + width - 1) collidableObjectTilemap.SetTile(new Vector3Int(x, height, 0), grassRight);
        else collidableObjectTilemap.SetTile(new Vector3Int(x, height, 0), grassMiddle);
    } 


    /*
    //Everything we spawn will be a child of our procedural generation GameObject
    void spawnObject(GameObject obj, int width, int height) {
        obj = Instantiate(obj, new Vector2(width, height), Quaternion.identity);
        obj.transform.parent = this.transform;
    }
    */
}
