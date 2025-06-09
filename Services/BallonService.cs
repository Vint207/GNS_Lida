using CommunityToolkit.Maui.Alerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using static Services.HttpService;
using Services.Intrefaces;
using static Services.Intrefaces.IBallonService;
using ErrorOr;

namespace Services
{
    public class BallonService : IBallonService
    {
        public async Task<ErrorOr<Success>> CreateBallon(BallonVM ballon)
        {
            return await ExecuteHttpRequestAsync<BallonVM, Success>(Method.Post, $"api/balloons/", ballon);
		}

		//public async Task<ErrorOr<BallonVM>> GetBallonById(string id)
		//{
		//	return await ExecuteHttpRequestAsync<Empty, BallonVM>(Method.Get, $"api/balloons/{id}/");
		//}

		public async Task<ErrorOr<BallonVM>> GetBallonByNfc(string nfc)
		{
			return await ExecuteHttpRequestAsync<Empty, BallonVM>(Method.Get, $"api/balloons/nfc/{nfc}/");
		}

		public async Task<ErrorOr<IEnumerable<BallonVM>>> GetBallonBySerialNumber(string sn)
		{
			return await ExecuteHttpRequestAsync<Empty, IEnumerable<BallonVM>>(Method.Get, $"api/balloons/serial-number/{sn}/");	
		}

		public async Task<ErrorOr<Success>> UpdateBallonByNfc(string nfc, BallonVM ballon)
        {
			return await ExecuteHttpRequestAsync<BallonVM, Success>(Method.Patch, $"api/balloons/{nfc}/", ballon);
        }

        public async Task<ErrorOr<IEnumerable<string>>> GetBallonStateOptions()
        {
			return await ExecuteHttpRequestAsync<Empty, IEnumerable<string>>(Method.Get, $"api/balloon-status-options");
        }

    }
}

