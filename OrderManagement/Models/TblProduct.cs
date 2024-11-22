using System;
using System.Collections.Generic;

namespace OrderManagement.Models;

public partial class TblProduct
{
    public int IntProductId { get; set; }

    public string StrProductName { get; set; } = null!;

    public decimal NumUnitPrice { get; set; }

    public decimal NumStock { get; set; }

    public virtual ICollection<TblOrder> TblOrders { get; set; } = new List<TblOrder>();
}
