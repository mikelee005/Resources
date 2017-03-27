using IrvineReview2.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IrvineReview2.Domain
{
    public class PaginateItemsResponse<T> : ItemsResponse<T>
    {
        public int TotalCount { get; set; }

        public PaginateItemsResponse()
        {
            Items = new List<T>();
        }
    }
}