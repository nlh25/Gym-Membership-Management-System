using FluentValidation;
using GMMS.Database.AppDbContextModels;
using GMMS.Domain.Features.Payment.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMMS.Domain.Features.Payment
{
    public class PaymentService
    {
        private readonly AppDbContext _db;
        private readonly IValidator<PaymentListRequestModel> _listValidator;
        private readonly IValidator<CreatePaymentRequestModel> _createValidator;

        public PaymentService(
            AppDbContext db,
            IValidator<PaymentListRequestModel> listValidator,
            IValidator<CreatePaymentRequestModel> createValidator)
        {
            _db = db;
            _listValidator = listValidator;
            _createValidator = createValidator;
        }

        public Result<PaymentListResponseModel> GetList(PaymentListRequestModel request)
        {
            var validationResult = _listValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new Result<PaymentListResponseModel>
                {
                    IsSuccess = false,
                    Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
                };
            }

            try
            {
                var query = _db.TblPayments
                    .AsNoTracking();


                var totalCount = query.Count();


                var payments = query
                    .OrderByDescending(x => x.PaymentId)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => new PaymentModel
                    {
                        PaymentId = x.PaymentId,

                        MembershipId = x.MembershipId,

                        PaymentMethodName = x.PaymentMethod.Name,

                        Amount = x.Amount,

                        Sspath = x.Sspath,

                        Status = x.Status,

                        CreatedAt = x.CreatedAt
                    })
                    .ToList();


                return new Result<PaymentListResponseModel>
                {
                    IsSuccess = true,
                    Message = "Payments retrieved successfully.",
                    Data = new PaymentListResponseModel
                    {
                        TotalCount = totalCount,
                        Payments = payments
                    }
                };
            }
            catch (Exception ex)
            {
                return new Result<PaymentListResponseModel>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public Result<PaymentDetailModel> GetById(int paymentId)
        {
            try
            {
                var payment = _db.TblPayments
                    .AsNoTracking()
                    .Where(x => x.PaymentId == paymentId)
                    .Select(x => new PaymentDetailModel
                    {
                        PaymentId = x.PaymentId,
                        MemberName = x.Membership.Member.Name,
                        PaymentMethodName = x.PaymentMethod.Name,
                        Amount = x.Amount,
                        Status = x.Status,
                        CreatedAt = x.CreatedAt
                    })
                    .FirstOrDefault();
                if (payment == null)
                {
                    return new Result<PaymentDetailModel>
                    {
                        IsSuccess = false,
                        Message = "Payment not found."
                    };
                }
                return new Result<PaymentDetailModel>
                {
                    IsSuccess = true,
                    Message = "Payment retrieved successfully.",
                    Data = payment
                };
            }
            catch (Exception ex)
            {
                return new Result<PaymentDetailModel>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public Result<PaymentModel> Create(CreatePaymentRequestModel request)
        {
            var validationResult = _createValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new Result<PaymentModel>
                {
                    IsSuccess = false,
                    Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
                };
            }

            try
            {
                var payment = _db.TblPayments
                    .FirstOrDefault(x => x.MembershipId == request.MembershipId && x.Status == "Pending");
                if (payment != null)
                {
                    return new Result<PaymentModel>
                    {
                        IsSuccess = false,
                        Message = "A pending payment already exists for this membership."
                    };
                }

                var PaymentMethod = _db.TblPaymentMethods
                    .FirstOrDefault(x => x.PaymentMethodId == request.PaymentMethodId && !x.IsDeleted && x.IsActive);
                     
                if (PaymentMethod == null)
                {
                    return new Result<PaymentModel>
                    {
                        IsSuccess = false,
                        Message = "Payment method not found or inactive."
                    };
                }
                
                var membership = _db.TblMemberships
                    .FirstOrDefault(x => x.MembershipId == request.MembershipId && !x.IsDeleted);
                if (membership == null)
                {
                    return new Result<PaymentModel>
                    {
                        IsSuccess = false,
                        Message = "Membership not found."
                    };
                }


                var newPayment = new TblPayment
                {
                    MembershipId = request.MembershipId,
                    PaymentMethodId = request.PaymentMethodId,
                    Amount = request.Amount,
                    Sspath = request.Sspath,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                _db.TblPayments.Add(newPayment);
                _db.SaveChanges();
                return new Result<PaymentModel>
                {
                    IsSuccess = true,
                    Message = "Payment created successfully."
                };
            }
            catch (Exception ex)
            {
                return new Result<PaymentModel>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}