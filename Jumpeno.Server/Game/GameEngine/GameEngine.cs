namespace Jumpeno.Server.Utils;

public class GameEngine {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private Game Game { get; set; }
    public string Code => Game.Code;
    public string Name => Game.Name;
    private readonly Updater Updater;
    private readonly Locker Lock;

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public GameEngine(string code, string name, byte capacity) {
        Game = new Game(code, name, capacity);
        Updater = new Updater();
        Lock = new Locker();
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public Game ClientGameCopy() { return Game; }
    
    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task<Player> AddPlayer(User user) {
        return await Lock.Lock(async () => {
            var player = Game.AllocatePlayer(user);
            var update = Updater.NewPlayerUpdate(player, PLAYER_ACTION.JOIN);
            var updated = Game.Update(update);
            Lock.Unlock();
            if (updated) await GameHub.SendGameUpdate(Code, update);
            return player;
        });
    }

    public async Task RemovePlayer(Player player) {
        await Lock.Lock(async () => {
            var update = Updater.NewPlayerUpdate(player, PLAYER_ACTION.LEAVE);
            var updated = Game.Update(update);
            Lock.Unlock();
            if (updated) await GameHub.SendGameUpdate(Code, update);
        });
    }

    public async Task Start() {
        await Lock.Lock(async () => {
            if (Game.State == GAME_STATE.GAMEPLAY) { Lock.Unlock(); return; }
            var update = Updater.NewGamePlayUpdate(0, GAME_STATE.GAMEPLAY);
            var updated = Game.Update(update);
            Lock.Unlock();
            if (updated) await GameHub.SendGameUpdate(Code, update);
        });
    }

    public async Task Reset() {
        await Lock.Lock(async () => {
            var update = Updater.NewGamePlayUpdate(0, GAME_STATE.LOBBY);
            var updated = Game.Update(update);
            Updater.Reset();
            Lock.Unlock();
            if (updated) await GameHub.SendException(Code, new([new(I18N.T("You have been disconnected from the server."))]));
        });
    }
}
