using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IrvinewReview.Models.Requests
{
    public class BlogInsertRequest
    {
        [Required]
        public float Latpoint { get; set; }

        [Required]
        public float Lngpoint { get; set; }

        [Required]
        public float Radius { get; set; }
    }
}