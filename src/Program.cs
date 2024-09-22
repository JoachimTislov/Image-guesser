using Image_guesser.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Image_guesser.Core.Domain.UserContext;
using MediatR;
using Image_guesser.Core.Domain.OracleContext.Services;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.ImageContext.Services;
using Image_guesser.Core.Domain.SignalRContext.Services;
using Image_guesser.Core.Domain.SignalRContext.Hubs;
using Image_guesser.SharedKernel;
using Image_guesser.Core.Domain.GameContext.Services;
using Image_guesser.Core.Domain.OracleContext.Repositories;
using Image_guesser.Core.Domain.ImageContext.Repositories;
using Image_guesser.Infrastructure.GenericRepository;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddMediatR(typeof(Program));

// Add DI to builders
services.AddTransient<IOracleService, OracleService>();
services.AddTransient<IGameService, GameService>();
services.AddTransient<ISessionService, SessionService>();
services.AddTransient<IAI_Repository, AI_Repository>();

services.AddTransient<IImageRepository, ImageRepository>();
services.AddTransient<IImageService, ImageService>();

services.AddScoped<IRepository, Repository>();

services.AddSingleton<IConnectionMappingService, ConnectionMappingService>();

// Configure DbContext with SQLite
services.AddDbContext<ImageGameContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity with the User entity
services.AddDefaultIdentity<User>(IdentityOptionsConfiguration.ConfigureIdentityOptions)
    .AddEntityFrameworkStores<ImageGameContext>();

// Add Razor Pages and SignalR
services.AddRazorPages();
services.AddSignalR();

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