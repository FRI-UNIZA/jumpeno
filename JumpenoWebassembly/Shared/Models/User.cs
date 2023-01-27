using System;
using System.ComponentModel.DataAnnotations;

namespace JumpenoWebassembly.Shared.Models
{
    /// <summary>
    /// Reprezentuje pouzivatela z databazy
    /// </summary>
    public class User
    {
        [Key]
        public long Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public bool IsConfirmed { get; set; }
        public bool IsDeleted { get; set; }
        public string Skin { get; set; }

        public UserStatistics Statistics { get; set; }
    }
}
