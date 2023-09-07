using Sandbox;

namespace CarGame;

public partial class CarController : Controller {
    [Net] public Car Car { get; set; }
    public TimeSince CameraControl { get; set; } = 0;
    public Angles CameraAngles = Angles.Zero;

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
        Camera.FieldOfView = Screen.CreateVerticalFieldOfView(Game.Preferences.FieldOfView + 35);
        Camera.FirstPersonViewer = null;

        if (Input.AnalogLook != Angles.Zero) CameraControl = 0;

        if (CameraControl > 5) {
            CameraAngles = Angles.Zero;
            var carRot = Car.Rotation.Angles().WithPitch(0).WithRoll(0).ToRotation();
            Camera.Rotation = Rotation.Lerp(Camera.Rotation, carRot, Time.Delta * 2);
            Camera.Position = Vector3.Lerp(Camera.Position, Car.Position + Camera.Rotation.Backward * 160 + Vector3.Up * 100, Time.Delta * 5);
        } else {
            CameraAngles += new Angles(Input.AnalogLook.pitch, Input.AnalogLook.yaw, 0);
            CameraAngles.pitch = CameraAngles.pitch.Clamp(-80, 80);
            var carRot1 = Car.Rotation.Angles().WithPitch(0) + CameraAngles;
            var carRot2 = carRot1.WithRoll(0).ToRotation();
            Camera.Rotation = Rotation.Lerp(Camera.Rotation, carRot2, Time.Delta * 2);
            Camera.Position = Vector3.Lerp(Camera.Position, Car.Position + Camera.Rotation.Backward * 160 + Vector3.Up * 100, Time.Delta * 20);
        }
    }
}