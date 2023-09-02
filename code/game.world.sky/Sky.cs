using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarGame;


public partial class Sky : BaseNetworkable {
    public static Sky Current;
    [Net] public ModelEntity ModelSky { get; set; }
    [Net] public SceneSunLight Light { get; set; }

    public Material SkyMaterial;

    public Sky Init() {
        if (Current is not null) return null;
        Current = this;
        Event.Register(this);

        Light = new SceneSunLight(Game.SceneWorld, Rotation.From(new Angles(82, 4, 0)), Color.White);
        ModelSky = new ModelEntity("models/sky.vmdl") {
            Name = "WorldSkyEntity",
        };
        SkyMaterial = ModelSky.Model.Materials.First();

        return this;
    }

    [GameEvent.Tick.Server]
    public void Tick() {
        var a = new ComputeBuffer<float>(1);
        a.SetData(new List<float>() { Random.Shared.Float(0, 1) });
        SkyMaterial.Attributes.Set("something", a);

        Light.Angles = new Angles(Time.Tick * 0.5f, 35, 0);
    }
}
