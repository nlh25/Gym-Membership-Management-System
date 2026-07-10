using System;
using System.Collections.Generic;

namespace GMMS.Database.AppDbContextModels;

public partial class TblPaymentMethod
{
    public int PaymentMethodId { get; set; }
    public string PaymentMethodCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<TblPayment> TblPayments { get; set; } = new List<TblPayment>();
}
