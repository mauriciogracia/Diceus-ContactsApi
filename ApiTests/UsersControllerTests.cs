// UsersControllersTests.cs (Unit Test class)
using ContactsApi.Controllers;
using ContactsApi.DAL;
using ContactsApi.Models;
using Microsoft.AspNetCore.Mvc;
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

        // Test for GetUserIdBySession when session exists
        [Fact]
        public void GetUserIdBySession_SessionExists_ReturnsUserId()
        {
            // Arrange
            var dbContext = SetupDbContext();
            var users = new List<User>
            {
                new User { Id = 1, Username = "user1", Password = "password1" },
                new User { Id = 2, Username = "user2", Password = "password2" },
            };
            dbContext.Users.AddRange(users);

            var sessions = new List<Session>
            {
                new Session { Token = "session-token-1", UserId = 1 },
                new Session { Token = "session-token-2", UserId = 2 },
            };
            dbContext.Sessions.AddRange(sessions);
            dbContext.SaveChanges();

            var usersController = new UsersController(dbContext);

            // Act
            var result = usersController.GetUserIdBySession("session-token-1");

            // Assert
            Assert.Equal(1, result.Value);
        }

        // Test for GetUserIdBySession when session does not exist
        [Fact]
        public void GetUserIdBySession_SessionDoesNotExist_ReturnsNull()
        {
            // Arrange
            var dbContext = SetupDbContext();
            var users = new List<User>
            {
                new User { Id = 3, Username = "user1", Password = "password1" },
                new User { Id = 4, Username = "user2", Password = "password2" },
            };
            dbContext.Users.AddRange(users);
            dbContext.SaveChanges();

            var usersController = new UsersController(dbContext);

            // Act
            var result = usersController.GetUserIdBySession("non-existing-session-token");

            // Assert
            Assert.Equal(result.Value,-1);
        }

        // Test for StartSession
        [Fact]
        public void StartSession_ValidUserId_ReturnsNewSessionToken()
        {
            // Arrange
            var dbContext = SetupDbContext();
            var user = new User { Id = 5, Username = "user1", Password = "password1" };
            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            var usersController = new UsersController(dbContext);

            // Act
            var result = usersController.StartSession("user1");

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var sessionToken = Assert.IsType<string>(actionResult.Value);
            Assert.NotNull(sessionToken);
        }

        // Test for EndSession when session exists
        [Fact]
        public void EndSession_SessionExists_ReturnsNoContentResult()
        {
            // Arrange
            var dbContext = SetupDbContext();
            var sessionToken = "session-token-1";
            var session = new Session { Token = sessionToken, UserId = 1 };
            dbContext.Sessions.Add(session);
            dbContext.SaveChanges();

            var usersController = new UsersController(dbContext);

            // Act
            var result = usersController.EndSession(sessionToken);

            // Assert
            var actionResult = Assert.IsType<NoContentResult>(result);
        }

        // Test for EndSession when session does not exist
        [Fact]
        public void EndSession_SessionDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            var dbContext = SetupDbContext();
            var usersController = new UsersController(dbContext);

            // Act
            var result = usersController.EndSession("non-existing-session-token");

            // Assert
            var actionResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Session not found", actionResult.Value);
        }
    }
}
