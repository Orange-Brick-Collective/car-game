using Sandbox;

public class CarSettings {
    public float WheelBaseLength = 700, WheelBaseWidth = 320;
};

public class Car : AnimatedEntity, IUse {
    public CarSettings settings;
    public bool active = false;

    public void Init(CarSettings settings) {
        this.settings = settings;
    }
    
    public void CreateWheels() {

    }

    public bool IsUsable(Entity user) {
        if (!active) return true;
        return false;
    }

    public bool OnUse(Entity user) {
        Player p = (Player)user;

        return true;
    }
}