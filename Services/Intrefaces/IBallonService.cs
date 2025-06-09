using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Services.BallonService;
using static Services.HttpService;

namespace Services.Intrefaces
{
    public interface IBallonService
    {
		Task<ErrorOr<Success>> CreateBallon(BallonVM ballon);
		//Task<ErrorOr<BallonVM>> GetBallonById(string id);
		Task<ErrorOr<BallonVM>> GetBallonByNfc(string nfc);
		Task<ErrorOr<IEnumerable<BallonVM>>> GetBallonBySerialNumber(string sn);
		Task<ErrorOr<Success>> UpdateBallonByNfc(string nfc, BallonVM ballon);
        Task<ErrorOr<IEnumerable<string>>> GetBallonStateOptions();

		public class BallonVM
		{
			//[JsonPropertyName("id")]
			//public int? Id { get; set; }

			[JsonPropertyName("nfc_tag")]
			public string? NFC_Tag { get; set; }

			[JsonPropertyName("serial_number")]
			public string? Serial_Number { get; set; }

			[JsonPropertyName("creation_date")]
			public DateOnly? Creation_Date { get; set; }

			[JsonPropertyName("size")]
			public float? Size { get; set; }

			[JsonPropertyName("netto")]
			public float? Netto { get; set; }

			[JsonPropertyName("brutto")]
			public float? Brutto { get; set; }

			[JsonPropertyName("current_examination_date")]
			public DateOnly? Current_Examination_Date { get; set; }

			[JsonPropertyName("next_examination_date")]
			public DateOnly? Next_Examination_Date { get; set; }

			[JsonPropertyName("status")]
			public string? Status { get; set; }

			[JsonPropertyName("manufacturer")]
			public string? Manufacturer { get; set; }

			[JsonPropertyName("wall_thickness")]
			public float? WallThickness { get; set; }

			[JsonPropertyName("filling_status")]
			public bool? FillingStatus { get; set; }

			[JsonPropertyName("update_passport_required")]
			public bool? UpdatePassportRequired { get; set; }
		}
	}
}
