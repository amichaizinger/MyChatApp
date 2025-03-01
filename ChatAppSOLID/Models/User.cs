using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppSOLID.Models
{
    public class User
    {


        public Guid Id { get; } = Guid.NewGuid(); //TODO: maybe enter here the server info
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
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Password cannot be empty");

                if (value.Length < 8)
                    throw new ArgumentException("Password must be at least 8 characters long");

                _password = value;
            }
        }
        private bool ?_isOnline;
        public bool ?IsOnline { get => _isOnline; set => _isOnline = value; }
        public List<Group> Groups { get; } = new List<Group>();
        public List<Message> Messages { get; set; } = new List<Message>();

        public User(Guid id, string username)
        {
            Id = id;
            UserName = username;
        }

        public User()
        {
            
        }




    }
}
