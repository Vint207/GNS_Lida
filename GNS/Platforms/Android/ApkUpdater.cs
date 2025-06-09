using static Services.HttpService;
using ErrorOr;
using Android.Content;
using Android.OS;
using AndroidX.Core.Content;
using FileProvider = AndroidX.Core.Content.FileProvider;
using System.Text.Json.Serialization;
using AndroidX.Activity.Result;
using Java.Interop;
using static Android.Preferences.PreferenceManager;
using System.Threading.Tasks;
using SkiaSharp;
using CommunityToolkit.Maui.Alerts;

namespace GNS.Platforms.Android;

public class ApkUpdater
{

	#region Check_Version
	private static async Task<ErrorOr<string>> GetLatestVersion()
	{
		var getApkVersionResult = await ExecuteHttpRequestAsync<Empty, GetLatestVersionResponse>(Method.Get, "api/app/version");

		if (getApkVersionResult.IsError)
			return getApkVersionResult.FirstError;

		return getApkVersionResult.Value.VersionName;
	}

	public record struct GetLatestVersionResponse(
		[property: JsonPropertyName("version_name")] string VersionName,
		[property: JsonPropertyName("apk_url")] string ApkUrl,
		[property: JsonPropertyName("update_date")] DateTime UpdateDate);


	public static async Task<ErrorOr<bool>> IsNewVersionAvailable()
	{
		try
		{
			var getLatestVersionResult = await GetLatestVersion();

			if (getLatestVersionResult.IsError)
				return getLatestVersionResult.FirstError;

			string latestVersionString = getLatestVersionResult.Value;
			string currentVersionString = await SecureStorage.GetAsync("currentVersion") ?? "1.0.0";

			var latestVersion = new Version(latestVersionString);
			var currentVersion = new Version(currentVersionString);

			if (latestVersion > currentVersion)
			{
				await SecureStorage.SetAsync("currentVersion", latestVersionString);
				return true;
			}

			return false;
		}
		catch (Exception ex)
		{
			return Error.Failure(description: ex.Message);
		}
	}
	#endregion


	#region Download_And_Install_Apk
	public static async Task<ErrorOr<Success>> DownloadAndInstallApk()
	{
		try
		{
			// Загрузка APK
			var getApkResult = await ExecuteHttpRequestAsync<Empty, GetApkResponse>(Method.Get, "api/app/apk");

			if (getApkResult.IsError)
				return getApkResult.FirstError;

			// Путь для сохранения APK
			_apkPath = Path.Combine(global::Android.App.Application.Context.FilesDir.AbsolutePath, getApkResult.Value.FileName);
			var apkBytes = Convert.FromBase64String(getApkResult.Value.ApkBytes);
			await File.WriteAllBytesAsync(_apkPath, apkBytes);

			// Установка APK на Android
			return await InstallApk();
		}
		catch (Exception ex)
		{
			return Error.Failure(description: ex.Message);
		}
	}

	public record struct GetApkResponse(string ApkBytes, string FileName, string Version);

	private static string _apkPath;

	private static async Task<ErrorOr<Success>> InstallApk()
	{
		try
		{
			var context = global::Android.App.Application.Context;
			if (context is null)
				return Error.Failure(description: "Контекст приложения не получен");

			var apkUri = FileProvider.GetUriForFile(
				context, 
				$"{context.PackageName}.fileprovider", 
				new Java.IO.File(_apkPath));

			var installApkIntent = new Intent(Intent.ActionInstallPackage);
			installApkIntent.SetData(apkUri);
			installApkIntent.SetFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.NewTask);
			installApkIntent.AddFlags(ActivityFlags.NewTask);

			// Проверяем разрешение на установку
			if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
			{
				if (!context.PackageManager.CanRequestPackageInstalls())
				{
					if (Platform.CurrentActivity is not MainActivity mainActivity)
						return Error.Failure(description: "Текущая активность не найдена");

					// Запускаем активность настроек с ожиданием результата
					var requestPermissionIntent = new Intent(global::Android.Provider.Settings.ActionManageUnknownAppSources);
					requestPermissionIntent.SetData(global::Android.Net.Uri.Parse($"package:{context.PackageName}"));
					requestPermissionIntent.AddFlags(ActivityFlags.NewTask);
					mainActivity.StartActivity(requestPermissionIntent);
					MainActivity.OnResumeHandler += InstallApkIntent;
					return Result.Success;
				}
			}

			// Разрешение получено, начинаем установку
			context.StartActivity(installApkIntent);
			return Result.Success;
		}
		catch (Exception ex)
		{
			return Error.Failure(description: ex.Message);
		}
	}

	private static async void InstallApkIntent()
	{
		try
		{
			var context = global::Android.App.Application.Context;
			if (context is null) return;

			var apkUri = FileProvider.GetUriForFile(
				context,
				$"{context.PackageName}.fileprovider",
				new Java.IO.File(_apkPath));

			var installApkIntent = new Intent(Intent.ActionInstallPackage);
			installApkIntent.SetData(apkUri);
			installApkIntent.SetFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.NewTask);
			installApkIntent.AddFlags(ActivityFlags.NewTask);

			if (context.PackageManager?.CanRequestPackageInstalls() ?? false)
			{
				// Разрешение получено, начинаем установку
				context.StartActivity(installApkIntent);
			}
			else await Toast.Make("Разрешение на установку не получено", CommunityToolkit.Maui.Core.ToastDuration.Short, 16).Show();

			MainActivity.OnResumeHandler -= InstallApkIntent;
		}
		catch (Exception ex)
		{
			await Toast.Make(ex.Message, CommunityToolkit.Maui.Core.ToastDuration.Short, 16).Show();
			return;
		}
	}
	#endregion
}

