using Sandbox;
using System;
using System.Linq;

public partial class Player : AnimatedEntity {
    [Net, Predicted] public Controller controller { get; set; }
    [ClientInput] public Vector3 InputDirection { get; set; }
    [ClientInput] public Angles ViewAngles { get; set; }

    public override void Spawn() {
        base.Spawn();

        controller = new WalkController();
        Model = Cloud.Model("https://asset.party/facepunch/watermelon");

        EnableDrawing = true;
        EnableHideInFirstPerson = true;
        EnableShadowInFirstPerson = true;
    }


    public override void BuildInput() {

    }

    /// <summary>
    /// Called every tick, clientside and serverside.
    /// </summary>
    public override void Simulate(IClient cl) {
        base.Simulate(cl);


    }

    /// <summary>
    /// Called every frame on the client
    /// </summary>
    public override void FrameSimulate(IClient cl) {
        base.FrameSimulate(cl);


    }
}
