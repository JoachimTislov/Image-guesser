using MediatR;

namespace Image_guesser.SharedKernel;

public abstract record BaseDomainEvent : INotification
{
	public DateTimeOffset DateOccurred { get; protected set; } = DateTimeOffset.UtcNow;
}
