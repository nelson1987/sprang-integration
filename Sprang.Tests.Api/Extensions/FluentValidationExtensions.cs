using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Sprang.Tests.Api.Extensions;
public static class FluentValidationExtensions
{
    public static bool IsInvalid(this ValidationResult result)
    {
        return !result.IsValid;
    }

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
}