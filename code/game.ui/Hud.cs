using Sandbox;
using Sandbox.UI;

namespace CarGame;

public class Hud : HudEntity<RootPanel> {
    public Hud() {
        RootPanel.StyleSheet.Load("game.ui/Hud.scss");

        RootPanel.AddChild(new Text("This is the Test Text field FIELD", new Color32[] { Color32.Black, new Color32(255, 0, 0), Color32.White }));
    }
}