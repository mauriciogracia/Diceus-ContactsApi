using ContactsApi.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RequestModels;

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

        private bool IsPasswordValid(string enteredPassword, string savedPassword)
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
    }

}
