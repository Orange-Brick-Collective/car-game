using Sandbox;

public partial class Controller {

    public virtual void BuildInput() { }
    public virtual void Simulate(IClient cl) { }
    public virtual void FrameSimulate(IClient cl) { }
}