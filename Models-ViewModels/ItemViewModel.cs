using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace adr.Web.Models.ViewModels
{
    public class ItemViewModel<T> : QrBaseViewModel
    {
        public T Item { get; set; }
    }
}