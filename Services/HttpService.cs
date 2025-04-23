using CommunityToolkit.Maui.Alerts;
using ErrorOr;
using Services.Intrefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Services;

public class HttpService
{

	public async static Task<ErrorOr<TResponseContent>> ExecuteHttpRequestAsync<TRequestContent, TResponseContent>(
		Method method,
		string apiPath,
		TRequestContent requestContent = default,
		string domen = default,
		int timeout = 10)
	{
		if (string.IsNullOrWhiteSpace(domen))
		{
			domen = await SecureStorage.GetAsync("base_address") ?? string.Empty;

			if (string.IsNullOrWhiteSpace(domen))
				return Error.Failure(description: $"Некорректный адрес сервера");
		}
		
		string path = $"{domen}/{apiPath}";

		try
		{
			using var httpClient = new HttpClient(new HttpClientHandler
			{ ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true });

			httpClient.Timeout = TimeSpan.FromSeconds(timeout);
			string? accessToken = await SecureStorage.GetAsync("access_token");
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

			var requestContentString = new StringContent("");

			if (requestContent is not null && typeof(TRequestContent) != typeof(Empty))
			{
				var serializedRequestContent = JsonSerializer.Serialize(requestContent, _serializerOptions);
				requestContentString = new StringContent(serializedRequestContent, Encoding.UTF8, "application/json");
			}

			var httpResponse = method switch
			{
				Method.Get => await httpClient.GetAsync(path),
				Method.Post => await httpClient.PostAsync(path, requestContentString),
				Method.Patch => await httpClient.PatchAsync(path, requestContentString),
				Method.Delete => await httpClient.DeleteAsync(path),
				_ => null
			};

			if (httpResponse is null)
				return Error.Failure($"Сервер вернул ошибку:\n\"Нет ответа\"");


			if (httpResponse.StatusCode == HttpStatusCode.OK || 
				httpResponse.StatusCode == HttpStatusCode.Created)
			{
				if (httpResponse.Content is null || typeof(TResponseContent) == typeof(Empty) || typeof(TResponseContent) == typeof(Success))
					return default;

				return JsonSerializer.Deserialize<TResponseContent>(await httpResponse.Content.ReadAsStringAsync());
			}

			if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
			{
				OnUnauthorizedHandler?.Invoke(null, EventArgs.Empty);
				return Error.Failure(description: $"Сервер вернул ошибку:\n\"Пользователь не авторизован\"");
			}
			else
			{
				var getProblemDetailsResult = await GetProblemDetails(httpResponse);

				if (getProblemDetailsResult.IsError)
					return Error.Failure(description: $"Сервер вернул ошибку:\n\"{getProblemDetailsResult.FirstError.Description}\"");

				if (string.IsNullOrWhiteSpace(getProblemDetailsResult.Value) || getProblemDetailsResult.Value.Equals("\n"))
					return Error.Failure(description: $"Сервер вернул ошибку:\nНекорректный запрос");

				return Error.Failure(description: $"Сервер вернул ошибку:\n\"{getProblemDetailsResult.Value}\"");
			}
		}
		catch (Exception ex)
		{
			return Error.Failure(description: $"Сервер вернул ошибку:\n\"{ex.Message}\"");
		}
	}

	public record struct Empty;

	public enum Method { Get, Post, Patch, Put, Delete }

	private static readonly JsonSerializerOptions _serializerOptions = new()
	{
		PropertyNamingPolicy = null
	};

	public static event EventHandler OnUnauthorizedHandler;


	private async static Task<ErrorOr<string>> GetProblemDetails(HttpResponseMessage response)
	{
		try
		{
			var responseString = await response.Content.ReadAsStringAsync();
			var problem = JsonSerializer.Deserialize<ProblemDetails>(responseString);

			return $"{problem.Detail}";
		}
		catch (Exception ex)
		{
			return Error.Failure(description: ex.Message);
		}
	}

	public class ProblemDetails
	{
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyOrder(-6)]
		[JsonPropertyName("type")]
		public string? Type { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyOrder(-5)]
		[JsonPropertyName("title")]
		public string? Title { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyOrder(-4)]
		[JsonPropertyName("status")]
		public int? Status { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyOrder(-3)]
		[JsonPropertyName("detail")]
		public string? Detail { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyOrder(-1)]
		[JsonPropertyName("traceId")]
		public string? TraceId { get; set; }
	}
}

