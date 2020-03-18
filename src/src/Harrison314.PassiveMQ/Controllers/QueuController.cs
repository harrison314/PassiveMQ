using Harrison314.PassiveMQ.Models;
using Harrison314.PassiveMQ.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Controllers
{
    [Route("api/[controller]")]
    [ProducesResponseType(typeof(ErrorResponseDto), 400)]
    [ProducesResponseType(typeof(ErrorResponseDto), 500)]
    public class QueuController : Controller
    {
        private readonly IQueuRepository queuRepository;
        private readonly INotificationSender notificationSender;
        private readonly ILogger<QueuController> logger;

        public QueuController(IQueuRepository queuRepository, INotificationSender notificationSender, ILogger<QueuController> logger)
        {
            this.queuRepository = queuRepository;
            this.notificationSender = notificationSender;
            this.logger = logger;
        }

        [HttpGet("{id}", Name = "GetQueuById")]
        [ProducesResponseType(typeof(QueuDto), 200)]
        public async Task<IActionResult> Get(Guid id)
        {
            this.logger.LogTrace("Call Get with Id={0}", id);

            QueuDto dto = await this.queuRepository.GetByIdAsync(id).ConfigureAwait(false);
            return this.Ok(dto);
        }

        [HttpGet("name/{name}", Name = "GetQueuByName")]
        [ProducesResponseType(typeof(QueuDto), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> Get([FromRoute]string name)
        {
            this.logger.LogTrace("Call get with name={0}", name);

            name = name.Trim();
            QueuDto dto = await this.queuRepository.GetByNameOrDefaultAsync(name).ConfigureAwait(false);
            if (dto == null)
            {
                return this.NotFound();
            }
            else
            {
                return this.Ok(dto);
            }
        }

        [HttpGet(Name ="GetAll")]
        [ProducesResponseType(typeof(IEnumerable<QueuDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            this.logger.LogTrace("Call GetAll");
            List<QueuDto> queus = await this.queuRepository.GetAllAsync().ConfigureAwait(false);

            return this.Ok(queus);
        }

        [HttpPost(Name = "CreateQueu")]
        [ProducesResponseType(typeof(QueuDto), 201)]
        [ProducesResponseType(typeof(QueuDto), 200)]
        public async Task<IActionResult> Post([FromBody]QueuCreateReqDto queuDto, [FromQuery] bool explicid = true)
        {
            this.logger.LogTrace("Call Post for Queu name='{0}', topic pattern='{1}', Notification Adress='{2}'.", queuDto.Name, queuDto.TopicPattern, queuDto.NotificationAdress);
            if (explicid == true)
            {
                Guid id = await this.queuRepository.CreateAsync(queuDto).ConfigureAwait(false);
                await this.notificationSender.SetNotificationAdress(id, queuDto.NotificationAdress).ConfigureAwait(false);

                QueuDto dto = await this.queuRepository.GetByIdAsync(id).ConfigureAwait(false);
                this.logger.LogInformation("Create queu with id={0}", dto.Id);
                return this.CreatedAtAction(nameof(this.Get), new { id = id }, dto);
            }
            else
            {
                QueuDto dto = await this.queuRepository.GetByNameOrDefaultAsync(queuDto.Name).ConfigureAwait(false);
                if (dto == null)
                {
                    Guid id = await this.queuRepository.CreateAsync(queuDto).ConfigureAwait(false);
                    await this.notificationSender.SetNotificationAdress(id, queuDto.NotificationAdress).ConfigureAwait(false);
                    dto = await this.queuRepository.GetByIdAsync(id).ConfigureAwait(false);

                    this.logger.LogInformation("Create queu with id={0}", dto.Id);
                    return this.CreatedAtAction(nameof(this.Get), new { id = id }, dto);
                }
                else
                {
                    return this.Ok(dto);
                }
            }
        }

        [HttpPut("{id}/NotificationAdress", Name = "PutNotificationAdress")]
        [ProducesResponseType(typeof(NotificationAdressUpdateDto), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> PutNotificationAdress(Guid id, [FromBody]NotificationAdressUpdateDto updateDto)
        {
            this.logger.LogTrace("Call PutNotificationAdress with id={0} NotificationAdress='{1}'.", id, updateDto?.NotificationAdress);
            bool hasUpdated = await this.notificationSender.SetNotificationAdress(id, updateDto.NotificationAdress).ConfigureAwait(false);
            if (hasUpdated)
            {
                this.logger.LogInformation("Update NotificationAdress with id={0} NotificationAdress='{1}'.", id, updateDto?.NotificationAdress);

                updateDto.QueuId = id;
                return this.Ok(updateDto);
            }
            else
            {
                return this.NotFound();
            }
        }

        [HttpDelete("{id}", Name = "DeleteQueu")]
        [ProducesResponseType(typeof(void), 204)]
        public async Task<IActionResult> Delete(Guid id)
        {
            this.logger.LogTrace("Call delete with Id={0}", id);
            await this.queuRepository.DeleteAsync(id).ConfigureAwait(false);

            this.logger.LogInformation("Delete Queu with id={0}", id);
            return this.NoContent();
        }
    }
}
