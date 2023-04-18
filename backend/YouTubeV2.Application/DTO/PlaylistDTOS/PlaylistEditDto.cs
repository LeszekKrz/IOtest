using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeV2.Api.Enums;

namespace YouTubeV2.Application.DTO.PlaylistDTOS
{
    public record PlaylistEditDto(string name, Visibility visibility);

}
