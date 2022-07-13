using JumpenoWebassembly.Server.Data;
using JumpenoWebassembly.Server.Services;
using JumpenoWebassembly.Shared;
using JumpenoWebassembly.Shared.Jumpeno;
using JumpenoWebassembly.Shared.Jumpeno.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JumpenoWebassembly.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ErrorController : ControllerBase
    {
        private readonly DataContext _context;

        public ErrorController(DataContext context)
        {
            _context = context;
        }

        [HttpPut("addError")]
        public async Task AddError(Error error)
        {
            _context.Errors.Add(error);
            await _context.SaveChangesAsync();
        }

        [HttpGet("getAllErrors")]
        public IActionResult GetAllError()
        {
            var errors = _context.Errors.ToList();
            return Ok(errors);
        }
    }
}
