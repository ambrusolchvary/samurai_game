using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralGeneration : MonoBehaviour
{
    [SerializeField] int minWidth, maxWidth, gapWidth, mapHeight, platformsNum; // a platform's min and max width, every gap's width, map's height
    [SerializeField] int startWidth, endWidth; // First and last platform's width;
    [SerializeField] float smoothness, seed;
    [SerializeField] TileBase groundTile;
    [SerializeField] TileBase endPlatformTile;
    [SerializeField] TileBase randomElementTile;
    [SerializeField] TileBase fenceTile;
    [SerializeField] Tilemap collidableTilemap;
    [SerializeField] Tilemap generableTilemap;
    [SerializeField] Tilemap spikeTilemap;
    [SerializeField] Tilemap woundingTilemap;
    [SerializeField] Tilemap doorTilemap;
    [SerializeField] Tile spikeTile;
    [SerializeField] GameObject sliceAbleWatermelon;
    int[,] map;
    int mapWidth;

    void Start() {
        Generation();
    }

    void Generation() {
        int[] widths = GeneratePlatformsWidth(minWidth, maxWidth, platformsNum, startWidth, endWidth);
        mapWidth = CalculateMapWidth(widths, gapWidth);
        map = MapInitialization(mapWidth, mapHeight);
        //Debug.Log(map);
        map = EnvironmentGeneration(map, widths, gapWidth, mapHeight, mapWidth);
        RenderMap(map, mapWidth, mapHeight, collidableTilemap, spikeTilemap, woundingTilemap, doorTilemap, groundTile, endPlatformTile, randomElementTile, fenceTile, spikeTile, sliceAbleWatermelon);
    }

    int[] GeneratePlatformsWidth(int minWidth, int maxWidth, int platformsNum, int startWidth, int endWidth) {
        int[] widths = new int[platformsNum];
        for (int x = 0; x < platformsNum; x++) {
            if (x == 0) widths[x] = startWidth;
            else if (x == platformsNum - 1) widths[x] = endWidth;
            else widths[x] = Random.Range(minWidth, maxWidth);
        }
        return widths;
    }

    int CalculateMapWidth(int[] widths, int gapWidth) {
        int mapWidth = widths[0]; // 25
        for(int x = 1; x < widths.Length; x++) {
            Debug.Log("width: "+widths[x]);
            mapWidth += (widths[x] + gapWidth); // 44 + 8       // 30 + 8
        }
        return mapWidth;
    }

    public int[,] MapInitialization(int width, int height) {
        map = new int[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                map[x, y] = 0;
            }
        }
        return map;
    }

    public int[,] EnvironmentGeneration(int[,] map, int[] widths, int gapWidth, int mapHeight, int mapWidth) {
        int widthIdx = 0;
        int gapStartIdx = widths[widthIdx]; //25
        int gapEndIdx = widths[widthIdx] + gapWidth - 1; //33
        int perlinHeight;
        for (int x = 0; x < mapWidth; x++) {
            perlinHeight = Mathf.RoundToInt(Mathf.PerlinNoise(x / smoothness, seed) * mapHeight / 4);
            perlinHeight += mapHeight / 4;
            if (x >= gapStartIdx && x <= gapEndIdx) {
                if (x == gapEndIdx && widthIdx < widths.Length - 1) {
                    widthIdx++;
                    gapStartIdx = gapEndIdx + widths[widthIdx] + 1; // 82
                    gapEndIdx = gapStartIdx + gapWidth -1; // 90
                }
                map[x, 0] = 4;
            }
            else {
                int endPstartIdx = mapWidth - widths[widths.Length - 1];
                bool isEndPlatform = (x >= endPstartIdx);
                if (x <= widths[0] || isEndPlatform) perlinHeight = 20;
                if (x > mapWidth - widths[widths.Length - 1]/3) perlinHeight = mapHeight;
                for (int y = 0; y < perlinHeight; y++) {
                    map[x, y] = !isEndPlatform ? 1 : 2;
                }
                if (!isEndPlatform) {
                    float rand = Random.Range(0.0f, 1.0f);
                    if (rand <= 0.2f) { // true, az esetek x%-ban kb tehat az adott platform x szazalekan lesz random objektum elhelyezve
                        map[x, perlinHeight] = 3;
                    } else if (rand > 0.2f && rand <= 0.25f && x > widths[0] / 2) { // spikes
                        map[x, perlinHeight] = 4;
                    } else if (rand > 0.25f && rand <= 0.3f && x > widths[0] / 2) {// sliceable object
                        map[x, perlinHeight + 20] = 5;
                    }
                }
                if (x >= endPstartIdx && x < endPstartIdx + 3) {
                    for(int y = perlinHeight; y < mapHeight; y++) {
                        map[x, y] = 6;
                    }                    
                }
            } 
        }
        return map;
    }

    public void RenderMap(int[,] map,int mapWidth, int mapHeight, Tilemap groundTilemap, Tilemap spikeTilemap, Tilemap woundingTilemap, Tilemap doorTilemap, TileBase groundTileBase, TileBase endPlatformTileBase, TileBase randomElementTileBase, TileBase fenceTileBase, Tile spikeTile, GameObject watermelon) {
        for (int x = 0; x < mapWidth; x++) {
            float rotateOnZaxis = 110; // minden egyes dinnyet ennyivel forgatunk az elotte levohoz kepest
            for (int y = 0; y < mapHeight; y++) {
                if(map[x, y] == 1) {
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), groundTileBase);
                }
                if (map[x, y] == 2) {
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), endPlatformTileBase);
                }
                if (map[x, y] == 3) {
                    int prevX = (x == 0) ? 0 : x - 1;
                    int nextX = (x == mapWidth-1) ? x : x + 1;
                    if (map[prevX, y] == 2 || map[nextX, y] == 2)
                        generableTilemap.SetTile(new Vector3Int(x, y, 0), fenceTileBase);
                    else
                        generableTilemap.SetTile(new Vector3Int(x, y, 0), randomElementTileBase);
                }
                if (map[x, y] == 4) {
                    spikeTilemap.transform.localScale = new Vector3(1f, 14f, 1f);
                    if (y == 0) spikeTilemap.SetTile(new Vector3Int(x, y, 0), spikeTile);
                    else woundingTilemap.SetTile(new Vector3Int(x, y, 0), spikeTile);
                }
                if (map[x, y] == 5) {
                    watermelon = Instantiate(watermelon, new Vector2(x, y), Quaternion.identity);
                    float rotationOnZ = watermelon.transform.rotation.z + rotateOnZaxis;
                    Quaternion rotation = Quaternion.Euler(0, 0, rotationOnZ);
                    watermelon.transform.rotation = rotation;
                }
                if (map[x, y] == 6) {
                    doorTilemap.SetTile(new Vector3Int(x, y, 0), endPlatformTileBase);
                }
            }
        }
    }
}
