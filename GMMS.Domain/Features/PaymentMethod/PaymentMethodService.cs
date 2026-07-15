 using FluentValidation;
using GMMS.Database.AppDbContextModels;
using GMMS.Domain.Features.PaymentMethod.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMMS.Domain.Features.PaymentMethod
{
    public class PaymentMethodService
    {
        private readonly AppDbContext _db;
        private readonly IValidator<PaymentMethodListRequestModel> _listValidator;
        private readonly IValidator<PaymentMethodCreateRequestModel> _createValidator;
        private readonly IValidator<PaymentMethodUpdateRequestModel> _updateValidator;

        public PaymentMethodService(
            AppDbContext db,
            IValidator<PaymentMethodListRequestModel> listValidator,
            IValidator<PaymentMethodCreateRequestModel> createValidator,
            IValidator<PaymentMethodUpdateRequestModel> updateValidator)
        {
            _db = db;
            _listValidator = listValidator;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public Result<PaymentMethodListResponseModel> GetList(PaymentMethodListRequestModel request)
        {
            var validationResult = _listValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new Result<PaymentMethodListResponseModel>
                {
                    IsSuccess = false,
                    Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
                };
            }

            try
            {
                var query = _db.TblPaymentMethods
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted);

                var totalCount = query.Count();
                var paymentMethods = query

                    .OrderByDescending(x => x.PaymentMethodId)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => new PaymentMethodModel
                    {
                        PaymentMethodId = x.PaymentMethodId,
                        PaymentMethodCode = x.PaymentMethodCode,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt
                    })
                    .ToList();

                return new Result<PaymentMethodListResponseModel>
                {
                    IsSuccess = true,
                    Message = "Payment methods retrieved successfully.",
                    Data = new PaymentMethodListResponseModel
                    {
                        TotalCount = totalCount,
                        PaymentMethods = paymentMethods
                    }
                };
            }
            catch (Exception ex)
            {
                return new Result<PaymentMethodListResponseModel>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public Result<PaymentMethodModel> GetById(int paymentMethodId)
        {
            try
            {
                var paymentMethod = _db.TblPaymentMethods
                    .AsNoTracking()
                    .Where(x => x.PaymentMethodId == paymentMethodId && !x.IsDeleted)
                    .Select(x => new PaymentMethodModel
                    {
                        PaymentMethodId = x.PaymentMethodId,
                        PaymentMethodCode = x.PaymentMethodCode,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt
                    })
                    .FirstOrDefault();

                if (paymentMethod == null)
                {
                    return new Result<PaymentMethodModel>
                    {
                        IsSuccess = false,
                        Message = "Payment method not found."
                    };
                }

                return new Result<PaymentMethodModel>
                {
                    IsSuccess = true,
                    Message = "Payment method retrieved successfully.",
                    Data = paymentMethod
                };
            }
            catch (Exception ex)
            {
                return new Result<PaymentMethodModel>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public Result<PaymentMethodModel> Create(PaymentMethodCreateRequestModel request)
        {
            var validationResult = _createValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new Result<PaymentMethodModel>
                {
                    IsSuccess = false,
                    Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
                };
            }

            try
            {
                var exists = _db.TblPaymentMethods
                    .Any(x => !x.IsDeleted && x.PaymentMethodCode.ToUpper() == request.PaymentMethodCode.ToUpperInvariant());
                if (exists)
                {
                    return new Result<PaymentMethodModel>
                    {
                        IsSuccess = false,
                        Message = "Payment method code already exists."
                    };
                }
                var paymentMethod = new TblPaymentMethod
                {
                    PaymentMethodCode = request.PaymentMethodCode.ToUpperInvariant(),
                    Name = request.Name.Trim(),
                    IsActive = request.IsActive,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };
                _db.TblPaymentMethods.Add(paymentMethod);
                _db.SaveChanges();
                return new Result<PaymentMethodModel>
                {
                    IsSuccess = true,
                    Message = "Payment method created successfully.",
                    Data = new PaymentMethodModel
                    {
                        PaymentMethodId = paymentMethod.PaymentMethodId,
                        PaymentMethodCode = paymentMethod.PaymentMethodCode,
                        Name = paymentMethod.Name,
                        IsActive = paymentMethod.IsActive,
                        CreatedAt = paymentMethod.CreatedAt,
                        UpdatedAt = paymentMethod.UpdatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new Result<PaymentMethodModel>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public Result<PaymentMethodModel> Update(int id, PaymentMethodUpdateRequestModel request)
        {
            var validationResult = _updateValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new Result<PaymentMethodModel>
                {
                    IsSuccess = false,
                    Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
                };
            }

            try
            {
                var paymentMethod = _db.TblPaymentMethods
                    .Where(x => x.PaymentMethodId == request.PaymentMethodId && !x.IsDeleted)
                    .FirstOrDefault();
                if (paymentMethod == null)
                {
                    return new Result<PaymentMethodModel>
                    {
                        IsSuccess = false,
                        Message = "Payment method not found."
                    };
                }
                var exists = _db.TblPaymentMethods
                    .Any(x => !x.IsDeleted && x.PaymentMethodCode.ToUpper() == request.PaymentMethodCode.ToUpperInvariant() && x.PaymentMethodId != request.PaymentMethodId);
                if (exists)
                {
                    return new Result<PaymentMethodModel>
                    {
                        IsSuccess = false,
                        Message = "Payment method code already exists."
                    };
                }

                paymentMethod.PaymentMethodCode = request.PaymentMethodCode.ToUpperInvariant();
                paymentMethod.Name = request.Name.Trim();
                paymentMethod.IsActive = request.IsActive;
                paymentMethod.UpdatedAt = DateTime.UtcNow;
                _db.SaveChanges();
                return new Result<PaymentMethodModel>
                {
                    IsSuccess = true,
                    Message = "Payment method updated successfully.",
                    Data = new PaymentMethodModel
                    {
                        PaymentMethodId = paymentMethod.PaymentMethodId,
                        PaymentMethodCode = paymentMethod.PaymentMethodCode,
                        Name = paymentMethod.Name,
                        IsActive = paymentMethod.IsActive,
                        CreatedAt = paymentMethod.CreatedAt,
                        UpdatedAt = paymentMethod.UpdatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new Result<PaymentMethodModel>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public Result<bool> Delete(int paymentMethodId)
        {
            try
            {
                var paymentMethod = _db.TblPaymentMethods
                    .Where(x => x.PaymentMethodId == paymentMethodId && !x.IsDeleted)
                    .FirstOrDefault();
                if (paymentMethod == null)
                {
                    return new Result<bool>
                    {
                        IsSuccess = false,
                        Message = "Payment method not found."
                    };
                }
                paymentMethod.IsDeleted = true;
                paymentMethod.UpdatedAt = DateTime.UtcNow;
                _db.SaveChanges();
                return new Result<bool>
                {
                    IsSuccess = true,
                    Message = "Payment method deleted successfully.",
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