namespace Jumpeno.Server.Utils;

public class GameEngine {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private Game Game { get; set; }
    public string Code => Game.Code;
    public double Time => Game.Time;
    public GAME_STATE State => Game.State;
    public byte Capacity => Game.Capacity;

    // Locks ------------------------------------------------------------------------------------------------------------------------------
    private readonly Semaphore Lock = new(1, 1);

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public GameEngine(string code, byte capacity) {
        Game = new Game(code, capacity);
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public Game ClientGameCopy() { return Game; }
    
    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task<Player> AddPlayer(string connectionID, User user) {
        Player player;
        try {
            Lock.WaitOne();
            player = Game.AddPlayer(connectionID, user);
        } finally {
            Lock.Release();
        }
        await GameHub.GamePlayerUpdate(this, player, PLAYER_ACTION.JOIN);
        return player;
    }

    public async Task RemovePlayer(Player player) {
        Lock.WaitOne();
        Game.RemovePlayer(player);
        Lock.Release();
        await GameHub.GamePlayerUpdate(this, player, PLAYER_ACTION.LEAVE);
    }
}
