
using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.UserContext;

public record UserNavigatedToPage(string PageUrl, string UserId) : BaseDomainEvent;