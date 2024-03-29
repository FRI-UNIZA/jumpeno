﻿using Blazored.LocalStorage;
using JumpenoWebassembly.Shared;
using JumpenoWebassembly.Shared.Constants;
using JumpenoWebassembly.Shared.Jumpeno;
using JumpenoWebassembly.Shared.Jumpeno.Entities;
using JumpenoWebassembly.Shared.Jumpeno.Game;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JumpenoWebassembly.Client.Shared
{
    public enum LobbySection
    {
        Info,
        Players,
        Chat
    }
    public partial class Lobby
    {
        [Parameter] public Player Player { get; set; }
        [Parameter] public LobbyInfo Info { get; set; }
        [Parameter] public GameSettings Settings { get; set; }
        [Parameter] public List<Player> Players { get; set; }
        [Parameter] public HubConnection Hub { get; set; }
        [Parameter] public Func<string, Task> OnMessageSend { get; set; }

        [Inject] public NavigationManager Navigation { get; set; }
        [Inject] public ILocalStorageService Storage { get; set; }
        [Inject] public Services.IAuthService AuthService { get; set; }


        private List<Message> _messages = new List<Message>();
        private LobbySection _section = LobbySection.Info;

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                Console.WriteLine("Hub message hooked!");
                Hub.On<Message>(GameHubC.ReceiveMessage, (message) =>
                {
                    _messages.Add(message);
                    StateHasChanged();
                });
            }
        }

        private async Task SwitchTimer()
        {
            Info.StoppedStartTimer = !Info.StoppedStartTimer;
            await Hub.InvokeAsync(GameHubC.ChangeLobbyInfo, Info);
        }

        private async Task SwitchStartTimer()
        {
            Info.StartTimerRunning = !Info.StartTimerRunning;
            await Hub.InvokeAsync(GameHubC.ChangeLobbyInfo, Info);
        }

        private async Task StartGame()
        {
            await Hub.InvokeAsync(GameHubC.StartGame);
        }

        private async Task DeleteGame()
        {
            await Hub.InvokeAsync(GameHubC.DeleteGame);
        }

        private async Task LeaveLobby()
        {
            if (!Player.Spectator)
            {
                await Hub.InvokeAsync(GameHubC.LeaveLobby);
            } else
            {
                await AuthService.Logout();
            }
            await Storage.RemoveItemAsync("code");
            Navigation.NavigateTo("/", true);
        }

        private async Task SendMessage(string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                return;
            }
            var msg = new Message { Text = message, User = Player.Name, UserId = Player.Id};
            //_messages.Add(msg);

            //msg.User = Player.Name;
            await Hub.InvokeAsync(GameHubC.SendMessage, msg);
        }
    }
}
