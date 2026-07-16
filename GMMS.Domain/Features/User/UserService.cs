using FluentValidation;
using GMMS.Database.AppDbContextModels;
using GMMS.Domain.Features.User.Models;
using Microsoft.EntityFrameworkCore;

namespace GMMS.Domain.Features.User;

public class UserService
{
    private readonly AppDbContext _db;
    private readonly IValidator<UserListRequestModel> _listValidator;
    private readonly IValidator<CreateUserRequestModel> _createValidator;
    private readonly IValidator<UpdateUserRequestModel> _updateValidator;
    private readonly IValidator<ResetPasswordRequestModel> _resetPasswordValidator;

    public UserService(
        AppDbContext db,
        IValidator<UserListRequestModel> listValidator,
        IValidator<CreateUserRequestModel> createValidator,
        IValidator<UpdateUserRequestModel> updateValidator,
        IValidator<ResetPasswordRequestModel> resetPasswordValidator)
    {
        _db = db;
        _listValidator = listValidator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _resetPasswordValidator = resetPasswordValidator;
    }

    public Result<UserListResponseModel> GetList(UserListRequestModel request)
    {
        var validationResult = _listValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return new Result<UserListResponseModel>
            {
                IsSuccess = false,
                Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
            };
        }

        try
        {
            var query = _db.TblUsers
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.Trim().ToLower();
                query = query.Where(x => x.UserName.ToLower().Contains(search) || x.Role.ToLower().Contains(search));
            }

            var totalCount = query.Count();

