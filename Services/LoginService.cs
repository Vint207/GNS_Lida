using CommunityToolkit.Maui.Alerts;
using ErrorOr;
using Services.Intrefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Services.HttpService;
using static Services.Intrefaces.ILoginService;

namespace Services
{
    public class LoginService : ILoginService
	{
        public static string BaseAddress { get; set; }
        private static Timer _timer;
        private User _user;

		public  LoginService(User user)
        {
			_user = user;
			StartTokenRefreshing(TimeSpan.FromMinutes(15));
		}

		public async Task<ErrorOr<Success>> Login(LoginDataVm loginData)
		{
			var response = await ExecuteHttpRequestAsync<LoginDataVm, JwtToken>(
				Method.Post, "api/token/", new LoginDataVm(loginData.Username, loginData.Password));

			if (response.IsError)
				return response.FirstError;

			try
			{
				await SecureStorage.SetAsync("access_token", response.Value.Access);
				await SecureStorage.SetAsync("refresh_token", response.Value.Refresh);
			}
			catch (Exception ex)
			{
				return Errors.ExceptionOccured(
					$"При попытке войти в аккаунт", ex.Message);
			}

			return Result.Success;		
		}

		public async Task<ErrorOr<Success>> Logout()
		{
			try
			{
				SecureStorage.Remove("access_token");
				SecureStorage.Remove("refresh_token");
				_user.Name = "Гость";
				_user.Type = UserType.Guest;

				return Result.Success;
			}
			catch (Exception ex)
			{
				return Errors.ExceptionOccured(
					$"При попытке выйти из аккаунта", ex.Message);
			}
		}

		public async Task<ErrorOr<Success>> RefreshToken(string refreshToken)
		{
			try
			{
				var response = await ExecuteHttpRequestAsync<JwtRefreshToken, JwtAccessToken>(
						Method.Post, "api/token/refresh/", new JwtRefreshToken(refreshToken));

				if (response.IsError)
					return response.FirstError;

				await SecureStorage.SetAsync("access_token", response.Value.Access);

				return Result.Success;
			}
			catch (Exception ex)
			{
				return Errors.ExceptionOccured(
					$"При попытке обновить токен доступа", ex.Message);
			}
		}

		public async Task<ErrorOr<string>> GetAccessToken(string baseAddress)
		{
			try
			{
				return await SecureStorage.GetAsync("access_token");
			}
			catch (Exception ex)
			{
				return Errors.ExceptionOccured(
					$"При попытке получить токен доступа", ex.Message);
			}
		}

		private ErrorOr<Success> StartTokenRefreshing(TimeSpan period)
        {
            _timer = new Timer(async state =>
            {
                await RefreshToken(await SecureStorage.GetAsync("refresh_token"));
            },
            null,
            TimeSpan.Zero,
            period);

			return Result.Success;
        }

		public static class Errors
		{
			public static Error ExceptionOccured(string action, string message) => Error.Failure(
				code: "ExceptionOccured",
				description: $"{action} возникло исключение \"{message}\"");
		}
	}
}
