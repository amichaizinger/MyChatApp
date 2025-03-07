﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppSOLID.Models
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
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Password cannot be empty");

                if (value.Length < 8)
                    throw new ArgumentException("Password must be at least 8 characters long");

                _password = value;
            }
        }
           public List<Group> Groups { get; } 
        public List<Message> Messages { get; set; } 

        public User(string id, string username)
        {
            Messages = new List<Message>();
            Groups = new List<Group>();
            Id = id;
            UserName = username;
        }

        public User()
        {
            
        }




    }
}
