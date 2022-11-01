using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRecordSchedules.Core.ApplicationServices;
using Tgs.GameRecordSchedules.Core.Models;

namespace Tgs.GameRecordSchedules.Core.UnitTest;

[TestClass]
public class GameRecordScheduleUpdateServiceUnitTest
{
	[TestMethod]
	public async Task Normal_Process()
	{
		var fakerGameRecordSchedule = Substitute.For<IGameRecordSchedule>();

		var gameRecordScheduleId = Guid.NewGuid();

		var data = new GameRecordScheduleUpdateRequest()
		{
			GameRecordScheduleId = gameRecordScheduleId,
			Config = JsonSerializer.Serialize(new { })
		};

		var service = new GameRecordScheduleUpdateService(fakerGameRecordSchedule);

		_ = await service.Handle(data, default).ConfigureAwait(false);

		await fakerGameRecordSchedule
			.Received(1)
			.UpdateAsync(
			Arg.Any<GameRecordScheduleUpdate>()).ConfigureAwait(false);
	}
}
