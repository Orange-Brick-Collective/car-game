using Sandbox;

namespace cargame;

public partial class CarController : Controller {
    [Net] public Car Car { get; set; }

    public override void BuildInput() {
        Plr.InputDirection = Input.AnalogMove;


    }

    public override void Simulate(IClient cl) {
        Car.Position += Plr.InputDirection;

        var angle = Car.Rotation.Angles();
        var vec = Vector3.Zero;
        Car.Rotation.SmoothDamp(Car.Rotation, angle.WithYaw(angle.yaw + Plr.InputDirection.y).ToRotation(), ref vec, 1, Time.Delta);


        if (Input.Pressed("use")) {
            Plr.ChangeController<WalkController>();
        }
    }

    public override void FrameSimulate(IClient cl) {
        Camera.Position = Car.Position + Car.Rotation.Backward * 160 + Vector3.Up * 100;
        Camera.Rotation = Car.Rotation;
        Camera.FieldOfView = Screen.CreateVerticalFieldOfView(Game.Preferences.FieldOfView);
    }
}