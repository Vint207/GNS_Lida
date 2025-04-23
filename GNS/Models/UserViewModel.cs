using Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GNS.Models
{
    public class UserViewModel : INotifyPropertyChanged
    {
		private User _user;
		public string Name
		{
			get => _user.Name;
			set
			{
				_user.Name = value;
			}
		}
		public UserType Type
		{
			get => _user.Type;
			set
			{
				_user.Type = value;
			}
		}

		public UserViewModel(User user)	
		{
			user.PropertyChanged += User_PropertyChanged;
			_user = user;
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		private void User_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(User.Name))
			{
				OnPropertyChanged(nameof(Name));
			}

			if (e.PropertyName == nameof(User.Type))
			{
				OnPropertyChanged(nameof(Type));
			}
		}

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
