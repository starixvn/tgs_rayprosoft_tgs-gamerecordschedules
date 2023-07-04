using System;
using System.Text.Json;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameRecordSchedules.Core.ApplicationServices;
using Tgs.GameRecordSchedules.Core.Models;

namespace Tgs.GameRecordSchedules.Core.UnitTest;

public class GameRecordScheduleAddServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakerRepository = Substitute.For<IGameRecordScheduleRepository>();

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
        _ = fakerRepository.AddAsync(Arg.Any<GameRecordScheduleAdd>()).Returns(gameRecordScheduleId);

        var service = new GameRecordScheduleAddService(fakerRepository);

        var actual = await service.Handle(data, default).ConfigureAwait(false);

        Assert.Equal(gameRecordScheduleId, actual);
    }
}
