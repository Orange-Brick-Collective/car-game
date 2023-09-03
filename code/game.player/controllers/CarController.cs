using Sandbox;

namespace CarGame;

public partial class CarController : Controller {
    [Net] public Car Car { get; set; }
    public TimeSince CameraControl { get; set; } = 0;

    public override void BuildInput() {
        Plr.InputDirection = Input.AnalogMove;
    }

    public override void Simulate(IClient cl) {
        Car.Drive(Plr.InputDirection.x);
        Car.Steer(Plr.InputDirection.y);

        if (Input.Pressed("use")) {
            Plr.ChangeController<WalkController>();
        }
    }

    public override void FrameSimulate(IClient cl) {
        Camera.FieldOfView = Screen.CreateVerticalFieldOfView(Game.Preferences.FieldOfView);
        Camera.FirstPersonViewer = null;

        //if (CameraControl > 5) {
        Camera.Position = Vector3.Lerp(Camera.Position, Car.Position + Camera.Rotation.Backward * 160 + Vector3.Up * 100, Time.Delta * 4);
        var carRot = Car.Rotation.Angles().WithPitch(0).WithRoll(0).ToRotation();
        Camera.Rotation = Rotation.Lerp(Camera.Rotation, carRot, Time.Delta * 2);
        //} else {

        //}

    }
}