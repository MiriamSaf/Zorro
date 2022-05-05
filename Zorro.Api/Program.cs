using Microsoft.EntityFrameworkCore;
using Zorro.Dal;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add entity framework
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/ProcessPayment", async (TransactionRequest transactionRequest) =>
{
    await Task.Delay(100);

    Console.WriteLine($"Processed eCommerce payment request of {transactionRequest.Amount:c} for UserID {transactionRequest.UserID}");
    
    return new TransactionResponse();
});

app.Run();

class TransactionRequest
{
    public string CustomerWalletId { get; set; } = "";
    public decimal Amount { get; set; }
    public string PaymentDescription { get; set; } = "";
}

class TransactionResponse
{
    public string ReceiptNumber => "123456";
}