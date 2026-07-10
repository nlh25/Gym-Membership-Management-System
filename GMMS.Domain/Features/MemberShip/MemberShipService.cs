using GMMS.Database.AppDbContextModels;
using GMMS.Domain.Enums;
using GMMS.Domain.Features.MemberShip.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMMS.Domain.Features.MemberShip
{
    public class MemberShipService
    {
        private readonly AppDbContext _db;
        public MemberShipService(AppDbContext db)
        {
            _db = db;
        }
        public Result<MemberShipListResponseModel> GetList(MemberShipListRequestModel request)
        {
            try
            {
                if (request.PageNumber <= 0)
                    request.PageNumber = 1;

                if (request.PageSize <= 0)
                    request.PageSize = 10;
                var query = _db.TblMemberships
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted);

                if (request.MemberId.HasValue)
                    query = query.Where(x => x.MemberId == request.MemberId.Value);

                var totalCount = query.Count();

                var memberships = query
                    .OrderByDescending(x => x.MembershipId)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => new MemberShipModel
                    {
                        MembershipId = x.MembershipId,
                        MemberCode = x.Member.MemberCode,
                        MemberName = x.Member.Name,
                        PlanCode = x.MembershipPlan.PlanCode,
                        PlanName = x.MembershipPlan.PlanName,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        Status = x.Status
                    })
                    .ToList();
                return new Result<MemberShipListResponseModel>
                {
                    IsSuccess = true,
                    Message = "Memberships retrieved successfully.",
                    Data = new MemberShipListResponseModel
                    {
                        TotalCount = totalCount,
                        MemberShips = memberships
                    }
                };
            }
            catch (Exception ex)
            {
                return new Result<MemberShipListResponseModel>
                {
                    IsSuccess = false,
                    Message = ex.Message

                };
            }
        }
        public Result<MembershipDetailModel> GetById(int membershipId)
        {
            try
            {
                var membership = _db.TblMemberships
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted && x.MembershipId == membershipId)
                    .Select(x => new MembershipDetailModel
                    {
                        MembershipId = x.MembershipId,
                        MemberId = x.MemberId,
                        MemberCode = x.Member.MemberCode,
                        MemberName = x.Member.Name,
                        MembershipPlanId = x.MembershipPlanId,
                        PlanCode = x.MembershipPlan.PlanCode,
                        PlanName = x.MembershipPlan.PlanName,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        Status = x.Status,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt
                    })
                    .FirstOrDefault();
                if (membership == null)
                {
                    return new Result<MembershipDetailModel>
                    {
                        IsSuccess = false,
                        Message = "Membership not found."
                    };
                }
                return new Result<MembershipDetailModel>
                {
                    IsSuccess = true,
                    Message = "Membership detail retrieved successfully.",
                    Data = membership
                };
            }
            catch (Exception ex)
            {
                return new Result<MembershipDetailModel>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public Result<MembershipDetailModel> Create(CreateMemberShipRequestModel request)
        {
            try
            {

                var member = _db.TblMembers
                    .FirstOrDefault(x => !x.IsDeleted && x.MemberId == request.MemberId);

                if (member == null)
                {
                    return new Result<MembershipDetailModel>
                    {
                        IsSuccess = false,
                        Message = "Member not found."
                    };
                }



                var plan = _db.TblMembershipPlans
                    .FirstOrDefault(x =>
                        !x.IsDeleted &&
                        x.MembershipPlanId == request.MembershipPlanId);

                if (plan == null)
                {
                    return new Result<MembershipDetailModel>
                    {
                        IsSuccess = false,
                        Message = "Membership plan not found."
                    };
                }



                var exists = _db.TblMemberships
                    .Any(x =>
                        !x.IsDeleted &&
                        x.MemberId == request.MemberId &&
                        x.Status == MembershipPlanStatus.Active.ToString());

                if (exists)
                {
                    return new Result<MembershipDetailModel>
                    {
                        IsSuccess = false,
                        Message = "Member already has an active membership."
                    };
                }

                var paymentMethod = _db.TblPaymentMethods
                    .FirstOrDefault(x => !x.IsDeleted && x.PaymentMethodId == request.PaymentMethodId);

                if (paymentMethod == null)
                {
                    return new Result<MembershipDetailModel>
                    {
                        IsSuccess = false,
                        Message = "Payment method not found."
                    };
                }

                var today = DateOnly.FromDateTime(DateTime.UtcNow);

                var newMembership = new TblMembership
                {
                    MemberId = request.MemberId,
                    MembershipPlanId = request.MembershipPlanId,

                    StartDate = today,
                    EndDate = today.AddDays(plan.DurationDays),

                    Status = MembershipPlanStatus.Active.ToString(),

                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                _db.TblMemberships.Add(newMembership);
                _db.SaveChanges();

                var newPayment = new TblPayment
                {
                    MembershipId = newMembership.MembershipId,
                    PaymentMethodId = request.PaymentMethodId,
                    Amount = request.Amount,
                    Sspath = request.Sspath,
                    Status = PaymentStatus.Pending.ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                _db.TblPayments.Add(newPayment);
                _db.SaveChanges();

                return new Result<MembershipDetailModel>
                {
                    IsSuccess = true,
                    Message = "Membership and payment created successfully.",

                    Data = new MembershipDetailModel
                    {
                        MembershipId = newMembership.MembershipId,

                        MemberId = member.MemberId,
                        MemberCode = member.MemberCode,
                        MemberName = member.Name,

                        MembershipPlanId = plan.MembershipPlanId,
                        PlanCode = plan.PlanCode,
                        PlanName = plan.PlanName,

                        StartDate = newMembership.StartDate,
                        EndDate = newMembership.EndDate,

                        Status = newMembership.Status,

                        CreatedAt = newMembership.CreatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new Result<MembershipDetailModel>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public Result<MembershipDetailModel> Update(UpdateMembershipRequestModel request)
        {
            try
            {
                var membership = _db.TblMemberships
                    .FirstOrDefault(x => !x.IsDeleted && x.MembershipId == request.MembershipId);
                if (membership == null)
                {
                    return new Result<MembershipDetailModel>
                    {
                        IsSuccess = false,
                        Message = "Membership not found."
                    };
                }
                var plan = _db.TblMembershipPlans
                    .FirstOrDefault(x =>
                        !x.IsDeleted &&
                        x.MembershipPlanId == request.MembershipPlanId);
                if (plan == null)
                {
                    return new Result<MembershipDetailModel>
                    {
                        IsSuccess = false,
                        Message = "Membership plan not found."
                    };
                }
                var exits = _db.TblMemberships
                    .Any(x =>
                        !x.IsDeleted &&
                        x.MemberId == membership.MemberId &&
                        x.MembershipId != membership.MembershipId &&
                        x.Status == MembershipPlanStatus.Active.ToString());
                if (exits)
                {
                    return new Result<MembershipDetailModel>
                    {
                        IsSuccess = false,
                        Message = "Member already has an active membership."
                    };
                }
                membership.MembershipPlanId = request.MembershipPlanId;
                membership.EndDate = membership.StartDate.AddDays(plan.DurationDays);
                membership.UpdatedAt = DateTime.UtcNow;
                _db.SaveChanges();
                return new Result<MembershipDetailModel>
                {
                    IsSuccess = true,
                    Message = "Membership updated successfully.",
                    Data = new MembershipDetailModel
                    {
                        MembershipId = membership.MembershipId,
                        MemberId = membership.MemberId,
                        MemberCode = membership.Member.MemberCode,
                        MemberName = membership.Member.Name,
                        MembershipPlanId = plan.MembershipPlanId,
                        PlanCode = plan.PlanCode,
                        PlanName = plan.PlanName,
                        StartDate = membership.StartDate,
                        EndDate = membership.EndDate,
                        Status = membership.Status,
                        CreatedAt = membership.CreatedAt,
                        UpdatedAt = membership.UpdatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new Result<MembershipDetailModel>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public Result<bool> Delete(int membershipId)
        {
            try
            {
                var membership = _db.TblMemberships
                    .FirstOrDefault(x => !x.IsDeleted && x.MembershipId == membershipId);
                if (membership == null)
                {
                    return new Result<bool>
                    {
                        IsSuccess = false,
                        Message = "Membership not found."
                    };
                }
                membership.IsDeleted = true;
                membership.UpdatedAt = DateTime.UtcNow;
                _db.SaveChanges();
                return new Result<bool>
                {
                    IsSuccess = true,
                    Message = "Membership deleted successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new Result<bool>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = false
                };
            }
        }
    }
}
