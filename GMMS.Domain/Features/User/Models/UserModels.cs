namespace GMMS.Domain.Features.User.Models;

public class UserListRequestModel
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}

public class UserListResponseModel
{
    public int TotalCount { get; set; }
    public List<UserModel> Users { get; set; } = new();
}

public class UserModel
{
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Role { get; set; } = null!;
    public bool IsActive { get; set; }
    public bool MustChangePassword { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedByUser { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedByUser { get; set; }
}

public class CreateUserRequestModel
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Role { get; set; } = "Admin";
    public bool IsActive { get; set; } = true;
}

public class UpdateUserRequestModel
{
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Role { get; set; } = null!;
    public bool IsActive { get; set; }
}

public class ResetPasswordRequestModel
{
    public int UserId { get; set; }
    public string NewPassword { get; set; } = null!;
}
