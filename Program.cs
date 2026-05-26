using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniForm.Data;
using MiniForm.Options;
using MiniForm.Services;
using MiniForm.Services.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<MiniForm.Application.Interfaces.IUserService, MiniForm.Infrastructure.Services.UserService>();
builder.Services.AddScoped<MiniForm.Application.Interfaces.IFormService, MiniForm.Infrastructure.Services.FormService>();
builder.Services.AddScoped<MiniForm.Application.Interfaces.IUserRepository, MiniForm.Infrastructure.Repositories.EfUserRepository>();
builder.Services.AddScoped<MiniForm.Application.Interfaces.IFormRepository, MiniForm.Infrastructure.Repositories.EfFormRepository>();
builder.Services.AddScoped<MiniForm.Application.Interfaces.IUnitOfWork, MiniForm.Infrastructure.UnitOfWork.EfUnitOfWork>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT configuration is missing.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
