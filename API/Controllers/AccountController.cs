using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(
    AppDbContext context, ITokenService tokenService
) : BaseApiController
{
    [HttpPost("register")]
    /*
    public async Task<ActionResult<AppUser>>Register(
        string displayName, string email, string password
     */
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await EmailExists(registerDto.Email))
        {
            return BadRequest("This email is already used by another user");
        }

        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            // DisplayName = displayName,
            // Email = email,
            // PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
            DisplayName = registerDto.DisplayName,
            Email = registerDto.Email,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(
                registerDto.Password
            )),
            PasswordSalt = hmac.Key
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user.ToDto(tokenService);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var result = await GetUserFromDb(loginDto);
        AppUser? user = result.user;
        // var user = await context.Users.SingleOrDefaultAsync(
        //     x => x.Email.ToLower() == loginDto.Email.ToLower()
        // );
        if (user == null)
        {
            var exception = result.exception == "duplicate"
                ? "More than one matching email address found"
                : "Invalid email or password";
            return Unauthorized(exception);
            // return Unauthorized("Invalid email or password");
        }

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(
            Encoding.UTF8.GetBytes(loginDto.Password)
        );
        if (!computedHash.SequenceEqual(user.PasswordHash))
        {
            return Unauthorized("Invalid email or password");
        }
        // for (int i = 0; i < computedHash.Length; i++)
        // {
        //     if (computedHash[i] != user.PasswordHash[i])
        //     {
        //         return Unauthorized("Invalid email or password");
        //     }
        // }

        return user.ToDto(tokenService);
    }

    private async Task<bool> EmailExists(string email)
    {
        return await context.Users.AnyAsync(
            x => x.Email.ToLower() == email.ToLower()
        );
    }

    private async Task<(AppUser? user, string exception)>
        GetUserFromDb(LoginDto loginDto)
    {
        try
        {
            var user = await context.Users.SingleOrDefaultAsync( //
                x => x.Email.ToLower() == loginDto.Email.ToLower()
            );
            return (user, "");
        }
        catch (InvalidOperationException)
        {
            return (null, "duplicate");
        }
        catch
        {
            return (null, "");
        }
    }
}