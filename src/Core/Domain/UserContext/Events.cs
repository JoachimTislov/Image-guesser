
using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.UserContext;

public record UserNavigatedToPage(string PathName, string UserId) : BaseDomainEvent;