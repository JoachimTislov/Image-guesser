using Image_guesser.Core.Domain.ImageContext.Services;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Infrastructure;
using MediatR;

namespace Image_guesser.Core.Domain.SessionContext.Pipelines;

public class EditSession
{
    public record Request(
        Guid Id,
        int NumberOfRounds,
        int LobbySize,
        GameMode GameMode,
        bool RandomPictureMode,
        bool RandomUserOracle,
        bool UseAI,
        string ImageIdentifier,
        User NewOracle,
        string ChosenImageName) : IRequest<Session?>;

    public class Handler(IImageService imageService, ImageGameContext db, IMediator mediator) : IRequestHandler<Request, Session?>
    {
        private readonly IImageService _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
        private readonly ImageGameContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        public async Task<Session?> Handle(Request request, CancellationToken cancellationToken)
        {
            var session = await _mediator.Send(new GetSessionById.Request(request.Id), cancellationToken);

            if (session != null)
            {

                // Temporarily store the old gameMode so that in case of a mismatch between player count and lobby size we can restore the old gameMode
                var OldGameMode = session.Options.GameMode;

                // Updating GameMode
                session.Options.GameMode = request.GameMode;

                // Switch to ensure that the count of players is in correlation with the gameMode selected
                // ----- GameMode Logic -----
                switch (request.GameMode)
                {
                    case GameMode.SinglePlayer:
                        if (session.SessionUsers.Count > 1)
                        {
                            session.Options.GameMode = OldGameMode;
                            break;
                        }
                        session.Options.LobbySize = 1;
                        break;
                    case GameMode.Duo:
                        if (session.SessionUsers.Count > 2)
                        {
                            session.Options.GameMode = OldGameMode;
                            break;
                        }
                        session.Options.LobbySize = 2;
                        session.ChosenOracleId = session.SessionHostId;
                        break;
                    case GameMode.FreeForAll:
                        if (session.SessionUsers.Count > request.LobbySize)
                        {
                            session.Options.LobbySize = session.SessionUsers.Count;
                            break;
                        }
                        session.Options.LobbySize = request.LobbySize;
                        session.ChosenOracleId = session.SessionHostId;
                        break;
                }

                // ----- Options -----
                session.Options.NumberOfRounds = request.NumberOfRounds;
                session.Options.RandomPictureMode = request.RandomPictureMode;
                session.Options.RandomUserOracle = request.RandomUserOracle;
                session.Options.UseAI = request.UseAI;

                // ----- Oracle Logic -----
                if (request.NewOracle != null)
                {
                    session.ChosenOracleId = request.NewOracle.Id;
                }

                // ----- Image Logic -----
                session.ImageIdentifier = !request.RandomPictureMode ? request.ImageIdentifier : string.Empty;

                if (session.ImageIdentifier != null)
                {
                    var ImageRecord = await _imageService.GetImageRecordById(session.ImageIdentifier);
                    session.ChosenImageName = ImageRecord.Name;
                }

                _db.Update(session);
                await _db.SaveChangesAsync(cancellationToken);
            }

            return session;
        }
    }
}