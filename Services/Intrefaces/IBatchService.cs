using ErrorOr;
using System.Text.Json.Serialization;
﻿using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Services.BatchService;

namespace Services.Intrefaces;

public interface IBatchService
{
	Task<ErrorOr<IEnumerable<CarVM>>> GetStationCars();

	Task<ErrorOr<IEnumerable<TrailerVM>>> GetStationTrailers();

	Task<ErrorOr<IEnumerable<int>>> GetStationLoadingReaders();

	Task<ErrorOr<IEnumerable<int>>> GetStationUnloadingReaders();

	Task<ErrorOr<BatchVM>> StartLoading(
		StartLoadingRequest batch,
		string url,
		string message = "Приемка начата");

	Task<ErrorOr<Success>> StopLoading(
		StopLoadingRequest stopLoadingVm,
		string url,
		string message = "Приемка закончена");

	Task<ErrorOr<BallonsAmountVm>> GetScannedBallonsAmount(int batchId, string batchType);

	Task<ErrorOr<BatchVM>> GetLastActiveBatch(string batchType);

	Task<ErrorOr<Success>> AddBallonToBatch(
		int ballonId,
		int batchId,
		string batchType,
		string message = "Баллон добавлен в партию");

	Task<ErrorOr<Success>> DeleteBallonFromBatch(
		int ballonId,
		int batchId,
		string batchType,
		string message = "Баллон удален из партии");

	Task<ErrorOr<IEnumerable<ActiveBatchVM>>> GetActiveBatchList(string batchListType);

	public enum ReaderType { Loading, Unloading }

	public record struct AddOrDeleteBallonToBatchVm([property: JsonPropertyName("balloon_id")] int BallonId);

	public record struct StartLoadingRequest(
		[property: JsonPropertyName("truck")] int TruckId,
		[property: JsonPropertyName("reader_number")] int ReaderNumber,
		[property: JsonPropertyName("is_active")] bool IsActive,
		[property: JsonPropertyName("ttn")] string TTN,
		[property: JsonPropertyName("amount_of_ttn")] int? AmountOfTtn);

	public record struct StopLoadingRequest(
		[property: JsonPropertyName("amount_of_5_liters")] int Amount5,
		[property: JsonPropertyName("amount_of_12_liters")] int Amount12,
		[property: JsonPropertyName("amount_of_27_liters")] int Amount27,
		[property: JsonPropertyName("amount_of_50_liters")] int Amount50,
		[property: JsonPropertyName("is_active")] bool IsActive,
		[property: JsonPropertyName("ttn")] string TTN,
		[property: JsonPropertyName("amount_of_ttn")] int? AmountOfTtn);

	public record struct BatchListVM(
		[property: JsonPropertyName("id")] int? Id,
		[property: JsonPropertyName("begin_date")] DateOnly? BeginDate,
		[property: JsonPropertyName("begin_time")] TimeOnly? BeginTime,
		[property: JsonPropertyName("truck")] BatchCarVM Car,
		[property: JsonPropertyName("reader_number")] int? ReaderNumber,
		[property: JsonPropertyName("ttn")] string TTN,
		[property: JsonPropertyName("amount_of_ttn")] int? AmountOfTtn);

	public record struct BatchCarVM(
		[property: JsonPropertyName("id")] int? Id,
		[property: JsonPropertyName("car_brand")] string Brand,
		[property: JsonPropertyName("registration_number")] string Number);

	public record struct BatchVM(
		[property: JsonPropertyName("id")] int? Id,
		[property: JsonPropertyName("begin_date")] DateOnly? BeginDate,
		[property: JsonPropertyName("begin_time")] TimeOnly? BeginTime,
		[property: JsonPropertyName("end_date")] DateOnly? EndDate,
		[property: JsonPropertyName("end_time")] TimeOnly? EndTime,
		[property: JsonPropertyName("truck")] int? Truck,
		[property: JsonPropertyName("trailer")] int? Trailer,
		[property: JsonPropertyName("reader_number")] int? ReaderNumber,
		[property: JsonPropertyName("amount_of_rfid")] int? AmountOfRfid,
		[property: JsonPropertyName("amount_of_5_liters")] int? AmountOf5Liters,
		[property: JsonPropertyName("amount_of_12_liters")] int? AmountOf12Liters,
		[property: JsonPropertyName("amount_of_27_liters")] int? AmountOf27Liters,
		[property: JsonPropertyName("amount_of_50_liters")] int? AmountOf50Liters,
		[property: JsonPropertyName("gas_amount")] int? GasAmount,
		[property: JsonPropertyName("is_active")] bool IsActive,
		[property: JsonPropertyName("ttn")] string TTN,
		[property: JsonPropertyName("amount_of_ttn")] int? AmountOfTTN);

	public record struct ActiveBatchVM(
		[property: JsonPropertyName("id")] int? Id,
		[property: JsonPropertyName("begin_date")] DateOnly? BeginDate,
		[property: JsonPropertyName("begin_time")] TimeOnly? BeginTime,
		[property: JsonPropertyName("end_date")] DateOnly? EndDate,
		[property: JsonPropertyName("end_time")] TimeOnly? EndTime,
		[property: JsonPropertyName("truck")] BatchCarVM? Truck,
		[property: JsonPropertyName("trailer")] int? Trailer,
		[property: JsonPropertyName("reader_number")] int? ReaderNumber,
		[property: JsonPropertyName("amount_of_rfid")] int? AmountOfRfid,
		[property: JsonPropertyName("amount_of_5_liters")] int? AmountOf5Liters,
		[property: JsonPropertyName("amount_of_12_liters")] int? AmountOf12Liters,
		[property: JsonPropertyName("amount_of_27_liters")] int? AmountOf27Liters,
		[property: JsonPropertyName("amount_of_50_liters")] int? AmountOf50Liters,
		[property: JsonPropertyName("gas_amount")] int? GasAmount,
		[property: JsonPropertyName("is_active")] bool IsActive,
		[property: JsonPropertyName("ttn")] string TTN,
		[property: JsonPropertyName("amount_of_ttn")] int? AmountOfTTN);

	public record struct CarVM(
		[property: JsonPropertyName("id")] int Id,
		[property: JsonPropertyName("car_brand")] string Brand,
		[property: JsonPropertyName("registration_number")] string Number,
		[property: JsonPropertyName("type")] string Type,
		[property: JsonPropertyName("entry_date")] DateOnly? EntryDate,
		[property: JsonPropertyName("entry_time")] TimeOnly? EntryTime,
		[property: JsonPropertyName("departure_date")] DateOnly? DepartureDate,
		[property: JsonPropertyName("departure_time")] TimeOnly? DepartureTime,
		[property: JsonPropertyName("is_on_station")] bool IsOnStation);

	public record struct TrailerVM(
		[property: JsonPropertyName("id")] int? Id,
		[property: JsonPropertyName("trailer_brand")] string? Brand,
		[property: JsonPropertyName("registration_number")] string? Number,
		[property: JsonPropertyName("type")] string? Type,
		[property: JsonPropertyName("is_on_station")] bool? IsOnStation);

	public record struct BallonsAmountVm(
		[property: JsonPropertyName("id")] int? Id,
		[property: JsonPropertyName("amount_of_rfid")] int? Amount);
}