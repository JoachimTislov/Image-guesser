namespace Image_guesser.SharedKernel;

public interface IValidator<T>
{
	(bool IsValid, string Error) IsValid(T item);
}
