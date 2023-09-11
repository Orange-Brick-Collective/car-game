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

        var camPos = Car.Position + Camera.Rotation.Backward * 160 + Vector3.Up * 100;
        if (CameraControl > 5) {
            CameraAngles = Angles.Zero;

            var camRot = Car.Rotation.Angles().WithPitch(0).WithRoll(0).ToRotation();

            Camera.Rotation = Rotation.Lerp(Camera.Rotation, camRot, Time.Delta * 2);
            Camera.Position = Vector3.Lerp(Camera.Position, camPos, Time.Delta * 5);
        } else {
            CameraAngles += new Angles(Input.AnalogLook.pitch, Input.AnalogLook.yaw, 0);
            CameraAngles.pitch = CameraAngles.pitch.Clamp(-60, 60);

            var addedRot = Car.Rotation.Angles().WithPitch(0) + CameraAngles;
            var camRot = addedRot.WithRoll(0).ToRotation();

            Camera.Rotation = camRot;
            Camera.Position = Vector3.Lerp(Camera.Position, camPos, Time.Delta * 50);
        }
    }
}