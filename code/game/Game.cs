using Sandbox;
using System;
using System.Linq;

namespace CarGame;

public partial class MyGame : GameManager {
    public ChunkManager ChunkManager { get; set; }

    public MyGame() {
        ChunkManager = new ChunkManager();

        if (Game.IsServer) {
            _ = new ModelEntity("models/sky.vmdl") {
                Name = "WorldSkyEntity",
            };
            _ = new ModelEntity("models/debug.vmdl") {
                Name = "WorldDebugBlendEntity",
                Position = Vector3.Up * 1500,
            };
            _ = new SceneSunLight(Game.SceneWorld, Rotation.From(new Angles(82, 4, 0)), Color.White);
        }

        if (Game.IsClient) {

        }
    }

    public override void ClientJoined(IClient client) {
        base.ClientJoined(client);

        var pawn = new Player();
        client.Pawn = pawn;
        pawn.Position = Vector3.Up * 1000.0f;
    }
}
