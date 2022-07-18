using JumpenoWebassembly.Server.Data;
using JumpenoWebassembly.Server.Services;
using JumpenoWebassembly.Shared.ErrorHandling;
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

        [HttpPut("submitError")]
        public async Task SubmitError(Error error)
        {
            _context.Errors.Add(error);
            await _context.SaveChangesAsync();
        }

        [HttpGet("receiveErrorLog")]
        public IActionResult ReceiveErrorLog()
        {
            var errors = _context.Errors.ToList();
            return Ok(errors);
        }
    }
}
