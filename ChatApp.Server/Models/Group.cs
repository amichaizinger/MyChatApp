using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Server.Models
{
    public class Group
    {
        public string Id { get; set;  } 
        public string Name { get; set; }
        public List<User> Members { get; set; }
        public List<Message> Messages { get; set; }
    }
}
