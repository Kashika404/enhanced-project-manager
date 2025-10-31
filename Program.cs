var builder = WebApplication.CreateBuilder(args);

// --- Add services to the container ---

// This line is CRITICAL. It tells .NET to find and register
// all of our classes that are [ApiControllers] (like our SchedulingController).
builder.Services.AddControllers(); 

// This adds the Swagger/OpenAPI (the interactive test page)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- Configure the HTTP request pipeline ---
if (app.Environment.IsDevelopment())
{
    // These two lines enable the /swagger test page
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// This line is also CRITICAL. It tells .NET to use the
// controller routes we defined (e.g., /api/v1/...).
app.MapControllers();

app.Run();