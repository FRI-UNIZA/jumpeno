using JumpenoWebassembly.Server.Components.Jumpeno.Entities;
using JumpenoWebassembly.Server.Data;
using JumpenoWebassembly.Shared.Constants;
using JumpenoWebassembly.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JumpenoWebassembly.Server.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(DataContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContextAccessor = httpContext;
        }

        public async Task<User> GetUser()
        {
            var authMethod = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.AuthenticationMethod);
            if (authMethod == AuthenticationMethod.Anonym) {
                var id = int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var name = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                return new User { Id = id, Username = name, Statistics = new UserStatistics() };

            } else if (authMethod == AuthenticationMethod.Spectator) {
                return new User { Username = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name), Id = 0 };

            } else {
                var mail = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
                User user =  await _context.Users.FirstOrDefaultAsync(u => u.Email == mail);
                user.Statistics = await _context.UserStatistics.FirstOrDefaultAsync(u => u.UserId == user.Id);
                return user;
            }
        }

        public async Task SaveGame(List<Player> players)
        {
            foreach (var player in players)
            {
                await SaveGameUser(player);
            }
        }

        public async Task SaveGameUser(Player player)
        {
            var databaseUser = await _context.Users.FirstOrDefaultAsync(user => user.Id == player.Id);
            if (databaseUser != null)
            {
                databaseUser.Statistics = await _context.UserStatistics.FirstOrDefaultAsync(stat => stat.UserId == player.Id);
                databaseUser.Statistics.TotalJumps += player.Statistics.TotalJumps;
                databaseUser.Statistics.TotalScore += player.Statistics.TotalScore;
                databaseUser.Statistics.GamesPlayed += player.Statistics.GamesPlayed;
                databaseUser.Statistics.GameTime += player.Statistics.GameTime;
                databaseUser.Statistics.Victories += player.Statistics.Victories;
                await _context.SaveChangesAsync();
            }
        }
    }
}
