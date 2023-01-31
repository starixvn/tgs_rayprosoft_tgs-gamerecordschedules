using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRecordSchedules.Core.ApplicationServices;
using Tgs.GameRecordSchedules.Core.Models;

namespace Tgs.GameRecordSchedules.Core.UnitTest;

[TestClass]
public class GameRecordScheduleAddServiceUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var fakerGameRecordSchedule = Substitute.For<IGameRecordSchedule>();

        var data = new GameRecordScheduleAddRequest()
        {
            GameProviderId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            GameProviderCode = "KY",
            BrandCode = "Default",
            GameProviderAgentId = "72412",
            CurrencyCode = "CNY",
            Config = JsonSerializer.Serialize(new { })
        };

        var gameRecordScheduleId = Guid.NewGuid();
        _ = fakerGameRecordSchedule.AddAsync(Arg.Any<GameRecordScheduleAdd>()).Returns(gameRecordScheduleId);

        var service = new GameRecordScheduleAddService(fakerGameRecordSchedule);

        var actual = await service.Handle(data, default).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(gameRecordScheduleId, actual);
    }
}
