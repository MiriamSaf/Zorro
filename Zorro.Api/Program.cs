var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    Console.WriteLine($"Received eCommerce payment request of {transactionRequest.Amount:c} for UserID {transactionRequest.UserID}");

    return new TransactionResponse();
});

app.Run();

class TransactionRequest
{ 
    public Guid UserID { get; set; }
    public decimal Amount { get; set; }
}

class TransactionResponse
{
    public string ReceiptNumber => "123456";
}