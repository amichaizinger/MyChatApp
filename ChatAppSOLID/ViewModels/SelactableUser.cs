using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ChatAppSOLID.Models;

namespace ChatAppSOLID.ViewModels
{
        public class SelectableUser : PropertyNotifier
        {
            private readonly User _user;
            private bool _isSelected;

            public User User => _user;
            public bool IsSelected
            {
                get => _isSelected;
                set
                {
                    if (_isSelected != value)
                    {
                        _isSelected = value;
                        OnPropertyChanged();
                    }
                }
            }

        public string Username => User?.UserName;

        public SelectableUser(User user)
            {
                _user = user;
            }

           
        }
    }

