using System;
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
        int posX = (int)chunkPos.x + x * CM.QuadSide;
        int posY = (int)chunkPos.y + y * CM.QuadSide;
        float posZ = 0;

        float biome = CM.NoisePerlin.GetNoise(posX * 0.05f, posY * 0.05f);

        if (new Range(0.4f, 1f).IsIn(biome)) {
            posZ += CM.NoisePerlin.GetNoise(posX * 0.1f, posY * 0.1f) * 700 + 200;
            posZ += CM.NoisePerlin.GetNoise(posX * 0.011f, posY * 0.011f) * 300;

        } else if (new Range(0.2f, 0.4f).IsIn(biome)) {

            float z1 = CM.NoisePerlin.GetNoise(posX * 0.1f, posY * 0.1f) * 700 + 200;
            z1 += CM.NoisePerlin.GetNoise(posX * 0.011f, posY * 0.011f) * 300;

            float z2 = CM.NoisePerlin.GetNoise(posX * 0.06f, posY * 0.06f) * 200;

            posZ = BiomeBlendZ(z1, z2, 0.2f, 0.4f);

        } else if (new Range(-0.4f, 0.2f).IsIn(biome)) {
            posZ += CM.NoisePerlin.GetNoise(posX * 0.06f, posY * 0.06f) * 200;

        } else {
            posZ += CM.NoisePerlin.GetNoise(posX * 0.1f, posY * 0.1f) * 20;
        }

        return new Vector3(posX, posY, posZ);
    }

    private float BiomeBlendZ(float z1, float z2, float low, float high) {
        float top = (high - low) / 1;
        float fraction = (z1 + z2 * 0.5f) * top;
        return MathX.Lerp(z1, z2, fraction);
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