using ContactsApi.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RequestModels;
using ContactsApi.Models;

namespace ContactsApi.Controllers
{
    // UsersController.cs (Controller class)
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ContactsDbContext _dbContext;

        public UsersController(ContactsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // POST: api/Users
        [HttpPost]
        public async Task<bool> CreateUser(UserRequest userRequest)
        {
            var userExists = await UsernameExists(userRequest.Username);
            int result = 0;

            if (!userExists) { 
                User user = new User(userRequest);
                _dbContext.Users.Add(user);
                result = await _dbContext.SaveChangesAsync();
            }
            return (result == 1);
        }

        // POST: api/Users/validate
        [HttpPost("validate")]
        public async Task<bool> ValidateUser(UserRequest user)
        {
            var dbUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
            var isValid = (dbUser != null && IsPasswordValid(user.Password, dbUser.Password));

            return isValid;
        }

        private static bool IsPasswordValid(string enteredPassword, string savedPassword)
        {
            // In a real application, use a secure hashing and password verification method.
            // For demonstration purposes only, we are doing a basic string comparison here.
            return enteredPassword == savedPassword;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<User> GetUser(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);

            return user;
        }

        private async Task<bool> UsernameExists(string username)
        {
            var dbUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

            return (dbUser != null);
        }

        // POST: api/Users/{userId}/StartSession
        [HttpPost("{userId}/StartSession")]
        public IActionResult StartSession(string username)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                // User with the provided username not found
                return NotFound();
            }

            // Generate a new session token (using Guid as an example)
            var sessionToken = Guid.NewGuid().ToString();

            // Create a new session record in the database
            var session = new Session
            {
                Token = sessionToken,
                UserId = user.Id
            };
            _dbContext.Sessions.Add(session);
            _dbContext.SaveChanges();

            return Ok(sessionToken);
        }

        // GET: api/Users/GetUserIdBySession
        [HttpGet("GetUserIdBySession")]
        public ActionResult<int> GetUserIdBySession(string token)
        {
            // Find the session with the given token
            var session = _dbContext.Sessions.FirstOrDefault(s => s.Token == token);

            // Return the associated userId if session is found, otherwise return -1
            return session?.UserId ?? -1;
        }

        // DELETE: api/Users/EndSession
        [HttpDelete("EndSession")]
        public IActionResult EndSession(string token)
        {
            // Find the session with the given token
            var session = _dbContext.Sessions.FirstOrDefault(s => s.Token == token);

            if (session != null)
            {
                // Remove the session from the database
                _dbContext.Sessions.Remove(session);
                _dbContext.SaveChanges();

                return NoContent(); // 204 No Content
            }
            else
            {
                return NotFound("Session not found");
            }
        }
    }
}
