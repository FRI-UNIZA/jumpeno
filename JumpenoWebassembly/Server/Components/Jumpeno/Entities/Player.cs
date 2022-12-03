using JumpenoWebassembly.Server.Logging;
using JumpenoWebassembly.Shared.Jumpeno;
using JumpenoWebassembly.Shared.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using static JumpenoWebassembly.Shared.Jumpeno.Enums;

namespace JumpenoWebassembly.Server.Components.Jumpeno.Entities
{
    /// <summary>
    /// Reprezentuje telo hráča s ktorým sa pohybuje
    /// </summary>
    public class Player : MoveableJumpenoComponent
    {
        public long Id { get; set; }
        public bool Spectator { get; set; } = false;
        public bool Alive { get; set; }
        public bool InGame { get; set; }
        public string Skin { get; set; }
        public ILogger Logger { get; set; } 
        public UserStatistics Statistics { get; set; }


        public void SetBody()
        {
            //Animation = new Animation(Skin + ".png", new Vector2(4, 3), out Vector2 bodySize);
            Body.Size = new Vector2(64, 76);//= bodySize;
            Body.Origin = Body.Size / 2;
            State = AnimationState.Idle;
        }

        public void Die()
        {
            Alive = false;
            State = AnimationState.Dead;
            Velocity.Y = 0;
            Statistics.GameTime = (long)(DateTime.Now - Statistics.StartGameTime).TotalSeconds;
        }

        public void Win()
        {
            Statistics.Victories++;
            Statistics.GameTime = (long)(DateTime.Now - Statistics.StartGameTime).TotalSeconds;
        }

        public void Freeze()
        {
            for (int i = 0; i < Movement.Length; i++) {
                Movement[i] = false;
            }
        }

        public void TryAddJump(Enums.MovementDirection direction, bool value)
        {
            if (value && direction == Enums.MovementDirection.Jump && CanJump && !JumpIsCounted)//musi pripocitat aj pri drzani tlacidla
            {
                Logger.LogInformation(LogEvents.PlayerStatistics, Statistics.TotalJumps + ", " + JumpIsCounted);
                Statistics.TotalJumps++;
                JumpIsCounted = true;
                Logger.LogInformation(LogEvents.PlayerStatistics, "Player: " + this.Name + " add jump");
                Console.WriteLine();
            }
        }
    }
}
