using Image_guesser.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Image_guesser.Core.Domain.UserContext;
using MediatR;
using Image_guesser.Core.Domain.OracleContext.Services;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.ImageContext.Services;
using Image_guesser.Core.Domain.SignalRContext.Services;
using Image_guesser.Core.Domain.OracleContext.Pipelines;
using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.SignalRContext.Hubs;
using Image_guesser.SharedKernel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(typeof(Program));

// Add DI to builders
builder.Services.AddTransient<IOracleService, OracleService>();
builder.Services.AddTransient<ISessionService, SessionService>();
builder.Services.AddTransient<IImageService, ImageService>();

builder.Services.AddSingleton<IConnectionMappingService, ConnectionMappingService>();

// Add generic DI to builders
builder.Services.AddTransient<IRequestHandler<AddOracle<User>.Request, Guid>, AddOracle<User>.Handler>();
builder.Services.AddTransient<IRequestHandler<AddOracle<RandomNumbersAI>.Request, Guid>, AddOracle<RandomNumbersAI>.Handler>();

builder.Services.AddTransient(typeof(IRequestHandler<GetOracleById<User>.Request, GenericOracle<User>>), typeof(GetOracleById<User>.Handler));
builder.Services.AddTransient(typeof(IRequestHandler<GetOracleById<RandomNumbersAI>.Request, GenericOracle<RandomNumbersAI>>), typeof(GetOracleById<RandomNumbersAI>.Handler));

// Configure DbContext with SQLite
builder.Services.AddDbContext<ImageGameContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity with the User entity
builder.Services.AddDefaultIdentity<User>(IdentityOptionsConfiguration.ConfigureIdentityOptions)
    .AddEntityFrameworkStores<ImageGameContext>();

// Add Razor Pages and SignalR
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapHub<GameHub>("/gameHub");

app.Run();

public partial class Program { }