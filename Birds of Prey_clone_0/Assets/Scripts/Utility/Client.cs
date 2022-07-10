public class Client {
    public string id;
    public string name;
    public string teamColor;
    public bool isReady;
    public bool isDead;
    public int playerHealth;
    public PlaneTypes[] planeTypes;
    public int currentPlaneType;

    public Client(string id, string name, string teamColor, bool isReady, bool isDead, int playerHealth, PlaneTypes[] planeTypes, int startingPlaneType) {
        this.id = id;
        this.name = name;
        this.teamColor = teamColor;
        this.isReady = isReady;
        this.isDead = isDead;
        this.planeTypes = (PlaneTypes[])planeTypes.Clone();
        this.playerHealth = playerHealth;
        this.currentPlaneType = startingPlaneType;
    }

    public PlaneTypes getCurrentType() {
        return planeTypes[this.currentPlaneType];
    }
}
