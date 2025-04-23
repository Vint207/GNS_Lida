using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Services
{
	public enum UserType
	{ Guest, Operator, Admin }

	public class User : INotifyPropertyChanged
	{
		public Guid Id { get; set; } 

		private string _name;
		public string Name
		{
			get => _name;
			set
			{
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		} 

		private UserType _type;
		public UserType Type
		{
			get => _type;
			set
			{
				_type = value;
				OnPropertyChanged(nameof(Type));
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		public static async Task<User?> GetUser()
		{
			var userString = await SecureStorage.GetAsync("user");

			if (!string.IsNullOrWhiteSpace(userString))
			{
				return JsonSerializer.Deserialize<User>(userString);
			}

			return null;
		}

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
