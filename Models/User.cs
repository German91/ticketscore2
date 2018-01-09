using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tickets.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Address { get; set; }
        public int Phone { get; set; }
        public ICollection<Ticket> Tickets { get; set; }

        public User()
        {
            Tickets = new Collection<Ticket>();
        }
    }
}