using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Core.Model
{
    public class User
    {
        public int UserId { get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(25)]
        public string Password { get; set; }
        [MaxLength(50)]
        public string ApiKey { get; set; }
    }
}
