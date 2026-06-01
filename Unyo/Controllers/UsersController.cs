using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unyo.Models;

namespace Unyo.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    public UsersController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    // GET: api/Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        return Ok(users);
    }

    // GET: api/Users/guid-utilizator
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    // PUT: api/Users/guid-utilizator
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(string id, [FromBody] User incomingUser)
    {
        if (id != incomingUser.Id)
        {
            return BadRequest("ID mismatch");
        }

        var userFromDb = await _userManager.FindByIdAsync(id);

        if (userFromDb == null)
        {
            return NotFound();
        }

        userFromDb.FirstName = incomingUser.FirstName;
        userFromDb.LastName = incomingUser.LastName;
        userFromDb.BirthDate = incomingUser.BirthDate;

        // Identity fields
        userFromDb.Email = incomingUser.Email;
        userFromDb.UserName = incomingUser.UserName;

        var result = await _userManager.UpdateAsync(userFromDb);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }

    // POST: api/Users
    [HttpPost]
    public async Task<ActionResult<User>> PostUser([FromBody] User incomingUser)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new User
        {
            UserName = incomingUser.UserName ?? incomingUser.Email,
            Email = incomingUser.Email,
            FirstName = incomingUser.FirstName,
            LastName = incomingUser.LastName,
            BirthDate = incomingUser.BirthDate,
            EmailConfirmed = true
        };

        // In case an admin creates a user without specifying a password, we can set a default one and force them to change it later.
        var result = await _userManager.CreateAsync(user, "User@Default123!");

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        await _userManager.AddToRoleAsync(user, "User");

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    // DELETE: api/Users/guid-utilizator
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }
}