using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Services.BatchService;
using static Services.CarouselService;

namespace Services.Intrefaces;

public interface ICarouselService
{
	Task<ErrorOr<GetCarouselCorrectionResponse>> GetCarouselParameters();

	public readonly record struct GetCarouselCorrectionResponse(
		[property: JsonPropertyName("id")] int Id,
		[property: JsonPropertyName("read_only")] bool ReadOnly,
		[property: JsonPropertyName("use_weight_management")] bool UseWeightManagement,
		[property: JsonPropertyName("use_common_correction")] bool UseCommonCorrection,
		[property: JsonPropertyName("weight_correction_value")] double WeightCorrectionValue,
		[property: JsonPropertyName("post_1_correction")] double Post1Correction,
		[property: JsonPropertyName("post_2_correction")] double Post2Correction,
		[property: JsonPropertyName("post_3_correction")] double Post3Correction,
		[property: JsonPropertyName("post_4_correction")] double Post4Correction,
		[property: JsonPropertyName("post_5_correction")] double Post5Correction,
		[property: JsonPropertyName("post_6_correction")] double Post6Correction,
		[property: JsonPropertyName("post_7_correction")] double Post7Correction,
		[property: JsonPropertyName("post_8_correction")] double Post8Correction,
		[property: JsonPropertyName("post_9_correction")] double Post9Correction,
		[property: JsonPropertyName("post_10_correction")] double Post10Correction,
		[property: JsonPropertyName("post_11_correction")] double Post11Correction,
		[property: JsonPropertyName("post_12_correction")] double Post12Correction,
		[property: JsonPropertyName("post_13_correction")] double Post13Correction,
		[property: JsonPropertyName("post_14_correction")] double Post14Correction,
		[property: JsonPropertyName("post_15_correction")] double Post15Correction,
		[property: JsonPropertyName("post_16_correction")] double Post16Correction,
		[property: JsonPropertyName("post_17_correction")] double Post17Correction,
		[property: JsonPropertyName("post_18_correction")] double Post18Correction,
		[property: JsonPropertyName("post_19_correction")] double Post19Correction,
		[property: JsonPropertyName("post_20_correction")] double Post20Correction);
	public readonly record struct SetCarouselCorrectionRequest(
		[property: JsonPropertyName("id")] int Id,
		[property: JsonPropertyName("read_only")] bool ReadOnly,
		[property: JsonPropertyName("use_weight_management")] bool UseWeightManagement,
		[property: JsonPropertyName("use_common_correction")] bool UseCommonCorrection,
		[property: JsonPropertyName("weight_correction_value")] double WeightCorrectionValue,
		[property: JsonPropertyName("post_1_correction")] double Post1Correction,
		[property: JsonPropertyName("post_2_correction")] double Post2Correction,
		[property: JsonPropertyName("post_3_correction")] double Post3Correction,
		[property: JsonPropertyName("post_4_correction")] double Post4Correction,
		[property: JsonPropertyName("post_5_correction")] double Post5Correction,
		[property: JsonPropertyName("post_6_correction")] double Post6Correction,
		[property: JsonPropertyName("post_7_correction")] double Post7Correction,
		[property: JsonPropertyName("post_8_correction")] double Post8Correction,
		[property: JsonPropertyName("post_9_correction")] double Post9Correction,
		[property: JsonPropertyName("post_10_correction")] double Post10Correction,
		[property: JsonPropertyName("post_11_correction")] double Post11Correction,
		[property: JsonPropertyName("post_12_correction")] double Post12Correction,
		[property: JsonPropertyName("post_13_correction")] double Post13Correction,
		[property: JsonPropertyName("post_14_correction")] double Post14Correction,
		[property: JsonPropertyName("post_15_correction")] double Post15Correction,
		[property: JsonPropertyName("post_16_correction")] double Post16Correction,
		[property: JsonPropertyName("post_17_correction")] double Post17Correction,
		[property: JsonPropertyName("post_18_correction")] double Post18Correction,
		[property: JsonPropertyName("post_19_correction")] double Post19Correction,
		[property: JsonPropertyName("post_20_correction")] double Post20Correction);

	Task<ErrorOr<Success>> SetCarouselParameters(SetCarouselCorrectionRequest request);
}

