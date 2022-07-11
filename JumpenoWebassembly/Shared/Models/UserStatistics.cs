using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpenoWebassembly.Shared.Models
{
    public class UserStatistics
    {
        [Key]
        public int Id { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
        public int GamesPlayed { get; set; }
        public int Victories { get; set; }
        public int TotalScore { get; set; }
        public int TotalJumps { get; set; }
        public long GameTime { get; set; }

        public DateTime StartGameTime { get; set; }

        public UserStatistics()
        {
            GamesPlayed = 0;
            Victories = 0;
            TotalScore = 0;
            GameTime = 0;
            TotalJumps = 0;
        }
    }
}
