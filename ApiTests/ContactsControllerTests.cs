using ContactsApi.Controllers;
using ContactsApi.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RequestModels;

namespace ApiTests
{
    public class ContactsControllerTests
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
        public async Task CreateContact_ValidContact_ReturnsCreatedResult()
        {
            // Arrange
            var dbContext = SetupDbContext();
            var controller = new ContactsController(dbContext);
            var userRequest = new UserRequest { Username = "testuser", Password = "testpassword" };
            var user = new User(userRequest);
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var newContact = new Contact { UserId = user.Id, Name = "John Doe", Phone = "123456789", Email = "john@example.com" };

            // Act
            var result = await controller.CreateContact(newContact);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);

            // Ensure the contact was added to the database
            var contactFromDb = await dbContext.Contacts.FirstOrDefaultAsync(c => c.Name == "John Doe");
            Assert.NotNull(contactFromDb);
            Assert.Equal("John Doe", contactFromDb.Name);
            Assert.Equal("123456789", contactFromDb.Phone);
            Assert.Equal("john@example.com", contactFromDb.Email);
        }


        [Fact]
        public async Task UpdateContact_ExistingContact_ReturnsNoContent()
        {
            // Arrange
            var dbContext = SetupDbContext();
            var controller = new ContactsController(dbContext);
            var userRequest = new UserRequest { Username = "testuser", Password = "testpassword" };
            var user = new User(userRequest);
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var existingContact = new Contact { UserId = user.Id, Name = "Initial Name", Phone = "123456789", Email = "initial@example.com" };
            dbContext.Contacts.Add(existingContact);
            await dbContext.SaveChangesAsync();

            // Detach the existing contact entity from the DbContext
            dbContext.Entry(existingContact).State = EntityState.Detached;

            var updatedContact = new Contact { Id = existingContact.Id, UserId = user.Id, Name = "Updated Name", Phone = "987654321", Email = "updated@example.com" };

            // Act
            var result = await controller.UpdateContact(updatedContact);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Ensure the contact was updated in the database
            var contactFromDb = await dbContext.Contacts.FirstOrDefaultAsync(c => c.Id == existingContact.Id);
            Assert.NotNull(contactFromDb);
            Assert.Equal("Updated Name", contactFromDb.Name);
            Assert.Equal("987654321", contactFromDb.Phone);
            Assert.Equal("updated@example.com", contactFromDb.Email);
        }

        [Fact]
        public async Task DeleteContact_ExistingContact_ReturnsNoContent()
        {
            // Arrange
            var dbContext = SetupDbContext();
            var controller = new ContactsController(dbContext);
            var userRequest = new UserRequest { Username = "testuser", Password = "testpassword" };
            var user = new User(userRequest);
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var existingContact = new Contact { UserId = user.Id, Name = "Test Contact", Phone = "55555555", Email = "test@example.com" };
            dbContext.Contacts.Add(existingContact);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await controller.DeleteContact(existingContact.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var contactFromDb = await dbContext.Contacts.FirstOrDefaultAsync(c => c.Id == existingContact.Id);
            Assert.Null(contactFromDb); // The contact should be deleted from the database
        }

        // Add other test methods for different scenarios as needed.
    }
}

