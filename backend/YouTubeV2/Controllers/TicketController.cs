using Microsoft.AspNetCore.Mvc;
using YouTubeV2.Api.Attributes;
using YouTubeV2.Application.DTO.TicketDTOS;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services;

namespace YouTubeV2.Api.Controllers
{
    [ApiController]
    [Route("api/ticket")]
    public class TicketController : IdentityControllerBase
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
        public async Task<ActionResult<SubmitTicketResponseDto>> SubmitTicket([FromBody] SubmitTicketDto dto, CancellationToken cancellationToken)
        {
            string? authorId = GetUserId();
            if (authorId is null) return Forbid();

            User? author = await _userService.GetByIdAsync(authorId);
            if (author is null) return NotFound($"User with id {authorId} not found");

            var result = await _ticketService.SubmitTicketAsync(dto, author, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [Roles(Role.Simple, Role.Creator, Role.Administrator)]
        public async Task<ActionResult<GetTicketDto>> GetTicket([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var result = await _ticketService.GetTicketAsync(id, cancellationToken);
            return Ok(result);
        }

        [HttpPut]
        [Roles(Role.Administrator)]
        public async Task<ActionResult<SubmitTicketResponseDto>> RespondToTicket([FromQuery] Guid id, [FromBody] RespondToTicketDto dto, CancellationToken cancellationToken)
        {
            var result = await _ticketService.RespondToTicketAsync(id, dto, cancellationToken);
            return Ok(result);
        }

        [HttpGet("status")]
        [Roles(Role.Simple, Role.Creator, Role.Administrator)]
        public async Task<ActionResult<GetTicketStatusDto>> GetTicketStatus([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var result = await _ticketService.GetTicketStatusAsync(id, cancellationToken);
            return Ok(result);
        }

        [HttpGet("list")]
        [Roles(Role.Administrator)]
        public async Task<ActionResult<IEnumerable<GetTicketDto>>> GetTicketList(CancellationToken cancellationToken)
        {
            var result = await _ticketService.GetTicketListAsync(cancellationToken);
            return Ok(result);
        }
    }

}
