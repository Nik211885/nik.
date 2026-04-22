using FluentValidation;

namespace backend.ViewModels.Category.Requests;
public class CreateCategoryRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }
}

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(ApplicationMessage.NameIsRequired);
        RuleFor(x => x.Description).NotEmpty().WithMessage(ApplicationMessage.DescriptionIsRequired);
        RuleFor(x => x.Title).NotEmpty().WithMessage(ApplicationMessage.TitleIsRequired);
        RuleFor(x => x.Image).NotEmpty().WithMessage(ApplicationMessage.ImageIsRequired);
    }
}

public static class CreateCategoryRequestExtension
{
    extension(CreateCategoryRequest request)
    {
        public backend.Entities.Category ToCategoryEntity()
        {
            return new backend.Entities.Category
            {
                Name = request.Name,
                Description = request.Description,
                Title = request.Title,
                Image = request.Image,
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedDate = DateTimeOffset.UtcNow
            };
        }
    }
}
