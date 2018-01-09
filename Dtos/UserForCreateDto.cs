using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tickets.API.Dtos
{
    public class UserForCreateDto
    {
        [Required, StringLength(100)]
        public string Username { get; set; }
        [Required, StringLength(10, MinimumLength = 6, ErrorMessage = "You must specify a password greater than 6 and lower than 10 characters")]
        public string Password { get; set; }
        [Required, RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email is not valid")]
        public string Email { get; set; }
        public string Address { get; set; }
    }
}