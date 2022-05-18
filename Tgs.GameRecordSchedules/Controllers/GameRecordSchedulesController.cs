using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecordSchedules.Core.ApplicationServices;
using Tgs.GameRecordSchedules.ViewModels;

namespace Tgs.GameRecordSchedules.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GameRecordSchedulesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GameRecordSchedulesController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        public async ValueTask<Guid?> AddAsync([FromBody] GameRecordScheduleAddViewModel model)
            => await _mediator.Send(new GameRecordScheduleAddRequest()
            {
                GameProviderId = model.GameProviderId,
                GameProviderCode = model.GameProviderCode,
                GameProviderAgentId = model.GameProviderAgentId,
                BrandCode = model.BrandCode,
                CurrencyCode = model.CurrencyCode,
                Config = model.Config
            }).ConfigureAwait(false);

        [HttpGet("{gameProviderCode}")]
        public async ValueTask<GameRecordScheduleGetViewModel?> GetAsync(
            string gameProviderCode,
            [FromQuery] string? brandCode,
            [FromQuery] string? gameProviderAgentId,
            [FromQuery] string? currencyCode
            )
        {
            var response = await _mediator.Send(new GameRecordScheduleGetGameProviderRequest()
            {
                GameProviderCode = gameProviderCode.ToUpper(),
                BrandCode = brandCode,
                GameProviderAgentId = gameProviderAgentId,
                CurrencyCode = currencyCode
            }).ConfigureAwait(false);

            return response is not null
                ? new GameRecordScheduleGetViewModel()
                {
                    GameRecordScheduleId = response.GameRecordScheduleId,
                    GameProviderId = response.GameProviderId,
                    GameProviderCode = response.GameProviderCode,
                    BrandCode = response.BrandCode,
                    GameProviderAgentId = response.GameProviderAgentId,
                    CurrencyCode = response.CurrencyCode,
                    Status = (Protos.GameRecordScheduleStatus)response.Status,
                    Config = response.Config,
                    CreatedOn = response.CreatedOn,
                    UpdatedOn = response.UpdatedOn,
                } : null;
        }

        [HttpPut("{gameRecordScheduleId}")]
        public async ValueTask UpdateAsync(
            Guid gameRecordScheduleId,
            [FromBody] GameRecordScheduleUpdateViewModel model)
        {
            _ = await _mediator.Send(new GameRecordScheduleUpdateRequest()
            {
                GameRecordScheduleId = gameRecordScheduleId,
                Config = model.Config
            }).ConfigureAwait(false);
        }
    }
}
