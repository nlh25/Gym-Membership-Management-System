using System;

namespace GMMS.Database.AppDbContextModels;

public abstract class BaseEntity
{
    public bool IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}