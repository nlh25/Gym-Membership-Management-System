using System;
using System.Collections.Generic;

namespace GMMS.Database.AppDbContextModels;

public partial class TblMembershipPlan : BaseEntity
{
    public int MembershipPlanId { get; set; }

    public string PlanCode { get; set; } = null!;

    public string PlanName { get; set; } = null!;

    public decimal Price { get; set; }

    public int DurationDays { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<TblMembership> TblMemberships { get; set; } = new List<TblMembership>();
}