using Sandbox;

namespace CarGame;

public partial class Controller : BaseNetworkable {
    [Net] public Player Plr { get; set; }

    public virtual Controller Init(Player plr) {
        Plr = plr;
        return this;
    }
    public virtual void BuildInput() { }
    public virtual void Simulate(IClient cl) { }
    public virtual void FrameSimulate(IClient cl) { }
}