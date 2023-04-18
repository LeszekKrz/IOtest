using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YouTubeV2.Api.Attributes;
using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Exceptions;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services;
using YouTubeV2.Application.Services.TicketServices;

namespace YouTubeV2.Api.Controllers
{
    [ApiController]
    [Route("ticket")]
    public class TicketController : ControllerBase
    {
        ITicketService _ticketService;
        IUserService _userService;

        public TicketController(ITicketService ticketService, IUserService userService)
        {
            _ticketService = ticketService;
            _userService = userService;
        }

        [HttpPost]
        [Roles(Role.Simple, Role.Creator, Role.Administrator)]
        public async Task<Guid> SubmitTicketAsync(SubmitTicketDTO submitTicketDto, CancellationToken cancellationToken)
        {
            string userId = GetUserId();
            User? user = await _userService.GetByIdAsync(userId);
            if (user == null) throw new BadRequestException("There is no user identifiable by given token");

            Guid ticketId = await _ticketService.SubmitTicketAsync(submitTicketDto, user, cancellationToken);

            return ticketId;
        }

        [HttpGet]
        [Roles(Role.Simple, Role.Creator, Role.Administrator)]
        public async Task<GetTicketDTO> GetTicketAsync([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            string userId = GetUserId();
            User? user = await _userService.GetByIdAsync(userId);
            if (user == null) throw new BadRequestException("There is no user identifiable by given token");

            if (GetUserRole() != Role.Administrator && id.ToString() != userId)
                throw new BadRequestException("Not authorized");

            var ticketDto = await _ticketService.GetTicketAsync(id, cancellationToken);

            return ticketDto;
        }

        [HttpPut]
        [Roles(Role.Administrator)]
        public async Task<Guid> RespondToTicketAsync([FromQuery] Guid id, [FromBody] RespondToTicketDTO respondToTicketDTO, CancellationToken cancellationToken)
        {
            await _ticketService.RespondToTicketAsync(id, respondToTicketDTO.response, cancellationToken);

            return id;
        }

        [HttpGet]
        [Roles(Role.Simple, Role.Creator, Role.Administrator)]
        public async Task<GetTicketStatusDTO> GetTicketStatusAsync([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            string userId = GetUserId();
            User? user = await _userService.GetByIdAsync(userId);
            if (user == null) throw new BadRequestException("There is no user identifiable by given token");

            if (GetUserRole() != Role.Administrator && id.ToString() != userId)
                throw new BadRequestException("Not authorized");

            var ticketDto = await _ticketService.GetTicketStatusAsync(id, cancellationToken);

            return ticketDto;
        }

        [HttpGet]
        [Roles(Role.Simple, Role.Creator, Role.Administrator)]
        public async Task<IEnumerable<GetTicketDTO>> GetTicketListAsync(CancellationToken cancellationToken)
        {
            string userId = GetUserId();
            var ticketsDto = await _ticketService.GetTicketListAsync(userId, cancellationToken);

            return ticketsDto;
        }

        private string GetUserId() => User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
        private string GetUserRole() => User.Claims.First(claim => claim.Type == ClaimTypes.Role).Value;
    }
}
