using GMMS.Database.AppDbContextModels;
using GMMS.Domain.Features.Member.Models;
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

        public MemberShipPlanService(AppDbContext db)
        {
            _db = db;
        }
        public Result<MemberShipPlanListResponseModel> GetList(MemberShipPlanlistRequestModel request)
        {
            try
            {
                if (request.PageNumber <= 0)
                    request.PageNumber = 1;

                if (request.PageSize <= 0)
                    request.PageSize = 10;

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
            try
            {
                var exists = _db.TblMembershipPlans
                    .Any(x => !x.IsDeleted && x.PlanCode == request.PlanCode);

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
                    PlanCode = request.PlanCode,
                    PlanName = request.PlanName,
                    Price = request.Price,
                    DurationDays = request.DurationDays,
                    Description = request.Description,
                    IsActive = true,
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
                    .Any(x => !x.IsDeleted && x.PlanCode == request.PlanCode && x.MembershipPlanId != request.MemberShipPlanId);
                if (existsplan)
                {
                    return new Result<MembershipPlanDetailModel>
                    {
                        IsSuccess = false,
                        Message = "Membership plan already exists."
                    };
                }

                plan.PlanCode = request.PlanCode;
                plan.PlanName = request.PlanName;
                plan.Price = request.Price;
                plan.DurationDays = request.DurationDays;
                plan.Description = request.Description;
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
