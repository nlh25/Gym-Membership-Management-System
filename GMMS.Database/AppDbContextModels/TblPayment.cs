using System;
using System.Collections.Generic;

namespace GMMS.Database.AppDbContextModels;

public partial class TblPayment
{
    public int PaymentId { get; set; }

    public int MembershipId { get; set; }

    public int PaymentMethodId { get; set; }

    public decimal Amount { get; set; }

    public string? Sspath { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual TblMembership Membership { get; set; } = null!;

    public virtual TblPaymentMethod PaymentMethod { get; set; } = null!;
}
