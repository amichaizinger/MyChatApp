using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppSOLID.Models
{
    public class Group
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; set; }
        public List<User> Members { get; set; } = new List<User>();
        public List<Message> Messages { get; set; } = new List<Message>();
    }
}
