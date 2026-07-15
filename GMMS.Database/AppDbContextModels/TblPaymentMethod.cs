using System;
using System.Collections.Generic;

namespace GMMS.Database.AppDbContextModels;

public partial class TblPaymentMethod : BaseEntity
{
    public int PaymentMethodId { get; set; }
    public string PaymentMethodCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<TblPayment> TblPayments { get; set; } = new List<TblPayment>();
}
