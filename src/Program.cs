using Image_guesser.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Image_guesser.Core.Domain.UserContext;
using MediatR;
using Image_guesser.Core.Domain.OracleContext.Services;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.ImageContext.Services;
using Image_guesser.Core.Domain.SignalRContext.Hubs;
using Image_guesser.SharedKernel;
using Image_guesser.Core.Domain.GameContext.Services;
using Image_guesser.Infrastructure.GenericRepository;
using Image_guesser.Core.Domain.OracleContext.AI_Repository;
using Image_guesser.Core.Domain.ImageContext.Repository;
using Image_guesser.Core.Domain.UserContext.Services;
using Image_guesser.Core.Domain.ImageContext;
using Image_guesser.Core.Domain.SessionContext.Repository;
using Image_guesser.Core.Domain.SignalRContext.Services.ConnectionMapping;
using Image_guesser.Core.Domain.SignalRContext.Services.Hub;
using Image_guesser.Core.Domain.LeaderboardContext.Repository;
using Image_guesser.Core.Domain.LeaderboardContext.Services;
using Image_guesser.Core.Domain.LeaderboardContext;
using Image_guesser.Core.Domain.GameContext;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddDistributedMemoryCache();

services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

services.AddHttpContextAccessor();

// Add DI to builders
services.AddScoped<IAI_Repository, AI_Repository>();
services.AddScoped<IOracleService, OracleService>();

services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();
services.AddScoped<ILeaderboardService, LeaderboardService>();

services.AddScoped<IGameService, GameService>();

services.AddScoped<ISessionRepository, SessionRepository>();
services.AddScoped<ISessionService, SessionService>();

services.AddScoped<IUserService, UserService>();

services.AddScoped<IImageRepository, ImageRepository>();
services.AddScoped<IImageService, ImageService>();

services.AddScoped<IRepository, Repository>();

services.AddScoped<IHubService, HubService>();
services.AddSingleton<IConnectionMappingService, ConnectionMappingService>();

services.AddMediatR(typeof(Program));

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

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var DbRepository = scope.ServiceProvider.GetRequiredService<IRepository>();
    var _imageRepository = scope.ServiceProvider.GetRequiredService<IImageRepository>();
    var _leaderboardService = scope.ServiceProvider.GetRequiredService<ILeaderboardService>();

    if (!DbRepository.Any<ImageRecord>())
    {
        await _imageRepository.AddAllMappedImagesToDatabase();
    }

    /*if (!DbRepository.Any<BaseLeaderboardEntry>() && DbRepository.Any<BaseGame>())
    {
        await _leaderboardService.InitializeLeaderboards();
    }*/
}
else
{
    // Configure the HTTP request pipeline.
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapHub<GameHub>("/gameHub");

app.Run();

public partial class Program { }