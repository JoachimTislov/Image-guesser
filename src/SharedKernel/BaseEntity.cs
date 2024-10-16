namespace Image_guesser.SharedKernel;
public abstract class BaseEntity
{
	public List<BaseDomainEvent> Events = [];
	public DateTime CreatedAt { get; private set; } = DateTime.Now;
}
