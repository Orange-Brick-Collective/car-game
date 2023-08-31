using Sandbox;
using System;
using System.Linq;

namespace CarGame;

internal static class Console {
    [ConCmd.Server("cg_regenchunks")]
    private static void RegenChunksCMD() {
        DateTime time = DateTime.Now;

        ChunkManager.SpawnChunks();

        Log.Info($"Completed in {DateTime.Now - time}");
    }

    [ConCmd.Server("cg_respawn")]
    private static void RespawnCMD() {
        ((Player)ConsoleSystem.Caller.Pawn).Position = Vector3.Up * 1000;
    }
}