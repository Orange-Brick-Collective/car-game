using System;
using Sandbox;

namespace CarGame;

public partial class WalkController : Controller {
    [Net, Predicted] bool IsGrounded { get; set; } = false;
    public Vector3 ViewPosition => Plr.Position + (Vector3.Up * 61);

    public TimeSince TimeSinceJump = 0;

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

        var groundedVec = Plr.Transform.WithPosition(Plr.Position + (Vector3.Down * 5));
        var groundedTr = Trace.Sweep(Plr.PhysicsBody, Plr.Transform, groundedVec).Ignore(Plr).Run();
        IsGrounded = groundedTr.Hit;
        if (IsGrounded) {
            SimulateGrounded();
        } else {
            SimulateAir();
        }

        if (Input.Pressed("attack1") && Game.IsServer) {
            var tr = Trace.Ray(ViewPosition, ViewPosition + (Plr.ViewAngles.Forward * 150)).Ignore(Plr).Run();
            if (PrefabLibrary.TrySpawn<Entity>("prefabs/debugcar.prefab", out var car)) {
                car.Position = tr.EndPosition;
                car.Rotation = Rotation.From(new Angles(Vector3.Up));
            }
        }

        if (Input.Pressed("use") && Game.IsServer) {
            var tr = Trace.Ray(ViewPosition, ViewPosition + (Plr.ViewAngles.Forward * 150)).Ignore(Plr).Run();

            if (tr.Hit && tr.Entity is IUse use) {
                if (use.IsUsable(Plr)) use.OnUse(Plr);
            }
        }
    }

    private void SimulateGrounded() {
        var movement = Plr.InputDirection.Normal;
        Plr.Velocity = Plr.Rotation * movement;
        Plr.Velocity *= Input.Down("run") ? 500 : 200;
        Plr.Velocity += Vector3.Down;

        var stepVec = Plr.Transform.WithPosition(Plr.Position + (Plr.Velocity.Normal * 2) + Vector3.Up * 14);
        var stepTr = Trace.Sweep(Plr.PhysicsBody, stepVec, stepVec.Add(Vector3.Down * 128, true)).Ignore(Plr).Run();
        if (stepTr.Hit && !stepTr.StartedSolid) {
            Plr.Position = Plr.Position.WithZ(stepTr.HitPosition.z + 4);
        }

        MoveHelper helper = new(Plr.Position, Plr.Velocity) {
            Trace = Trace.Sphere(4, Plr.Position + Vector3.Up * 40, Plr.Position + Vector3.Up * 40).Ignore(Plr)
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
        Camera.ZFar = 160000;
        Camera.FirstPersonViewer = Plr;
    }
}