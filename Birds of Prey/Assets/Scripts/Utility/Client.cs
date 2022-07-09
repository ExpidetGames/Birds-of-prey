public class Client {
    public string id;
    public string name;
    public string teamColor;
    public bool isReady;
    public int playerHealth;
    public PlaneTypes[] planeTypes;

    public Client(string id, string name, string teamColor, bool isReady, int playerHealth, PlaneTypes[] planeTypes) {
        this.id = id;
        this.name = name;
        this.teamColor = teamColor;
        this.isReady = isReady;
        this.planeTypes = (PlaneTypes[])planeTypes.Clone();
        this.playerHealth = playerHealth;
    }
}
