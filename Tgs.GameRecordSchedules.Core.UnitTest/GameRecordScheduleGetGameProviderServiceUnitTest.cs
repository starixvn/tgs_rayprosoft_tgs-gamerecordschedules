using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRecordSchedules.Core.ApplicationServices;
using Tgs.GameRecordSchedules.Core.Models;

namespace Tgs.GameRecordSchedules.Core.UnitTest
{
    [TestClass]
    public class GameRecordScheduleGetGameProviderServiceUnitTest
    {
        [TestMethod]
        public async Task Normal_Process()
        {
            var fakerGameRecordSchedule = Substitute.For<IGameRecordSchedule>();

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

            _ = fakerGameRecordSchedule.GetAsync(
                Arg.Is(request.GameProviderCode),
                Arg.Is(request.BrandCode),
                Arg.Is(request.GameProviderAgentId),
                Arg.Is(request.CurrencyCode)).Returns(gameRecordSchedule);

            var service = new GameRecordScheduleGetGameProviderService(fakerGameRecordSchedule);

            var actual = await service.Handle(request, default).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(gameRecordScheduleId, actual.GameRecordScheduleId);
        }
    }
}
