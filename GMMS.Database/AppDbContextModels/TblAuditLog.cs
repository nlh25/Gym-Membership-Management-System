namespace GMMS.Database.AppDbContextModels;

public partial class TblAuditLog
{
    public long AuditId { get; set; }

    public string TableName { get; set; } = null!;

    public string RecordId { get; set; } = null!;

    public string Action { get; set; } = null!;

    public int UserId { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public DateTime CreatedAt { get; set; }
}