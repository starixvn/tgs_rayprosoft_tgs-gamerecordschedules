using System;
using System.Text.Json;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameRecordSchedules.Core.ApplicationServices;
using Tgs.GameRecordSchedules.Core.Models;

namespace Tgs.GameRecordSchedules.Core.UnitTest;

public class GameRecordScheduleUpdateServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakerRepository = Substitute.For<IGameRecordScheduleRepository>();

		var gameRecordScheduleId = Guid.NewGuid();

		var data = new GameRecordScheduleUpdateRequest()
		{
			GameRecordScheduleId = gameRecordScheduleId,
			Config = JsonSerializer.Serialize(new { })
		};

		var service = new GameRecordScheduleUpdateService(fakerRepository);

		_ = await service.Handle(data, default);

		await fakerRepository
			.Received(1)
			.UpdateAsync(
			source: Arg.Any<GameRecordScheduleUpdate>());
	}
}
