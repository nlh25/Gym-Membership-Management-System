using System;
using System.Collections.Generic;

namespace GMMS.Database.AppDbContextModels;

public partial class TblMember
{
    public int MemberId { get; set; }

    public string MemberCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<TblMembership> TblMemberships { get; set; } = new List<TblMembership>();
}
