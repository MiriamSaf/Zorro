using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zorro.Api.Exceptions;
using Zorro.Api.Services;
using Zorro.Dal;
using Zorro.Dal.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add entity framework
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddAuthentication("ApiKeyAuthentication")
                .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>
                ("ApiKeyAuthentication", null);
builder.Services.AddAuthorization();

builder.Services.AddSingleton<IBusinessBankerService, BusinessBankerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
//app.UseMiddleware<ApiKeyMiddleware>();
app.UseHttpsRedirection();

app.MapPost("/ProcessPayment", [Authorize] async ([FromServices] IBusinessBankerService banker, TransactionRequest transactionRequest, HttpRequest req) =>
{
    try
    {
        if (req.HttpContext.User.Identity is null)
            return Results.Unauthorized();
        var merchantWallet = req.HttpContext.User.Identity.Name;

        var receipt = await banker.ProcessPayment(
        merchantWallet,
        transactionRequest.CustomerWalletId,
        transactionRequest.Amount,
        transactionRequest.Currency,
        transactionRequest.Description);

        return Results.Ok(new Receipt() { ReceiptNumber = receipt.ToString() });
    }
    catch (AbstractZorroException ex)
    {
        return Results.Problem(ex.Message);
    }
    catch (Exception)
    {
        return Results.Problem("An unexpected error occurred. Contact the Zorro service desk for support.");
    }

});

app.Run();

class TransactionRequest
{
    public string CustomerWalletId { get; set; } = "";
    public decimal Amount { get; set; }
    public string Description { get; set; } = "";
    public Currency Currency { get; set; } = Currency.Aud;
}

class Receipt
{
    public string ReceiptNumber { get; set; }
}