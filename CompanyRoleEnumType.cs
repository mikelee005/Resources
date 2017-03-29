using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace adr.Web.Enums
{
    public enum CompanyRoleType
    {
        [Description("Buyer")]
        Buyer = 1,
        [Description("Supplier")]
        Supplier = 2,
        [Description("Contractor")]
        Contractor = 3,
        [Description("Shipper")]
        Shipper = 4
    }
}