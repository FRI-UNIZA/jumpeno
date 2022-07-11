using JumpenoWebassembly.Shared.Jumpeno;
using JumpenoWebassembly.Shared.Models;
using System;
using System.Numerics;
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
                Console.WriteLine(Statistics.TotalJumps + ", " + JumpIsCounted);
                Statistics.TotalJumps++;
                JumpIsCounted = true;
                Console.WriteLine("Player add jump");
            }
        }
    }
}
