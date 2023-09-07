using Sandbox;
using System;
using System.Collections.Generic;
using CM = CarGame.ChunkManager;

namespace CarGame;

public partial class Chunk : Entity {
    public SceneObject SceneObject { get; set; }
    public PhysicsBody PhysicsBody { get; set; }
    public Biome Biome { get; set; } = new Biome(200, "materials/debugblend.vmat");

    public override void Spawn() {
        SceneObject = new(Game.SceneWorld, "materials/placeholder.vmdl", Transform);
        PhysicsBody = new(Game.PhysicsWorld);
        AsyncSpawn();
    }
    private async void AsyncSpawn() {
        await GameTask.DelayRealtime(200);
        GenerateModel();
    }

    internal void GenerateModel() {
        var vertex = MakeSurface();
        var indice = ToIndices(vertex);
        var vector = ToVectors(vertex);

        Log.Info("e");
        PhysicsBody.ClearShapes();
        PhysicsBody.AddMeshShape(vector, indice);

        if (Game.IsClient) {
            Material mat = Material.Load("materials/debugblend.vmat");

            Mesh mesh = new(mat);
            mesh.CreateVertexBuffer<BlendVertex>(vertex.Length, GetVertexAttribute(), vertex);
            mesh.CreateIndexBuffer(indice.Length, indice);

            ModelBuilder model = new();
            model.AddMesh(mesh);
            SceneObject.Model = model.Create();
        }
    }

    private BlendVertex[] MakeSurface() {
        List<BlendVertex> list = new();

        for (int x = -CM.HSize; x <= CM.HSize; x++) {
            for (int y = -CM.HSize; y <= CM.HSize; y++) {
                Vector3 pos = Biome.VertexPos(Position, x, y);

                if (Game.IsServer) {
                    list.Add(new BlendVertex() {
                        Position = pos,
                        Normal = new Vector3(0, 0, 0),
                        Tangent = new Vector4(0, 0, 0, 0),
                        TexCoord0 = new Vector2(0, 0),
                        Color = new Vector3(0, 0, 0),
                        BlendIndices = new Vector4(0, 0, 0, 0),
                        BlendWeights = new Vector4(0, 0, 0, 0),
                    });
                } else {
                    list.Add(new BlendVertex() {
                        Position = pos,
                        Normal = new Vector3(0, 0, 1), // 0 0 1
                        Tangent = new Vector4(0, 1, 0, 1), // 0 1 0 1
                        TexCoord0 = new Vector2(pos.x * 0.01f, pos.y * 0.01f),
                        Color = new Vector3(1, 1, 1),
                        BlendIndices = new Vector4(RandInt(), 0, 0, 0), // ! NOT WORKING
                        BlendWeights = new Vector4(RandFlt(), 0, 0, 0), // ! NOT WORKING
                    });
                }
            }
        }

        return list.ToArray();
    }

    private static float RandFlt() {
        return Random.Shared.Float(0, 1);
    }

    private static int RandInt() {
        return Random.Shared.Int(0, 4);
    }

    private static Vector3[] ToVectors(BlendVertex[] surface) {
        List<Vector3> list = new();

        foreach (var vert in surface) {
            list.Add(vert.Position);
        }

        return list.ToArray();
    }

    private static int[] ToIndices(BlendVertex[] surface) {
        List<int> list = new();

        for (int i = 0; i < surface.Length - CM.VSize - 1; i++) {
            // stop wraparound
            if (i % CM.VSize == CM.VSize - 1) continue;

            // weird but indice ordering has to be specific
            list.AddRange(new int[] {
                i + CM.VSize + 1,
                i + 1,
                i,
                i + CM.VSize + 1,
                i,
                i + CM.VSize,
            });
        }

        return list.ToArray();
    }

    private struct BlendVertex {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector4 Tangent;
        public Vector2 TexCoord0;
        public Vector3 Color;
        public Vector4 BlendIndices;
        public Vector4 BlendWeights;
    }



    private static VertexAttribute[] GetVertexAttribute() {
        List<VertexAttribute> list = new() {
            new VertexAttribute( // vec3 float
                VertexAttributeType.Position,
                VertexAttributeFormat.Float32,
                3,
                0
            ),
            new VertexAttribute( // vec3 float (maybe)
                VertexAttributeType.Normal,
                VertexAttributeFormat.Float32,
                3,
                0
            ),
            new VertexAttribute( // vec4 float
                VertexAttributeType.Tangent,
                VertexAttributeFormat.Float32,
                4,
                0
            ),
            new VertexAttribute( // vec2 float (maybe)
                VertexAttributeType.TexCoord,
                VertexAttributeFormat.Float32,
                2,
                0
            ),
            new VertexAttribute( // vec3 float
                VertexAttributeType.Color,
                VertexAttributeFormat.Float32,
                3,
                0
            ),
            new VertexAttribute( // something
                VertexAttributeType.BlendIndices,
                VertexAttributeFormat.UInt32,
                4,
                0
            ),
            new VertexAttribute( // vec4 float (maybe)
                VertexAttributeType.BlendWeights,
                VertexAttributeFormat.Float32,
                4,
                0
            )
        };

        return list.ToArray();
    }

    private void DebugBox(Vector3 pos, int size = 2, Color? color = null) {
        DebugOverlay.Box(pos, Rotation, -new Vector3(size), new Vector3(size), color ?? Color.White, 10, false);
    }

    public void Dispose() {
        SceneObject.Delete();
        PhysicsBody.Remove();
        Delete();
    }
}