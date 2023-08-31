using System;
using System.Globalization;
using Sandbox;
using CM = CarGame.ChunkManager;

namespace CarGame;

public class Biome {
    public float MaxHeight { get; set; } = 100;
    public Material Material { get; set; } = Material.Load("materials/debugblend.vmat");

    public Biome() { }
    public Biome(float maxHeight, string matPath) {
        MaxHeight = maxHeight;
        Material = Material.Load(matPath);
    }

    public virtual Vector3 VertexPos(Vector3 chunkPos, int x, int y) {
        Vector3 pos;

        int posX = (int)chunkPos.x + x * CM.QuadSide;
        int posY = (int)chunkPos.y + y * CM.QuadSide;

        float biome = CM.NoisePerlin.GetNoise(posX * 0.05f, posY * 0.05f);

        if (new Range(0.4f, 1f).IsIn(biome)) {

            float z = CM.NoisePerlin.GetNoise(posX * 0.1f, posY * 0.1f) * 700 + 200;
            float z2 = CM.NoisePerlin.GetNoise(posX * 0.011f, posY * 0.011f) * 300;
            pos = new Vector3(posX, posY, z + z2);

        } else if (new Range(0.2f, 0.4f).IsIn(biome)) {

            float z = CM.NoisePerlin.GetNoise(posX * 0.011f, posY * 0.011f) * 300;
            float z2 = CM.NoisePerlin.GetNoise(posX * 0.06f, posY * 0.06f) * 200;
            pos = new Vector3(posX, posY, z + z2);

        } else if (new Range(-0.4f, 0.2f).IsIn(biome)) {

            float z = CM.NoisePerlin.GetNoise(posX * 0.06f, posY * 0.06f) * 200;
            pos = new Vector3(posX, posY, z);

        } else {
            float z = CM.NoisePerlin.GetNoise(posX * 0.1f, posY * 0.1f) * 20;
            pos = new Vector3(posX, posY, z);
        }

        return pos;
    }

    public virtual int VertexIndice(int x, int y) {
        return 0;
    }

    public virtual int VertexBlend(int x, int y) {
        return 0;
    }
}

public class Range {
    public float lower;
    public float upper;

    public Range(float lower, float upper) {
        this.lower = lower;
        this.upper = upper;
    }

    public bool IsIn(float val) {
        return lower < val && upper > val;
    }
    public bool IsOut(float val) {
        return lower > val || upper < val;
    }
    public bool IsUnder(float val) {
        return val < lower;
    }
    public bool IsOver(float val) {
        return val > upper;
    }
}