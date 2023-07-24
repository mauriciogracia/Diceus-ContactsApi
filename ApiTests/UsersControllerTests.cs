// UsersControllersTests.cs (Unit Test class)
using ContactsApi.Controllers;
using ContactsApi.DAL;
using Microsoft.EntityFrameworkCore;
using RequestModels;

namespace ApiTests
{
    public class UsersControllerTests
    {
        // Helper method to set up the DbContext with In-Memory Database
        private ContactsDbContext SetupDbContext()
        {
            var options = new DbContextOptionsBuilder<ContactsDbContext>()
                .UseInMemoryDatabase(databaseName: "TestContactsDb")
                .Options;

            return new ContactsDbContext(options);
        }

        [Fact]
        public async Task CreateUser_ValidUser_ReturnsCreatedResult()
        {
            // Arrange
            var dbContext = SetupDbContext();
            var controller = new UsersController(dbContext);
            var userRequest = new UserRequest { Username = "testuser", Password = "testpassword" };
            var user = new User(userRequest);

            // Act
            var result = await controller.CreateUser(user);

            // Ensure the user was added to the database
            var userFromDb = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
            Assert.NotNull(userFromDb);
            Assert.Equal("testuser", userFromDb.Username);
            Assert.Equal("testpassword", userFromDb.Password);
        }

        [Fact]
        public async Task ValidateUser_ExistingUserAndPassword_ReturnsTrue()
        {
            // Arrange
            var dbContext = SetupDbContext();
            var controller = new UsersController(dbContext);
            var userRequest = new UserRequest { Username = "testuser", Password = "testpassword" };
            var user = new User(userRequest);
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await controller.ValidateUser(user);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ValidateUser_NonExistentUser_ReturnsFalse()
        {
            // Arrange
            var dbContext = SetupDbContext();
            var controller = new UsersController(dbContext);
            var existingRequest = new UserRequest { Username = "testuser", Password = "testpassword" };
            var existingUser = new User(existingRequest);
            dbContext.Users.Add(existingUser);
            await dbContext.SaveChangesAsync();

            
            // Act
            var e = new User(new UserRequest { Username = "testuser", Password = "blablebli" });
            var result = await controller.ValidateUser(e);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateUser_InvalidPassword_ReturnsFalse()
        {
            // Arrange
            var dbContext = SetupDbContext();
            var controller = new UsersController(dbContext);
            var existingRequest = new UserRequest { Username = "testuser", Password = "testpassword" };
            var existingUser = new User(existingRequest);
            dbContext.Users.Add(existingUser);
            await dbContext.SaveChangesAsync();

            // Act
            existingUser.Password = "blabla";
            var result = await controller.ValidateUser(existingUser);

            // Assert
            Assert.False(result);
        }

        // Add other test methods for different scenarios as needed.
    }
}
