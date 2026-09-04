using FluentValidation.Results;

namespace BestStories.Service.Models;

public sealed record ServiceResult<T>(T? Value, ValidationResult ValidationResult)
{
    public static ServiceResult<T> Success(T value) => new(value, new ValidationResult());

    public static ServiceResult<T> Failure(ValidationResult validationResult) => new(default, validationResult);
}
