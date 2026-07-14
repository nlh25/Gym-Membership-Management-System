using FluentValidation;
using FluentValidation.AspNetCore;
using GMMS.Database.AppDbContextModels;
using GMMS.Domain.Features.Member;
using GMMS.Domain.Features.Member.Models;
using GMMS.Domain.Features.MemberShip;
using GMMS.Domain.Features.MemberShipPlan;
using GMMS.Domain.Features.Payment;
using GMMS.Domain.Features.PaymentMethod;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"));
});

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateMemberRequestValidator>();

builder.Services.AddScoped<MemberService>();
builder.Services.AddScoped<MemberShipPlanService>();
builder.Services.AddScoped<MemberShipService>();
builder.Services.AddScoped<PaymentMethodService>();
builder.Services.AddScoped<PaymentService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
