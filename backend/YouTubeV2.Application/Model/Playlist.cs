using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeV2.Api.Enums;
using YouTubeV2.Application.DTO.UserDTOS;

namespace YouTubeV2.Application.Model
{
    public class Playlist
    {
        public Guid Id { get; init; }
        public User Creator { get; init; } = null!;
        public string Name { get; init; } = null!;
        public Visibility Visibility { get; init; }
        public virtual ICollection<Video> Videos { get; init; } = null!;

        public Playlist()
        { 
            Videos = new ObservableCollection<Video>();
        }
    }
}
