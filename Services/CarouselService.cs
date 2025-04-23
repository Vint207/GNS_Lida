using CommunityToolkit.Maui.Alerts;
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
using static Services.Intrefaces.ICarouselService;

namespace Services;

public class CarouselService: ICarouselService
{
	public async Task<ErrorOr<GetCarouselCorrectionResponse>> GetCarouselParameters()
	{
		return await ExecuteHttpRequestAsync<Empty, GetCarouselCorrectionResponse>(Method.Get, $"api/carousel/get-parameter/");
    }



	public async Task<ErrorOr<Success>> SetCarouselParameters(SetCarouselCorrectionRequest request)
	{
		var setCarouselParametersResult = await ExecuteHttpRequestAsync<SetCarouselCorrectionRequest, Empty>(
			Method.Patch, $"api/carousel/1/", requestContent: request);

		if (setCarouselParametersResult.IsError)
			return setCarouselParametersResult.FirstError;

		return Result.Success;
    }


}
