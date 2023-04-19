using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeV2.Api.Enums;
using YouTubeV2.Application.DTO.VideoDTOS;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.DTO.PlaylistDTOS
{
    public record PlaylistDto(string name, Visibility visibility, IEnumerable<VideoBaseDto> videos);
}
