namespace Neuro.Vision.DataStructures;

public class LastSeenPlayer
{
    public string location; // TODO: location == "" is used to check various things across the project. this should be changed
    public float time;
    public bool dead;
    public PlayerControl[] witnesses;
    public float gameTimeVisible; // Total time that we've been able to see this player for
    public float roundTimeVisible; // Total time this round that we've been able to see this player for
    public bool sawVent; // If we've caught this player venting in front of us before

    public LastSeenPlayer(string location, float time, bool dead)
    {
        this.location = location;
        this.time = time;
        this.dead = dead;
        this.gameTimeVisible = 0f;
        this.roundTimeVisible = 0f;
        this.sawVent = false;
        this.witnesses = null;
    }
}