            var users = query
                .OrderByDescending(x => x.UserId)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new UserModel
                {
                    UserId = x.UserId,
                    UserName = x.UserName,
                    Role = x.Role,
                    IsActive = x.IsActive,
                    MustChangePassword = x.MustChangePassword,
                    CreatedAt = x.CreatedAt,
                    CreatedByUser = x.CreatedBy + " - " + _db.TblUsers.Where(u => u.UserId == x.CreatedBy).Select(u => u.UserName).FirstOrDefault(),
                    UpdatedAt = x.UpdatedAt,
                    UpdatedByUser = x.UpdatedBy.HasValue
                        ? x.UpdatedBy.Value + " - " + _db.TblUsers.Where(u => u.UserId == x.UpdatedBy.Value).Select(u => u.UserName).FirstOrDefault()
                        : null
                })
                .ToList();

            return new Result<UserListResponseModel>
            {
                IsSuccess = true,
                Message = "Users retrieved successfully.",
                Data = new UserListResponseModel
                {
                    TotalCount = totalCount,
                    Users = users
                }
            };
        }
        catch (Exception ex)
        {
            return new Result<UserListResponseModel>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public Result<UserModel> GetById(int userId)
    {
        try
        {
            var user = _db.TblUsers
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.UserId == userId)
                .Select(x => new UserModel
                {
                    UserId = x.UserId,
                    UserName = x.UserName,
                    Role = x.Role,
                    IsActive = x.IsActive,
                    MustChangePassword = x.MustChangePassword,
                    CreatedAt = x.CreatedAt,
                    CreatedByUser = x.CreatedBy + " - " + _db.TblUsers.Where(u => u.UserId == x.CreatedBy).Select(u => u.UserName).FirstOrDefault(),
                    UpdatedAt = x.UpdatedAt,
                    UpdatedByUser = x.UpdatedBy.HasValue
                        ? x.UpdatedBy.Value + " - " + _db.TblUsers.Where(u => u.UserId == x.UpdatedBy.Value).Select(u => u.UserName).FirstOrDefault()
                        : null
                })
                .FirstOrDefault();

            if (user == null)
            {
                return new Result<UserModel>
                {
                    IsSuccess = false,
                    Message = "User not found."
                };
            }

            return new Result<UserModel>
            {
                IsSuccess = true,
                Message = "User retrieved successfully.",
                Data = user
            };
        }
        catch (Exception ex)
        {
            return new Result<UserModel>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public Result<UserModel> Create(CreateUserRequestModel request)
    {
        var validationResult = _createValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return new Result<UserModel>
            {
                IsSuccess = false,
                Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
            };
        }

        try
        {
            request.UserName = request.UserName.Trim().ToLowerInvariant();

            var exists = _db.TblUsers
                .Any(x => !x.IsDeleted && x.UserName == request.UserName);

            if (exists)
            {
                return new Result<UserModel>
                {
                    IsSuccess = false,
                    Message = "Username already exists."
                };
            }

            var user = new TblUser
            {
                UserName = request.UserName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role,
                IsActive = request.IsActive,
                MustChangePassword = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _db.TblUsers.Add(user);
            _db.SaveChanges();

            return new Result<UserModel>
            {
                IsSuccess = true,
                Message = "User created successfully.",
                Data = new UserModel
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    MustChangePassword = user.MustChangePassword,
                    CreatedAt = user.CreatedAt,
                    CreatedByUser = user.CreatedBy + " - " + _db.TblUsers.Where(u => u.UserId == user.CreatedBy).Select(u => u.UserName).FirstOrDefault()
                }
            };
        }
        catch (Exception ex)
        {
            return new Result<UserModel>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public Result<UserModel> Update(int id, UpdateUserRequestModel request)
    {
        var validationResult = _updateValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return new Result<UserModel>
            {
                IsSuccess = false,
                Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
            };
        }

        try
        {
            var user = _db.TblUsers
                .FirstOrDefault(x => !x.IsDeleted && x.UserId == request.UserId);

            if (user == null)
            {
                return new Result<UserModel>
                {
                    IsSuccess = false,
                    Message = "User not found."
                };
            }

            request.UserName = request.UserName.Trim().ToLowerInvariant();

            var exists = _db.TblUsers
                .Any(x => !x.IsDeleted && x.UserName == request.UserName && x.UserId != request.UserId);

            if (exists)
            {
                return new Result<UserModel>
                {
                    IsSuccess = false,
                    Message = "Username already exists."
                };
            }

            user.UserName = request.UserName;
            user.Role = request.Role;
            user.IsActive = request.IsActive;
            user.UpdatedAt = DateTime.UtcNow;
            _db.SaveChanges();

            return new Result<UserModel>
            {
                IsSuccess = true,
                Message = "User updated successfully.",
                Data = new UserModel
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    MustChangePassword = user.MustChangePassword,
                    CreatedAt = user.CreatedAt,
                    CreatedByUser = user.CreatedBy + " - " + _db.TblUsers.Where(u => u.UserId == user.CreatedBy).Select(u => u.UserName).FirstOrDefault(),
                    UpdatedAt = user.UpdatedAt,
                    UpdatedByUser = user.UpdatedBy.HasValue
                        ? user.UpdatedBy.Value + " - " + _db.TblUsers.Where(u => u.UserId == user.UpdatedBy.Value).Select(u => u.UserName).FirstOrDefault()
                        : null
                }
            };
        }
        catch (Exception ex)
        {
            return new Result<UserModel>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public Result<bool> ResetPassword(ResetPasswordRequestModel request)
    {
        var validationResult = _resetPasswordValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return new Result<bool>
            {
                IsSuccess = false,
                Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
            };
        }

        try
        {
            var user = _db.TblUsers
                .FirstOrDefault(x => !x.IsDeleted && x.UserId == request.UserId);

            if (user == null)
            {
                return new Result<bool>
                {
                    IsSuccess = false,
                    Message = "User not found."
                };
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.MustChangePassword = true;
            user.UpdatedAt = DateTime.UtcNow;
            _db.SaveChanges();

            return new Result<bool>
            {
                IsSuccess = true,
                Message = "Password reset successfully. User must change password on next login.",
                Data = true
            };
        }
        catch (Exception ex)
        {
            return new Result<bool>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public Result<bool> Delete(int userId)
    {
        try
        {
            var user = _db.TblUsers
                .FirstOrDefault(x => !x.IsDeleted && x.UserId == userId);

            if (user == null)
            {
                return new Result<bool>
                {
                    IsSuccess = false,
                    Message = "User not found."
                };
            }

            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;
            _db.SaveChanges();

            return new Result<bool>
            {
                IsSuccess = true,
                Message = "User deleted successfully.",
                Data = true
            };
        }
        catch (Exception ex)
        {
            return new Result<bool>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }
}
