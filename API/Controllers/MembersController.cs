using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMembers()
        {
            var members = await context.Users.ToListAsync();

            return members;
        }

        [HttpGet("{memberId}")]
        public async Task<ActionResult<AppUser>> GetMember(string memberId)
        {
            var member = await context.Users.FindAsync(memberId);

            return member != null
                ? member
                : NotFound();
        }
    }
}
