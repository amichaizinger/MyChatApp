using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Server.Models
{
    public class User
    {


        public string Id { get; set; }
        private string _username;
        public string UserName
        {
            get => _username;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Username cannot be empty");

                if (value.Length < 8)
                    throw new ArgumentException("Username must be at least 8 characters long");
                               
                _username = value;
            }
        }

        private string _password;

        public string ?Password
        {
            get => _password;
            set
            {
                _password = value;
            }
        }
        public List<Group> Groups { get; }
        public List<Message> Messages { get; set; }



        public User()
        {

        }


    }
}
