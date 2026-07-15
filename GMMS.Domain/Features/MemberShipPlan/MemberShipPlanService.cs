using FluentValidation;
using GMMS.Database.AppDbContextModels;
using GMMS.Domain.Features.MemberShipPlan.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMMS.Domain.Features.MemberShipPlan
{
    public class MemberShipPlanService
    {
        private readonly AppDbContext _db;
        private readonly IValidator<MemberShipPlanlistRequestModel> _listValidator;
        private readonly IValidator<CreateMemberShipPlanRequestModel> _createValidator;
        private readonly IValidator<UpdateMemberShipPlanRequestModel> _updateValidator;

        public MemberShipPlanService(
            AppDbContext db,
            IValidator<MemberShipPlanlistRequestModel> listValidator,
            IValidator<CreateMemberShipPlanRequestModel> createValidator,
            IValidator<UpdateMemberShipPlanRequestModel> updateValidator)
        {
            _db = db;
            _listValidator = listValidator;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public Result<MemberShipPlanListResponseModel> GetList(MemberShipPlanlistRequestModel request)
        {
            var validationResult = _listValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new Result<MemberShipPlanListResponseModel>
                {
                    IsSuccess = false,
                    Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
                };
            }

            try
            {
                var query = _db.TblMembershipPlans
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted);

                var totalCount = query.Count();

                var plans = query
                    .OrderByDescending(x => x.MembershipPlanId)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => new MemberShipPlanModel
                    {
                        MemberShipPlanId = x.MembershipPlanId,
                        PlanCode = x.PlanCode,
                        PlanName = x.PlanName,
                        Price = x.Price,
                        DurationDays = x.DurationDays,
                        IsActive = x.IsActive,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                    })
                    .ToList();

                return new Result<MemberShipPlanListResponseModel>
                {
                    IsSuccess = true,
                    Message = "Membership plans retrieved successfully.",
                    Data = new MemberShipPlanListResponseModel
                    {
                        TotalCount = totalCount,
                        MemberShipPlans = plans
                    }
                };
            }
            catch (Exception ex)
            {
                return new Result<MemberShipPlanListResponseModel>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public Result<MembershipPlanDetailModel> GetById(int membershipPlanId)
        {
            try
            {
                var plan = _db.TblMembershipPlans
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted && x.MembershipPlanId == membershipPlanId)
                    .Select(x => new MembershipPlanDetailModel
                    {
                        MemberShipPlanId = x.MembershipPlanId,
                        PlanCode = x.PlanCode,
                        PlanName = x.PlanName,
                        Price = x.Price,
                        DurationDays = x.DurationDays,
                        Description = x.Description,
                        IsActive = x.IsActive,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt
                    })
                    .FirstOrDefault();
                if (plan == null)
                {
                    return new Result<MembershipPlanDetailModel>
                    {
                        IsSuccess = false,
                        Message = "Membership plan not found."
                    };
                }
                return new Result<MembershipPlanDetailModel>
                {
                    IsSuccess = true,
                    Message = "Membership plan retrieved successfully.",
                    Data = plan
                };
            }
            catch (Exception ex)
            {
                return new Result<MembershipPlanDetailModel>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public Result<MembershipPlanDetailModel> Create(CreateMemberShipPlanRequestModel request)
        {
            var validationResult = _createValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new Result<MembershipPlanDetailModel>
                {
                    IsSuccess = false,
                    Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
                };
            }

            try
            {
                var exists = _db.TblMembershipPlans
                    .Any(x => !x.IsDeleted && x.PlanCode.ToUpper() == request.PlanCode.ToUpperInvariant());

                if (exists)
                {
                    return new Result<MembershipPlanDetailModel>
                    {
                        IsSuccess = false,
                        Message = "Membership plan already exists."
                    };
                }

                var plan = new TblMembershipPlan
                {
                    PlanCode = request.PlanCode.ToUpperInvariant(),
                    PlanName = request.PlanName.Trim(),
                    Price = request.Price,
                    DurationDays = request.DurationDays,
                    Description = request.Description?.Trim(),
                    IsActive = request.IsActive,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                _db.TblMembershipPlans.Add(plan);
                _db.SaveChanges();

                return new Result<MembershipPlanDetailModel>
                {
                    IsSuccess = true,
                    Message = "Membership plan created successfully.",
                    Data = new MembershipPlanDetailModel
                    {
                        MemberShipPlanId = plan.MembershipPlanId,
                        PlanCode = request.PlanCode,
                        PlanName = plan.PlanName,
                        Price = plan.Price,
                        DurationDays = plan.DurationDays,
                        Description = plan.Description,
                        CreatedAt = plan.CreatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new Result<MembershipPlanDetailModel>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public Result<MembershipPlanDetailModel> Update(int id, UpdateMemberShipPlanRequestModel request)
        {
            var validationResult = _updateValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new Result<MembershipPlanDetailModel>
                {
                    IsSuccess = false,
                    Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
                };
            }

            try
            {
                var plan = _db.TblMembershipPlans
                    .Where(x => !x.IsDeleted && x.MembershipPlanId == request.MemberShipPlanId)
                    .FirstOrDefault();
                if (plan == null)
                {
                    return new Result<MembershipPlanDetailModel>
                    {
                        IsSuccess = false,
                        Message = "Membership plan not found."
                    };
                }

                var existsplan = _db.TblMembershipPlans
                    .Any(x => !x.IsDeleted && x.PlanCode.ToUpper() == request.PlanCode.ToUpperInvariant() && x.MembershipPlanId != request.MemberShipPlanId);
                if (existsplan)
                {
                    return new Result<MembershipPlanDetailModel>
                    {
                        IsSuccess = false,
                        Message = "Membership plan already exists."
                    };
                }

                plan.PlanCode = request.PlanCode.ToUpperInvariant();
                plan.PlanName = request.PlanName.Trim();
                plan.Price = request.Price;
                plan.DurationDays = request.DurationDays;
                plan.Description = request.Description?.Trim();
                plan.IsActive = request.IsActive;
                plan.UpdatedAt = DateTime.UtcNow;
                _db.SaveChanges();
                return new Result<MembershipPlanDetailModel>
                {
                    IsSuccess = true,
                    Message = "Membership plan updated successfully.",
                    Data = new MembershipPlanDetailModel
                    {
                        MemberShipPlanId = plan.MembershipPlanId,
                        PlanCode = plan.PlanCode,
                        PlanName = plan.PlanName,
                        Price = plan.Price,
                        DurationDays = plan.DurationDays,
                        Description = plan.Description,
                        IsActive = plan.IsActive,
                        CreatedAt = plan.CreatedAt,
                        UpdatedAt = plan.UpdatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new Result<MembershipPlanDetailModel>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public Result<bool> Delete(int membershipPlanId)
        {
            try
            {
                var plan = _db.TblMembershipPlans
                    .Where(x => !x.IsDeleted && x.MembershipPlanId == membershipPlanId)
                    .FirstOrDefault();
                if (plan == null)
                {
                    return new Result<bool>
                    {
                        IsSuccess = false,
                        Message = "Membership plan not found."
                    };
                }
                plan.IsDeleted = true;
                plan.UpdatedAt = DateTime.UtcNow;
                _db.SaveChanges();
                return new Result<bool>
                {
                    IsSuccess = true,
                    Message = "Membership plan deleted successfully.",
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