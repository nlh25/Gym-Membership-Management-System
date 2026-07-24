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

    public async Task< Result<UserListResponseModel>> GetList(UserListRequestModel request)
    {
        var validationResult = await _listValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return new Result<UserListResponseModel>
            {
                IsSuccess = false,
                Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
            };
        }

        
            var query = _db.TblUsers
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.Trim().ToLower();
                query = query.Where(x => x.UserName.ToLower().Contains(search) || x.Role.ToLower().Contains(search));
            }

            var totalCount = await  query.CountAsync();

            var users = await query
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
                    CreatedByUser = x.CreatedBy + " - " + _db.TblUsers
                    .Where(u => u.UserId == x.CreatedBy)
                    .Select(u => u.UserName)
                    .FirstOrDefault(),
                    UpdatedAt = x.UpdatedAt,
                    UpdatedByUser = x.UpdatedBy.HasValue
                        ? x.UpdatedBy.Value + " - " + _db.TblUsers
                        .Where(u => u.UserId == x.UpdatedBy.Value)
                        .Select(u => u.UserName)
                        .FirstOrDefault()
                        : null
                })
                .ToListAsync();

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

    public async Task <Result<UserModel>> GetById(int userId)
    {
       
            var user = await _db.TblUsers
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
                .FirstOrDefaultAsync();

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

    public async Task<Result<UserModel>> Create(CreateUserRequestModel request)
    {
        var validationResult =await  _createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return new Result<UserModel>
            {
                IsSuccess = false,
                Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
            };
        }

       
            request.UserName = request.UserName
                .Trim()
                .ToLowerInvariant();

            var exists = await _db.TblUsers
                .AnyAsync(x => !x.IsDeleted && x.UserName == request.UserName);

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

            await _db.TblUsers.AddAsync(user);
            await _db.SaveChangesAsync();

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

    public async Task<Result<UserModel>> Update(int id, UpdateUserRequestModel request)
    {
        var validationResult = await _updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return new Result<UserModel>
            {
                IsSuccess = false,
                Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
            };
        }

       
            var user =await _db.TblUsers
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.UserId == request.UserId);

            if (user == null)
            {
                return new Result<UserModel>
                {
                    IsSuccess = false,
                    Message = "User not found."
                };
            }

            request.UserName = request.UserName.Trim().ToLowerInvariant();

            var exists = await _db.TblUsers
                .AnyAsync(x => !x.IsDeleted && x.UserName == request.UserName && x.UserId != request.UserId);

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
            await _db.SaveChangesAsync();

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

    public async Task <Result<bool> >ResetPassword(ResetPasswordRequestModel request)
    {
        var validationResult =await _resetPasswordValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return new Result<bool>
            {
                IsSuccess = false,
                Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
            };
        }

        
            var user = await _db.TblUsers
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.UserId == request.UserId);

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
            await _db.SaveChangesAsync();

            return new Result<bool>
            {
                IsSuccess = true,
                Message = "Password reset successfully. User must change password on next login.",
                Data = true
            };
       
    }

    public async Task <Result<bool>> Delete(int userId)
    {
        
            var user = await _db.TblUsers
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.UserId == userId);

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
            await _db.SaveChangesAsync();

            return new Result<bool>
            {
                IsSuccess = true,
                Message = "User deleted successfully.",
                Data = true
            };
        
       
    }
}
