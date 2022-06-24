﻿using JumpenoWebassembly.Server.Data;
using JumpenoWebassembly.Shared.Constants;
using JumpenoWebassembly.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
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
                return new User { Id = id, Username = name };

            } else if (authMethod == AuthenticationMethod.Spectator) {
                return new User { Username = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name), Id = 0 };

            } else {
                var mail = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
                return await _context.Users.FirstOrDefaultAsync(user => user.Email == mail);
            }
        }

        public async Task AddGame()
        {
            var authMethod = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.AuthenticationMethod);
            if (authMethod != AuthenticationMethod.Spectator && authMethod != AuthenticationMethod.Anonym)
            {
                var mail = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
                var user = await _context.Users.FirstOrDefaultAsync(user => user.Email == mail);
                user.Statistics.GamesPlayed += 1;
                user.Statistics.StartGameTime = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}
