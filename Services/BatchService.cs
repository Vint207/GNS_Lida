using ErrorOr;
using Services.Intrefaces;
using static Services.HttpService;
using static Services.Intrefaces.IBatchService;
﻿using CommunityToolkit.Maui.Alerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Net;
using System.Text.Json;
using System.Net.Http.Json;
using static Services.HttpService;

/* Unmerged change from project 'GNS (net8.0-maccatalyst)'
Before:
using static GNS.Services.BatchService.BatchListModel;
After:
using static GNS.Services.Main.BatchService.BatchListModel;
using GNS;
using GNS.Services;
using GNS.Services.Main;
*/
using Services.Intrefaces;
using System.IO;
using static Services.Intrefaces.IBatchService;
using ErrorOr;

namespace Services;

public class BatchService : IBatchService
{

	public async Task<ErrorOr<IEnumerable<CarVM>>> GetStationCars()
	{
		return await ExecuteHttpRequestAsync<Empty, IEnumerable<CarVM>>(Method.Get, $"api/trucks?on_station=True");
	}

	public async Task<ErrorOr<IEnumerable<TrailerVM>>> GetStationTrailers()
	{
		return await ExecuteHttpRequestAsync<Empty, IEnumerable<TrailerVM>>(Method.Get, $"api/trailers?on_station=True");
	}

	public async Task<ErrorOr<IEnumerable<int>>> GetStationLoadingReaders()
	{
		return await ExecuteHttpRequestAsync<Empty, IEnumerable<int>>(Method.Get, $"api/loading-balloon-reader-list");
	}

	public async Task<ErrorOr<IEnumerable<int>>> GetStationUnloadingReaders()
	{
		return await ExecuteHttpRequestAsync<Empty, IEnumerable<int>>(Method.Get, $"api/unloading-balloon-reader-list");
	}

	public async Task<ErrorOr<BatchVM>> StartLoading(
		StartLoadingRequest batch,
		string url,
		string message = "Приемка начата")
	{
		return await ExecuteHttpRequestAsync<StartLoadingRequest, BatchVM>(Method.Post, $"api/{url}", batch);
	}

	public async Task<ErrorOr<Success>> StopLoading(
		StopLoadingRequest stopLoadingVm,
		string url,
		string message = "Приемка закончена")
	{
		return await ExecuteHttpRequestAsync<StopLoadingRequest, Success>(Method.Patch, $"api/{url}", stopLoadingVm);
	}

	public async Task<ErrorOr<BallonsAmountVm>> GetScannedBallonsAmount(int batchId, string batchType)
	{
		string url = batchType.Equals("Приемка") ? "balloons-loading" : "balloons-unloading";

		return await ExecuteHttpRequestAsync<Empty, BallonsAmountVm>(Method.Get, $"api/{url}/{batchId}/rfid-amount/");
	}

	public async Task<ErrorOr<BatchVM>> GetLastActiveBatch(string batchType)
	{
		return await ExecuteHttpRequestAsync<Empty, BatchVM>(Method.Get, $"api/{batchType}");
	}

	public async Task<ErrorOr<Success>> AddBallonToBatch(
		int ballonId,
		int batchId,
		string batchType,
		string message = "Баллон добавлен в партию")
	{
		string url = batchType.Equals("Приемка") ? "balloons-loading" : "balloons-unloading";

		return await ExecuteHttpRequestAsync<AddOrDeleteBallonToBatchVm, Success>(
			Method.Patch, $"api/{url}/{batchId}/add-balloon/", new AddOrDeleteBallonToBatchVm(ballonId));
	}

	public async Task<ErrorOr<Success>> DeleteBallonFromBatch(
		int ballonId,
		int batchId,
		string batchType,
		string message = "Баллон удален из партии")
	{
		string url = batchType.Equals("Приемка") ? "balloons-loading" : "balloons-unloading";

		return await ExecuteHttpRequestAsync<AddOrDeleteBallonToBatchVm, Success>(
			Method.Patch, $"api/{url}/{batchId}/remove-balloon/", new AddOrDeleteBallonToBatchVm(ballonId));
	}

	public async Task<ErrorOr<IEnumerable<BatchVM>>> GetActiveBatchList(string batchListType)
	{
		string url = batchListType.Equals("Приемка") ? "balloons-loading" : "balloons-unloading";

		return await ExecuteHttpRequestAsync<Empty, IEnumerable<BatchVM>>(Method.Get, $"api/{url}/active/");
	}
}
