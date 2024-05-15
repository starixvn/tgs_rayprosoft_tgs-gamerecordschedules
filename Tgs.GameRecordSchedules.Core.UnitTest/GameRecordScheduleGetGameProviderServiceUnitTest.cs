using System;
using System.Text.Json;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameRecordSchedules.Core.ApplicationServices;
using Tgs.GameRecordSchedules.Core.Models;

namespace Tgs.GameRecordSchedules.Core.UnitTest;

public class GameRecordScheduleGetGameProviderServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakerRepository = Substitute.For<IGameRecordScheduleRepository>();

		var gameRecordScheduleId = Guid.NewGuid();
		var request = new GameRecordScheduleGetGameProviderRequest()
		{
			GameProviderCode = "KY",
			BrandCode = "Default",
			GameProviderAgentId = "72412",
			CurrencyCode = "CNY"
		};

		var gameRecordSchedule = new GameRecordScheduleGet()
		{
			GameRecordScheduleId = gameRecordScheduleId,
			GameProviderId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
			GameProviderCode = "KY",
			BrandCode = "Default",
			GameProviderAgentId = "72412",
			CurrencyCode = "CNY",
			Config = JsonSerializer.Serialize(new { }),
			CreatedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow
		};

		_ = fakerRepository.GetAsync(
			Arg.Is(request.GameProviderCode),
			Arg.Is(request.BrandCode),
			Arg.Is(request.GameProviderAgentId),
			Arg.Is(request.CurrencyCode)).Returns(gameRecordSchedule);

		var service = new GameRecordScheduleGetGameProviderService(fakerRepository);

		var actual = await service.Handle(request, default);

		Assert.NotNull(actual);
		Assert.Equal(gameRecordScheduleId, actual.GameRecordScheduleId);
	}
}
