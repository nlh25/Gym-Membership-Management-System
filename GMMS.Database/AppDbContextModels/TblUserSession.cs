namespace GMMS.Database.AppDbContextModels;

public partial class TblUserSession
{
    public int UserSessionId { get; set; }

    public Guid SessionId { get; set; }

    public int UserId { get; set; }

    public DateTime LoginTime { get; set; }

    public DateTime ExpiredAt { get; set; }

    public bool IsExpired { get; set; }

    public virtual TblUser User { get; set; } = null!;
}