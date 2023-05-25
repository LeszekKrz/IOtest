using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using YouTubeV2.Api.Attributes;
using YouTubeV2.Application.Services;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Enums;
using YouTubeV2.Application.DTO.SearchDTOS;

namespace YouTubeV2.Api.Controllers
{
    [ApiController]
    public class SearchController : IdentityControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet("api/search")]
        [Roles(Role.Simple, Role.Creator, Role.Administrator)]
        public async Task<ActionResult<SearchResultsDto>> SearchAsync([FromQuery][Required] string query, 
            [FromQuery][Required] SortingTypes sortingCriterion, [FromQuery][Required] SortingDirections sortingType,
            [FromQuery] DateTime? beginDate, [FromQuery] DateTime? endDate, 
            CancellationToken cancellationToken)
        {
            if (GetUserId() == null) return Forbid();

            return await _searchService.SearchAsync(query, sortingType, 
                sortingCriterion, beginDate, endDate, cancellationToken);
        }
    }
}
