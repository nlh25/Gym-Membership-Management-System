using FluentValidation;
using GMMS.Database.AppDbContextModels;
using GMMS.Domain.Features.Member.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMMS.Domain.Features.Member
{
    public class MemberService
    {
        private readonly AppDbContext _db;
        private readonly IValidator<CreateMemberRequestModel> _createValidator;
        private readonly IValidator<UpdateMemberRequestModel> _updateValidator;
        private readonly IValidator<MemberListRequestModel> _listValidator;

        public MemberService(
            AppDbContext db,
            IValidator<CreateMemberRequestModel> createValidator,
            IValidator<UpdateMemberRequestModel> updateValidator,
            IValidator<MemberListRequestModel> listValidator)
        {
            _db = db;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _listValidator = listValidator;
        }
        
        public Result<MemberListResponseModel> GetList(MemberListRequestModel request)
        {
            var validationResult = _listValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new Result<MemberListResponseModel>
                {
                    IsSuccess = false,
                    Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
                };
            }

            try
            {
                if (request.PageNumber <= 0)
                    request.PageNumber = 1;

                if (request.PageSize <= 0 || request.PageSize > 100)
                    request.PageSize = 10;

                var query = _db.TblMembers
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted);

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var search = request.SearchTerm.Trim().ToLower();
                    query = query.Where(x => x.MemberCode.ToLower().Contains(search) || x.Name.ToLower().Contains(search));
                }

                var totalCount = query.Count();

                var members = query
                    .OrderByDescending(x => x.MemberId)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => new MemberModel
                    {
                        MemberId = x.MemberId,
                        MemberCode = x.MemberCode,
                        Name = x.Name,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt
                    })
                    .ToList();

                return new Result<MemberListResponseModel>
                {
                    IsSuccess = true,
                    Message = "Members retrieved successfully.",
                    Data = new MemberListResponseModel
                    {
                        TotalCount = totalCount,
                        Members = members
                    }
                };
            }
            catch (Exception ex)
            {
                return new Result<MemberListResponseModel>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public Result<MemberModel> GetById(int memberId)
        {
            try
            {
                var member = _db.TblMembers
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted && x.MemberId == memberId)
                    .Select(x => new MemberModel
                    {
                        MemberId = x.MemberId,
                        MemberCode = x.MemberCode,
                        Name = x.Name,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt
                    })
                    .FirstOrDefault();

                if (member == null)
                {
                    return new Result<MemberModel>
                    {
                        IsSuccess = false,
                        Message = "Member not found."
                    };
                }

                return new Result<MemberModel>
                {
                    IsSuccess = true,
                    Message = "Member retrieved successfully.",
                    Data = member
                };
            }
            catch (Exception ex)
            {
                return new Result<MemberModel>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public Result<MemberModel> Create(CreateMemberRequestModel request)
        {
            var validationResult = _createValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new Result<MemberModel>
                {
                    IsSuccess = false,
                    Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
                };
            }

            try
            {
                request.MemberCode = request.MemberCode.Trim().ToUpperInvariant();
                request.Name = request.Name.Trim();

                var exists = _db.TblMembers
                    .Any(x => !x.IsDeleted && x.MemberCode.ToUpper() == request.MemberCode);

                if (exists)
                {
                    return new Result<MemberModel>
                    {
                        IsSuccess = false,
                        Message = "Member already exists."
                    };
                }
                var member = new TblMember
                {
                    MemberCode = request.MemberCode,
                    Name = request.Name,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };


                _db.TblMembers.Add(member);
                _db.SaveChanges();

                return new Result<MemberModel>
                {
                    IsSuccess = true,
                    Message = "Member created successfully.",
                    Data = new MemberModel
                    {
                        MemberId = member.MemberId,
                        MemberCode = member.MemberCode,
                        Name = member.Name,
                        CreatedAt = member.CreatedAt,
                        UpdatedAt = member.UpdatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new Result<MemberModel>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public Result<MemberModel> Update(int id, UpdateMemberRequestModel request)
        {
            var validationResult = _updateValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new Result<MemberModel>
                {
                    IsSuccess = false,
                    Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
                };
            }

            try
            {
                var member = _db.TblMembers
                    .Where(x => !x.IsDeleted && x.MemberId == request.MemberId)
                    .FirstOrDefault();
                if (member == null)
                {
                    return new Result<MemberModel>
                    {
                        IsSuccess = false,
                        Message = "Member not found."
                    };
                }

                request.MemberCode = request.MemberCode.Trim().ToUpperInvariant();
                request.Name = request.Name.Trim();

                var exists = _db.TblMembers
                    .Any(x => !x.IsDeleted && x.MemberCode.ToUpper() == request.MemberCode && x.MemberId != request.MemberId);
                if (exists)
                {
                    return new Result<MemberModel>
                    {
                        IsSuccess = false,
                        Message = "Member already exists."
                    };
                }

                member.MemberCode = request.MemberCode;
                member.Name = request.Name;
                member.UpdatedAt = DateTime.UtcNow;
             
                _db.SaveChanges();

                return new Result<MemberModel>
                {
                    IsSuccess = true,
                    Message = "Member updated successfully.",
                    Data = new MemberModel
                    {
                        MemberId = member.MemberId,
                        MemberCode = member.MemberCode,
                        Name = member.Name,
                        CreatedAt = member.CreatedAt,
                        UpdatedAt = member.UpdatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new Result<MemberModel>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }

        }
        public Result<bool> Delete(int memberId)
        {
            try
            {
                var member = _db.TblMembers
                    .Where(x => !x.IsDeleted && x.MemberId == memberId)
                    .FirstOrDefault();
                if (member == null)
                {
                    return new Result<bool>
                    {
                        IsSuccess = false,
                        Message = "Member not found."
                    };
                }
                member.IsDeleted = true;
                member.UpdatedAt = DateTime.UtcNow;
                _db.SaveChanges();
                return new Result<bool>
                {
                    IsSuccess = true,
                    Message = "Member deleted successfully.",
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
}