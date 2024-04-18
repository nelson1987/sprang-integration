using FluentResults;
using FluentValidation.Results;

namespace Sprang.Tests.Api.Extensions;

public static class FluentResultsExtensions
{
    public static Result ToFailResult(this ValidationResult validationResult)
    {
        var errors = validationResult.Errors.Select(x => new FluentResults.Error(x.ErrorMessage)
            .WithMetadata(nameof(x.PropertyName), x.PropertyName)
            .WithMetadata(nameof(x.AttemptedValue), x.AttemptedValue));

        return Result.Fail(errors);
    }

    public static string[] GetErrorsMessage(this ResultBase result)
    {
        return result.Errors.Select(x => x.Message).ToArray();
    }

    public static ErrorResponse ToErrorResponse(this ResultBase result, bool skipMetadata = false)
    {
        return new ErrorResponse(result.Errors.Select(x => new Error(x.Message, skipMetadata ? null : x.Metadata))
            .ToArray());
    }

    public record Error(string Message, IDictionary<string, object>? Metadata);

    public record ErrorResponse(Error[] Errors);
}