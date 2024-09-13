using Image_guesser.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Image_guesser.Core.Domain.UserContext;
using MediatR;
using Image_guesser.Core.Domain.OracleContext.Services;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.ImageContext.Services;
using Image_guesser.Core.Domain.SignalRContext.Services;
using Image_guesser.Core.Domain.OracleContext.Pipelines;
using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.SignalRContext.Hubs;

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
// ***********************

builder.Services.AddDbContext<ImageGameContext>(
    options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ImageGameContext>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 0;

    // SignIn settings
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._@";
    options.User.RequireUniqueEmail = false;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapHub<GameHub>("/gameHub");

app.Run();
