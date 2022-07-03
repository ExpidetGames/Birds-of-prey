public class Client {
    public string id;
    public string name;
    public string teamColor;
    public int playerHealth;
    public bool isReady;
    public PlaneTypes planeType;

    public Client(string id, string name, string teamColor, bool isReady, int playerHealth, PlaneTypes planeType) {
        this.id = id;
        this.name = name;
        this.teamColor = teamColor;
        this.isReady = isReady;
        this.planeType = planeType;
        this.playerHealth = playerHealth;
    }
}
