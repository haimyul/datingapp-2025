using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class MembersController(AppDbContext context) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMembers()
    {
        var members = await context.Users.ToListAsync();

        return members;
    }

    [Authorize]
    [Authorize]
    [HttpGet("{memberId}")]
    public async Task<ActionResult<AppUser>> GetMember(string memberId)
    {
        var member = await context.Users.FindAsync(memberId);

        return member != null
            ? member
            : NotFound();
    }
}
