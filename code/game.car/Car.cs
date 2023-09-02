using System.IO;
using Sandbox;

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

    public Wheel[] AllWheels { get; set; }
    public Wheel[] SteeringWheels { get; set; }
    public Wheel[] PoweredWheels { get; set; }

    public bool active = false;
    public bool broken = false;

    public void Drive() {
        foreach (var wheel in PoweredWheels) {

        }
    }

    public void Steer(float direction) {

        foreach (var wheel in PoweredWheels) {

        }
    }

    // spawn is called before anything else, even constructor 
    // need to delay to set and use data
    public override void Spawn() => DelaySpawn();
    public async void DelaySpawn() {
        await GameTask.DelayRealtime(20);
        Predictable = true;
        SetupPhysicsFromModel(PhysicsMotionType.Dynamic);

        // AllWheels = new Wheel[Data.Wheels.Length];
        // int i = 0, p = 0, s = 0;
        // foreach (var data in Data.Wheels) {
        //     Wheel wheel = new(data);
        //     AllWheels[i] = wheel;

        //     if (Data.Wheels[i].Powered) {
        //         PoweredWheels[p] = wheel;
        //         p++;
        //     }

        //     if (Data.Wheels[i].Steer) {
        //         PoweredWheels[s] = wheel;
        //         s++;
        //     }

        //     i++;
        // }
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