using System;
using Sandbox;

namespace cargame;

public partial class WalkController : Controller {
    [Net, Predicted] bool IsGrounded { get; set; } = false;
    public Vector3 ViewPosition => Plr.Position + (Vector3.Up * 61);

    public override void BuildInput() {
        Plr.InputDirection = Input.AnalogMove;
        Angles viewAngles = Plr.ViewAngles;
        viewAngles += Input.AnalogLook;
        viewAngles.pitch = viewAngles.pitch.Clamp(-82f, 82f);
        viewAngles.roll = 0f;
        Plr.ViewAngles = viewAngles.Normal;
    }

    public override void Simulate(IClient cl) {
        Plr.Rotation = Plr.ViewAngles.WithPitch(0).ToRotation();

        var groundedVec = Plr.Transform.WithPosition(Plr.Position + (Vector3.Down * 2));
        var groundedTr = Trace.Sweep(Plr.PhysicsBody, Plr.Transform, groundedVec).Ignore(Plr).Run();
        IsGrounded = groundedTr.Hit;

        if (IsGrounded) {
            SimulateGrounded();
        } else {
            SimulateAir();
        }

        if (Input.Pressed("attack1")) {
            _ = new Car() { Data = new CarData() { ModelPath = "models/debugcar.vmdl" } };
        }

        if (Input.Pressed("use")) {
            var tr = Trace.Ray(ViewPosition, ViewPosition + (Plr.ViewAngles.Forward * 150)).Ignore(Plr).Run();
            DebugOverlay.TraceResult(tr, 1);
            if (tr.Hit && tr.Entity is IUse use) {
                if (use.IsUsable(Plr)) use.OnUse(Plr);
            }
        }
    }

    private void SimulateGrounded() {
        var movement = Plr.InputDirection.Normal;
        Plr.Velocity = Plr.Rotation * movement;
        Plr.Velocity *= Input.Down("run") ? 500 : 200;

        var stepVec = Plr.Transform.WithPosition(Plr.Position + (Plr.Velocity.Normal * 2));
        var stepTr = Trace.Sweep(Plr.PhysicsBody, stepVec.Add(Vector3.Up * 16, true), stepVec).Ignore(Plr).Run();
        if (stepTr.Hit && !stepTr.StartedSolid) {
            Plr.Position = Plr.Position.WithZ(stepTr.HitPosition.z);
        }

        MoveHelper helper = new(Plr.Position, Plr.Velocity) {
            Trace = Trace.Body(Plr.PhysicsBody, Plr.Position).Ignore(Plr)
        };

        if (helper.TryMove(Time.Delta) > 0) {
            Plr.Position = helper.Position;
        }
    }
    private void SimulateAir() {
        var movement = Plr.InputDirection.Normal;
        Plr.Velocity = Plr.Rotation * movement;
        Plr.Velocity += Vector3.Down * 5;
        Plr.Velocity *= 40;

        MoveHelper helper = new(Plr.Position, Plr.Velocity) {
            Trace = Trace.Body(Plr.PhysicsBody, Plr.Position).Ignore(Plr)
        };

        if (helper.TryMove(Time.Delta) > 0) {
            Plr.Position = helper.Position;
        }
    }

    public override void FrameSimulate(IClient cl) {
        Plr.Rotation = Plr.ViewAngles.WithPitch(0).ToRotation();

        Camera.Position = ViewPosition;
        Camera.Rotation = Plr.ViewAngles.ToRotation();
        Camera.FieldOfView = Screen.CreateVerticalFieldOfView(Game.Preferences.FieldOfView);
        Camera.FirstPersonViewer = Plr;
    }
}