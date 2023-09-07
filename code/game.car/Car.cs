using Sandbox;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;

namespace CarGame;

public interface ICar {
    public Vector3 ColumnTop { get; set; }
    public float DamperStart { get; set; }
    public float DamperStrength { get; set; }
    public float SpringStrength { get; set; }

    public float Torque { get; set; }
    public float Horsepower { get; set; }
    public int Redline { get; set; }
}

[Prefab]
public partial class Car : AnimatedEntity, ICar, IUse {
    [Prefab, Net] public Vector3 ColumnTop { get; set; }
    [Prefab, Net] public float DamperStart { get; set; }
    [Prefab, Net] public float DamperStrength { get; set; }
    [Prefab, Net] public float SpringStrength { get; set; }

    [Prefab, Net] public float Torque { get; set; }
    [Prefab, Net] public float Horsepower { get; set; }
    [Prefab, Net] public int Redline { get; set; }

    [Prefab, Net] public List<Wheel> Wheels { get; set; } = new();
    public List<Wheel> SteeringWheels { get; set; } = new();
    public List<Wheel> PoweredWheels { get; set; } = new();

    public bool active = false;
    public bool broken = false;

    // spawn is called before anything else, even constructor 
    // need to delay to set and use data
    public override void Spawn() => DelaySpawn();
    public override void ClientSpawn() => DelayClientSpawn();
    public async void DelaySpawn() {
        await GameTask.DelayRealtime(20);
        Predictable = true;
        SetupPhysicsFromModel(PhysicsMotionType.Dynamic);

        // ! remove when wheels can be added in prefab editor, currently bugged
        var x = new Vector3(52, 0, 0);
        var y = new Vector3(0, 36, 0);
        var z = new Vector3(0, 0, 6);
        Wheels.Add(new Wheel() { Powered = false, Steer = true, LocalRotation = Rotation.FromPitch(0), LocalPosition = x + y + z });
        Wheels.Add(new Wheel() { Powered = false, Steer = true, LocalRotation = Rotation.FromPitch(0), LocalPosition = x + -y + z });
        Wheels.Add(new Wheel() { Powered = true, Steer = false, LocalRotation = Rotation.FromPitch(0), LocalPosition = -x + y + z });
        Wheels.Add(new Wheel() { Powered = true, Steer = false, LocalRotation = Rotation.FromPitch(0), LocalPosition = -x + -y + z });

        foreach (var wheel in Wheels) {
            if (wheel.Powered) PoweredWheels.Add(wheel);
            if (wheel.Steer) SteeringWheels.Add(wheel);
        }
    }
    public async void DelayClientSpawn() {
        await GameTask.DelayRealtime(20);
        Predictable = true;
        SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
    }

    public void Drive(float direction) {
        foreach (var wheel in PoweredWheels) {
            if (wheel.Grounded) {
                var ts = wheel.GetTransform(Position, Rotation);
                // ! not applying equally in all directions
                PhysicsBody.ApplyForceAt(ts.Position, ts.Rotation.Forward * (direction * 600000));
            }
        }
    }

    public void Steer(float direction) {
        PhysicsBody.Rotation = Rotation.RotateAroundAxis(Vector3.Up, direction);

        foreach (var wheel in SteeringWheels) {
            wheel.LocalRotation = Rotation.Lerp(wheel.LocalRotation, wheel.LocalRotation + Rotation.FromYaw(direction * 45), 0.07f);
        }
    }

    [GameEvent.Physics.PreStep]
    public void Physics() {
        foreach (var wheel in Wheels) {
            var ts = wheel.GetTransform(Position, Rotation);
            var tr = Trace.Ray(ts.Position, ts.Position + ts.Rotation.Down * 50).Ignore(this).Run();
            if (!tr.Hit) {
                wheel.Grounded = false;
                continue;
            } else wheel.Grounded = true;

            var vel = PhysicsBody.GetVelocityAtPoint(ts.Position); // ! velocity does not account for rotation

            // suspension forces
            // var suspensiondamp
            PhysicsBody.ApplyForceAt(ts.Position, (50 - tr.Distance) * ts.Rotation.Up * 10000);

            // wheel/tire forces
            var force = Vector3.Zero;

            var tireAngularThreshold = 50;
            force.y = tireAngularThreshold > vel.y ? -vel.y * 0.4f : -vel.y * 0.1f;

            var tireLinearThreshold = 50;
            force.x = tireLinearThreshold > vel.x ? -vel.x * 0.4f : -vel.x * 0.1f;

            PhysicsBody.ApplyForceAt(ts.Position, force * 9000);

            DebugOverlay.TraceResult(tr, 0.016f);
            DebugOverlay.Line(ts.Position, ts.Position + ts.Rotation.Forward * 20, Color.White, 0.016f, false);
        }
    }

    public bool IsUsable(Entity user) {
        if (!active) return true;
        return false;
    }

    public bool OnUse(Entity user) {
        Player plr = (Player)user;
        var c = (CarController)plr.ChangeController<CarController>();
        c.Car = this;
        return true;
    }
}