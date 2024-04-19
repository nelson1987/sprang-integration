using System.Text.Json;

namespace Sprang.Basics.Core.Extensions;

public static class FluentValidationExtensions
{
    public static bool IsInvalid(this ValidationResult result) => !result.IsValid;

    public static ModelStateDictionary ToModelState(this ValidationResult result)
    {
        var modelState = new ModelStateDictionary();

        foreach (var error in result.Errors)
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
        return modelState;
    }

    public static ModelStateDictionary ToModelState(this ValidationFailure result)
    {
        var modelState = new ModelStateDictionary();
        modelState.AddModelError(result.PropertyName, result.ErrorMessage);

        return modelState;
    }
    /*
     public IReadOnlyList<ValidationResult> Validate(object validatingObject)
       {
           var validationErrors = new List<ValidationResult>();

           IValidator fvValidator = _validatorFactory.GetValidator(validatingObject.GetType());

           if (fvValidator != null)
           {
               var validationContext = new ValidationContext<object>(validatingObject);
               var validationResult = fvValidator.Validate(validationContext);

               var mappedValidationErrors = validationResult.Errors
                   .Select(e => new ValidationResult(e.ErrorMessage, new[] { e.PropertyName }))
                   .ToList();

               validationErrors.AddRange(mappedValidationErrors);
           }

           return validationErrors;
       }
     */
}

public static class FluentResultsExtensions
{
    public record Error(string Message, IDictionary<string, object>? Metadata);
    public record ErrorResponse(Error[] Errors);

    public static Task<Result> ToFailResult(this FluentValidation.Results.ValidationResult validationResult)
    {
        var errors = validationResult.Errors.Select(x => new FluentResults.Error(x.ErrorMessage)
            .WithMetadata(nameof(x.PropertyName), x.PropertyName)
            .WithMetadata(nameof(x.AttemptedValue), x.AttemptedValue));

        return Task.FromResult(Result.Fail(errors));
    }
    public static string[] GetErrorsMessage(this ResultBase result) => 
        result.Errors.Select(x => x.Message).ToArray();
    public static ErrorResponse ToErrorResponse(this ResultBase result, bool skipMetadata = false) =>
        new(result.Errors.Select(x => new Error(x.Message, skipMetadata ? null : x.Metadata)).ToArray());
}

public static class AutoMapperExtensions
{
    private static readonly Lazy<IMapper> Lazy = new(() =>
    {
        var config = new MapperConfiguration(cfg => cfg.AddMaps(typeof(AutoMapperExtensions).Assembly));
        return config.CreateMapper();
    });

    public static IMapper Mapper => Lazy.Value;

    public static T MapTo<T>(this object source) => Mapper.Map<T>(source);
}

public static class Serializer
{
    public static string Serialize(this object obj)
    {
        return JsonSerializer.Serialize(obj);
    }

    public static T Deserialize<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json);
    }
}