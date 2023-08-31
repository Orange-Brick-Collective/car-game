using Sandbox;
using System;
using System.Linq;

namespace CarGame;

public partial class Player : AnimatedEntity {
    [Net, Predicted] public Controller Controller { get; set; }
    [ClientInput] public Vector3 InputDirection { get; set; }
    [ClientInput] public Angles ViewAngles { get; set; }

    public override void Spawn() {
        base.Spawn();

        ChangeController<WalkController>();
        SetModel("models/player.vmdl");
        SetupPhysicsFromModel(PhysicsMotionType.Keyframed);

        EnableDrawing = true;
        EnableHideInFirstPerson = true;
        EnableShadowInFirstPerson = true;
    }

    public override void BuildInput() {
        Controller?.BuildInput();
    }

    public override void Simulate(IClient cl) {
        base.Simulate(cl);
        Controller?.Simulate(cl);
    }

    public override void FrameSimulate(IClient cl) {
        base.FrameSimulate(cl);
        Controller?.FrameSimulate(cl);
    }

    public Controller ChangeController<T>() where T : Controller, new() {
        Controller = new T().Init(this);
        return Controller;
    }
}
