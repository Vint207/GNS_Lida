
using ErrorOr;
using System.Text.Json.Serialization;

namespace Services.Intrefaces
{
	public interface ILoginService
	{
		Task<ErrorOr<Success>> Login(LoginDataVm loginData);
		Task<ErrorOr<Success>> Logout();
		Task<ErrorOr<Success>> RefreshToken(string refreshToken);
		Task<ErrorOr<string>> GetAccessToken(string baseAddress);

		public record struct LoginDataVm(string Username, string Password)
		{
			[JsonPropertyName("username")]
			public string Username { get; set; } = Username;

			[JsonPropertyName("password")]
			public string Password { get; set; } = Password;
		}

	public record struct JwtToken(string Access, string Refresh)
		{
			[JsonPropertyName("refresh")]
			public string Refresh { get; set; } = Refresh;

			[JsonPropertyName("access")]
			public string Access { get; set; } = Access;
		}

		public record struct JwtAccessToken
		{
			[JsonPropertyName("access")]
			public string Access { get; set; }
		}

		public record struct JwtRefreshToken(string Token)
		{
			[JsonPropertyName("refresh")]
			public string Refresh { get; set; } = Token;
		}
	}
}
