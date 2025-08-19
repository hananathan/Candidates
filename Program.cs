using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Candidates API",
        Version = "v1"
    });
});

builder.Services.AddDbContext<CandidatesDbContext>(options =>
// Ensure the required using directive is added at the top of the file  
    options.UseSqlite("Data Source=candidates.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ensure DB is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CandidatesDbContext>();
    db.Database.EnsureCreated();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
