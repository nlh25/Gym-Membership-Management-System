namespace GMMS.Domain.Features.Auth.Models;

public class LoginRequestModel
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginResponseModel
{
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}


public class ChangePasswordRequestModel
{
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
    public string ConfirmNewPassword { get; set; } = null!;
}

public class UserSessionModel
{
    public int UserSessionId { get; set; }
    public Guid SessionId { get; set; }
    public int UserId { get; set; }
    public DateTime LoginTime { get; set; }
    public DateTime? ExpiredAt { get; set; }
    public bool IsActive { get; set; }
}