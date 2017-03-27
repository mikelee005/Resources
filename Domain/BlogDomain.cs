using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IrvineReview.Domain
{
    public class MapDomain
    {
        public int MapId { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public double DistanceInMile { get; set; }
    }
}
