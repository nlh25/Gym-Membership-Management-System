using System.Collections.Generic;

namespace GMMS.Database.AppDbContextModels;

public partial class TblUser : BaseEntity
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<TblUserSession> Sessions { get; set; } = new List<TblUserSession>();
}