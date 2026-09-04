using BestStories.Service.Models;
using FluentValidation;

namespace BestStories.Service.Validation;

public sealed class BestStoriesRequestValidator : AbstractValidator<BestStoriesRequest>
{
    public BestStoriesRequestValidator()
    {
        RuleFor(request => request.Count)
            .InclusiveBetween(1, 200)
            .OverridePropertyName("n")
            .WithMessage("n must be between 1 and 200.");
    }
}
