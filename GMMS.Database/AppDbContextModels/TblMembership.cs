using System;
using System.Collections.Generic;

namespace GMMS.Database.AppDbContextModels;

public partial class TblMembership : BaseEntity
{
    public int MembershipId { get; set; }

    public int MemberId { get; set; }

    public int MembershipPlanId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual TblMember Member { get; set; } = null!;

    public virtual TblMembershipPlan MembershipPlan { get; set; } = null!;

    public virtual ICollection<TblPayment> TblPayments { get; set; } = new List<TblPayment>();
}
