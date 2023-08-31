using Sandbox;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CarGame;

public partial class ChunkManager : BaseNetworkable {
    internal static ChunkManager Current;
    internal static readonly FastNoiseLite NoisePerlin = new();
    internal static readonly FastNoiseLite NoiseSimplex = new();
    internal static int Size = 24; // chunk block size (16x16) NEEDS TO BE EVEN
    internal static int HSize = Size / 2; // half chunk size, for iterating
    internal static int VSize = Size + 1; // verticle size (chunk size + 1, 17x17)

    [Net] public int Seed { get; set; } = 0;

    public ChunkManager() {
        if (Current is not null) return;
        Current = this;
        NoisePerlin.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        NoiseSimplex.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        SpawnChunks();
    }

    public static async void SpawnChunks() {
        ChangeSeed();
        await GameTask.DelayRealtime(1000);

        SpawnChunksClient(Current.Seed);
        foreach (var e in Entity.All.OfType<Chunk>()) e.Dispose();

        for (int x = -8; x <= 8; x++) {
            for (int y = -8; y <= 8; y++) {
                NewChunk(new Vector3(x, y, 0));
            }
        }
    }

    [ClientRpc]
    public static void SpawnChunksClient(int seed) {
        Log.Info(Current);
        Current.Seed = seed;
        NoisePerlin.SetSeed(Current.Seed);
        NoiseSimplex.SetSeed(Current.Seed);

        foreach (var e in Entity.All.OfType<Chunk>()) e.Dispose();

        for (int x = -8; x <= 8; x++) {
            for (int y = -8; y <= 8; y++) {
                NewChunk(new Vector3(x, y, 0));
            }
        }
    }

    public static void NewChunk(Vector3 chunkPosition) {
        _ = new Chunk() {
            Position = ChunkCoords(chunkPosition),
            Transmit = TransmitType.Never,
        };
    }

    public static Vector3 ChunkCoords(Vector3 position) {
        return position * Size * 100;
    }

    public static void ChangeSeed() {
        Current.Seed = Random.Shared.Int(0, 2100000000);
        NoisePerlin.SetSeed(Current.Seed);
        NoiseSimplex.SetSeed(Current.Seed);
    }
}