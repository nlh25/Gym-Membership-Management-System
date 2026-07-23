using System;
using System.Collections.Generic;

namespace GMMS.Database.AppDbContextModels;

public partial class TblMember : BaseEntity
{
    public int MemberId { get; set; }

    public string MemberCode { get; set; } = null!;

    public string Name { get; set; } = null!; 

    public virtual ICollection<TblMembership> TblMemberships { get; set; } = new List<TblMembership>();
}
