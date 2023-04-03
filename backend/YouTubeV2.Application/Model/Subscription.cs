using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeV2.Application.Model
{
    public class Subscription
    {
        public string SubscriberId { get ; set; }

        public virtual User Subscriber { get; set; }

        public string SubscribeeId { get; set; }

        public virtual User Subscribee { get; set; }
    }
}