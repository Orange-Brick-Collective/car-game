namespace CarGame;

public class Text : Sandbox.UI.Label {
    private static readonly Color32[] staticColor = { new Color32(12, 118, 148), new Color32(20, 200, 255) };

    public enum Wght {
        Black,
        ExtraBold,
        Bold,
        SemiBold,
        Medium,
        Regular,
        Light,
        ExtraLight,
        Thin,
    }

    public enum Dir {
        Up,
        Left,
        Circle
    }

    public Text(string text, Color32[] gradient = null, Dir gradientDir = Dir.Up, Wght fontWeight = Wght.Regular) {
        SetText(text);

        gradient ??= staticColor;

        string colors = $"{gradient[0].Hex} 25%";

        float spread = 50 / gradient.Length;
        for (int i = 1; i < gradient.Length; i++) {
            colors += $", {gradient[i].Hex} {spread * (i + 1) + 25}%";
        }

        Style.FontFamily = $"SairaSemiCondensed-{fontWeight.ToString()}";
        Style.TextStrokeWidth = 3;
        Style.TextStrokeColor = Color.Black;

        switch (gradientDir) {
            case Dir.Up: {
                    Log.Info($"linear-gradient({colors})");
                    SetProperty("color", $"linear-gradient({colors})");
                    break;
                }
            case Dir.Left: {
                    SetProperty("color", $"linear-gradient(deg(90), {colors})");
                    break;
                }
            case Dir.Circle: {
                    SetProperty("color", $"radial-gradient(circle, {colors})");
                    break;
                }
        }
    }

}