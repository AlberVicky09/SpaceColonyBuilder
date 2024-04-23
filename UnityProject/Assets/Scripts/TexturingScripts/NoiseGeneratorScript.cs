using UnityEngine;

public class NoiseScript {
    
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale) {
        float[,] noiseMap = new float[mapWidth, mapHeight];
        float sampleX, sampleY;

        for(int y = 0; y < mapHeight; y++) {
            for(int x = 0; x < mapWidth; x++) {
                sampleX = x / scale;
                sampleY = y / scale;

                noiseMap[x, y] = Mathf.PerlinNoise(sampleX, sampleY);
            }
        }
        return noiseMap;
    }
}
