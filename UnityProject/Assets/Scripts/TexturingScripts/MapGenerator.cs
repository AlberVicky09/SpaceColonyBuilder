using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public void GenerateMap() {
        float[,] noiseMap = NoiseScript.GenerateNoiseMap(mapWidth, mapHeight, noiseScale);

        DisplayMap display = FindObjectOfType<DisplayMap>();
        display.DrawNoiseMap(noiseMap);
    }
}
