using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarGame;

public partial class MyGame : GameManager {
    public ChunkManager ChunkManager { get; set; }
    [Net] public Sky Sky { get; set; }

    public MyGame() {
        ChunkManager = new ChunkManager();

        if (Game.IsServer) {
            Sky = new Sky().Init();

            _ = new ModelEntity("models/debug.vmdl") {
                Name = "WorldDebugBlendEntity",
                Position = Vector3.Up * 1500,
            };
        }

        if (Game.IsClient) {

        }
    }

    public override void Simulate(IClient cl) {
        base.Simulate(cl);


    }

    public override void ClientJoined(IClient cl) {
        base.ClientJoined(cl);

        var pawn = new Player();
        cl.Pawn = pawn;
        pawn.Position = Vector3.Up * 1000.0f;

        ChunkManager.SpawnChunksClient(To.Single(cl), ChunkManager.Seed);
    }
}
