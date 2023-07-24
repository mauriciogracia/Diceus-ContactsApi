using ContactsApi.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RequestModels;

namespace ContactsApi.Controllers
{
    // ContactsController.cs (Controller class)
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ContactsDbContext _dbContext;

        public ContactsController(ContactsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/Contacts/<userId>
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContactsByUser(int userId)
        {
            return await _dbContext.Contacts.Where(c => c.UserId == userId).ToListAsync();
        }

        // POST: api/Contacts
        [HttpPost]
        public async Task<ActionResult<Contact>> CreateContact(ContactRequest contactRequest)
        {
            var contact = new Contact(contactRequest) ;
            _dbContext.Contacts.Add(contact);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction("CreateContact", new { id = contact.Id }, contact);
        }

        // PUT: api/Contacts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(Contact contact)
        {
            _dbContext.Entry(contact).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(contact.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Contacts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var contact = await _dbContext.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            _dbContext.Contacts.Remove(contact);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool ContactExists(int id)
        {
            return _dbContext.Contacts.Any(c => c.Id == id);
        }
    }
}
