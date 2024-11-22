using System;
using System.Collections.Generic;

namespace OrderManagement.Models;

public partial class TblOrder
{
    public int IntOrderId { get; set; }

    public int IntProductId { get; set; }

    public string StrCustomerName { get; set; } = null!;

    public decimal NumQuantity { get; set; }

    public DateTime DtOrderDate { get; set; }

    public virtual TblProduct IntProduct { get; set; } = null!;
}
