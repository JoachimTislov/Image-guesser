using System.Security.Claims;
using Image_guesser.Core.Domain.SessionContext.Repository;
using Image_guesser.Core.Domain.SessionContext.ViewModels;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Core.Domain.UserContext.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Image_guesser.Core.Domain.SessionContext.Services;

public class SessionService(ISessionRepository sessionRepository, IUserService userService) : ISessionService
{
    private readonly ISessionRepository _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));

    public async Task CreateSession(ClaimsPrincipal User, Guid Id)
    {
        var user = await _userService.GetUserByClaimsPrincipal(User);

        Session session = new(user, Id);

        await _sessionRepository.AddSession(session);
    }

    public async Task<List<User>> GetUsersInSessionById(Guid Id)
    {
        return await _sessionRepository.GetUsersInSessionById(Id);
    }

    public async Task<List<SelectListItem>> GetSelectListOfUsersById(Guid Id)
    {
        var session = await GetSessionById(Id);
        var users = await GetUsersInSessionById(Id);

        return users.Select(user => new SelectListItem
        {
            Value = user.Id.ToString(),
            Text = session.UserIsOracle(user.Id) ? "You" : user.UserName,
            Selected = session.UserIsOracle(user.Id)
        }).ToList();
    }

    public async Task<Session> GetSessionById(Guid Id)
    {
        return await _sessionRepository.GetSessionById(Id);
    }

    public async Task<Guid> GetSessionHostIdById(Guid Id)
    {
        return await _sessionRepository.GetSessionHostIdBySessionId(Id);
    }

    public async Task UpdateSession(Session session)
    {
        await _sessionRepository.UpdateSession(session);
    }

    public async Task UpdateSessionOptions(Guid Id, ViewModelOptions options)
    {
        var session = await GetSessionById(Id);

        session.Options.SetOptionsValues(options);

        var canAssignNewUserOracle = options.OracleType == OracleContext.OracleTypes.User;
        if (canAssignNewUserOracle && !string.IsNullOrEmpty(options.SelectedUserId))
        {
            Guid GetRandomUser()
            {
                var users = session.SessionUsers;
                var usersLength = session.SessionUsers.Count;
                var randomIndex = new Random().Next(usersLength);

                return users.ElementAt(randomIndex).Id;
            }

            session.ChosenOracleId = options.UserOracleMode switch
            {
                UserOracleMode.Chosen => Guid.Parse(options.SelectedUserId),
                UserOracleMode.Random => GetRandomUser(),
                _ => session.ChosenOracleId
            };
        }

        await UpdateSession(session);
    }

    public async Task<bool> CheckIfOracleIsAI(Guid sessionId)
    {
        var session = await GetSessionById(sessionId);

        return session.Options.IsOracleAI();
    }

    public async Task<bool> CheckIfUserIsOracle(Guid sessionId, Guid userId)
    {
        var session = await GetSessionById(sessionId);

        return session.UserIsOracle(userId);
    }

    public async Task<bool> CheckIfUserIsSessionHost(Guid sessionId, Guid userId)
    {
        var session = await GetSessionById(sessionId);

        return session.UserIsSessionHost(userId);
    }

    public async Task AddUserToSession(string userId, string sessionId)
    {
        User user = await _userService.GetUserById(userId);
        var session = await GetSessionById(Guid.Parse(sessionId));

        if (!session.AddUser(user))
        {
            // Do something if user is already added ? 
            return;
        }

        await UpdateSessionAndUser(session, user);
    }

    private async Task UpdateSessionAndUser(Session session, User user)
    {
        await UpdateSession(session);
        await _userService.UpdateUser(user);
    }

    private static void UpdateChosenOracleIfUserWasOracle(Session session, Guid userId)
    {
        if (session.ChosenOracleId == userId)
        {
            // Maybe add some more functionality, rather than assigning oracle role to session host

            session.ChosenOracleId = session.SessionHostId;
        }
    }

    public async Task RemoveUserFromSession(string userId, Guid sessionId)
    {
        User user = await _userService.GetUserById(userId);
        var session = await GetSessionById(sessionId);

        UpdateChosenOracleIfUserWasOracle(session, Guid.Parse(userId));

        if (!session.RemoveUser(user))
        {
            // No user to remove...
            return;
        }

        await UpdateSessionAndUser(session, user);
    }

    public async Task DeleteSessionById(Guid Id)
    {
        await _sessionRepository.DeleteSessionById(Id);
    }

    public List<Session> GetAllOpenSessions()
    {
        return _sessionRepository.GetAllOpenSessions();
    }
}
