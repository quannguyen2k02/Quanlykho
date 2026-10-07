
using Infrastructure.ExternalServices.Mapper;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);



// ============================================================
// 3. CORS
// ============================================================
var myAllowSpecificOrigins = "AllowAllWithCredentials";
builder.Services.AddCors(options =>
{
    options.AddPolicy(myAllowSpecificOrigins, policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ============================================================
// 4. Controllers, Swagger, SignalR
// ============================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAutoMapper(typeof(ApplicationMapper));

var app = builder.Build();

// ============================================================
// 6. Swagger (chỉ cấu hình 1 lần, không lặp lại)
// ============================================================
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Led Model API V1");
    c.RoutePrefix = "swagger";
});

// ============================================================
// 7. Middleware pipeline
// ============================================================
app.UseHttpsRedirection();
app.UseCors(myAllowSpecificOrigins);
app.UseAuthorization();

app.MapControllers();

app.Run();