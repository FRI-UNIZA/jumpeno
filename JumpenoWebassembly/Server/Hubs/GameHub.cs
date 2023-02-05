using JumpenoWebassembly.Server.Components.Jumpeno.Entities;
using JumpenoWebassembly.Server.Services;
using JumpenoWebassembly.Shared;
using JumpenoWebassembly.Shared.Constants;
using JumpenoWebassembly.Shared.Jumpeno;
using JumpenoWebassembly.Shared.Jumpeno.Game;
using JumpenoWebassembly.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JumpenoWebassembly.Server.Hubs
{
    /// <summary>
    /// Hub pre client-server komunikaciu pocas priebehu hry.
    /// </summary>
    public class GameHub : Hub
    {
        private readonly GameService _gameService;
        private readonly IUserService _userService;
        private readonly Random _random;
        private readonly ILogger<GameHub> _logger;

        public GameHub(GameService gameService, IUserService userService, ILogger<GameHub> logger)
        {
            _logger = logger;
            _gameService = gameService;
            _userService = userService;
            _random = new Random();
        }

        [HubMethodName(GameHubC.ConnectToLobby)]
        public async Task ConnectToLobby(string code)
        {
            if (!_gameService.ExistGame(code))
            {
                await Clients.Caller.SendAsync(GameHubC.WrongGameCode);
                return;
            };
            if (_gameService.GameInProgress(code))
            {
                await Clients.Caller.SendAsync(GameHubC.GameInProgress);
                return;
            }

            var user = await _userService.GetUser();
            var authMethod = Context.User.FindFirstValue(ClaimTypes.AuthenticationMethod);
            if (authMethod == AuthenticationMethod.Spectator)
            {
                await _gameService.ConnectToSpectate(user.Id, code, Context.ConnectionId);
                return;
            }

            var player = new Player
            {
                Id = user.Id,
                Name = user.Username,
                Skin = user.Skin ?? Skins.Names[_random.Next(Skins.Names.Length)],
                Statistics = new UserStatistics()
            };

            var result = await _gameService.ConnectToPlay(player, code, Context.ConnectionId);
            if (!result)
            {
                await Clients.Caller.SendAsync(GameHubC.LobbyFull);
            }
        }

        [HubMethodName(GameHubC.ChangeLobbyInfo)]
        public async Task ChangeLobbyInfo(LobbyInfo info)
        {
            var user = await _userService.GetUser();
            await _gameService.ChangeLobbyInfo(info, user.Id);
        }

        [HubMethodName(GameHubC.ChangeGameplayInfo)]
        public async Task ChangeGameplayInfo(GameplayInfo info)
        {
            var user = await _userService.GetUser();
            await _gameService.ChangeGameplayInfo(info, user.Id);
        }

        [HubMethodName(GameHubC.StartGame)]
        public async Task StartGame()
        {
            var user = await _userService.GetUser();
            await _gameService.StartGame(user.Id);
        }

        [HubMethodName(GameHubC.DeleteGame)]
        public async Task DeleteGame()
        {
            var user = await _userService.GetUser();
            await _userService.SaveGame(_gameService.GetPlayers(_gameService.GetGameCode(user.Id)));
            await _gameService.DeleteGame(user.Id);
        }

        [HubMethodName(GameHubC.LeaveLobby)]
        public async Task LeaveLobby()
        {
            var user = await _userService.GetUser();
            await _userService.SaveGameUser(_gameService.GetPlayer(user.Id));
            await _gameService.DeleteGameIfLeavingEmpty(user.Id);
        }

        [HubMethodName(GameHubC.ChangePlayerMovement)]
        public async Task ChangePlayerMovement(Enums.MovementDirection direction, bool value)
        {
            var user = await _userService.GetUser();
            await _gameService.ChangePlayerMovement(user.Id, direction, value);
        }
        
        [HubMethodName(GameHubC.SendMessage)]
        public async Task SendMessage(Message message)
        {
            var code = _gameService.GetGameCode(message.UserId);
            await Clients.Group(code).SendAsync(GameHubC.ReceiveMessage, message);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = await _userService.GetUser();
            await _userService.SaveGameUser(_gameService.GetPlayer(user.Id));
            await _gameService.DeleteGameIfLeavingEmpty(user.Id);
            var gameCode = await _gameService.RemovePlayer(user.Id);
            if (!String.IsNullOrWhiteSpace(gameCode))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameCode);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
