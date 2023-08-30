using Sandbox;

public class WalkController : Controller {
    public Player plr;

    public void Init(Player plr) {
        this.plr = plr;
    }

    public override void BuildInput() {
        plr.InputDirection = Input.AnalogMove;

        var look = Input.AnalogLook;

        var viewAngles = plr.ViewAngles;
        viewAngles += look;
        plr.ViewAngles = viewAngles.Normal;
    }
    public override void Simulate(IClient cl) {
        plr.Rotation = plr.ViewAngles.ToRotation();

        var movement = plr.InputDirection.Normal;
        plr.Velocity = plr.Rotation * movement;
        plr.Velocity *= Input.Down("run") ? 1000 : 200;

        MoveHelper helper = new MoveHelper(plr.Position, plr.Velocity);
        helper.Trace = helper.Trace.Size(16);
        if (helper.TryMove(Time.Delta) > 0) {
            plr.Position = helper.Position;
        }
    }
    public override void FrameSimulate(IClient cl) {
        plr.Rotation = plr.ViewAngles.ToRotation();

        Camera.Position = plr.Position;
        Camera.Rotation = plr.Rotation;
        Camera.FieldOfView = Screen.CreateVerticalFieldOfView(Game.Preferences.FieldOfView);
        Camera.FirstPersonViewer = plr;
    }
}