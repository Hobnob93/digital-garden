using OneOf;
using OneOf.Types;

namespace DigitalGarden.Shared.Models.Client;

public class ApiClientResult<T>
{
    public required OneOf<T, NotFound, Error<string>> Result { get; set; }

    public T AsExpectedType => Result.AsT0;
    public NotFound AsNotFound => Result.AsT1;
    public Error<string> AsError => Result.AsT2;

    public bool IsExpectedType => Result.IsT0;
    public bool IsNotFound => Result.IsT1;
    public bool IsError => Result.IsT2;

    public static implicit operator ApiClientResult<T>(T result)
    {
        return new ApiClientResult<T> { Result = result };
    }

    public static implicit operator ApiClientResult<T>(NotFound result)
    {
        return new ApiClientResult<T> { Result = result };
    }

    public static implicit operator ApiClientResult<T>(Error<string> result)
    {
        return new ApiClientResult<T> { Result = result };
    }
}
