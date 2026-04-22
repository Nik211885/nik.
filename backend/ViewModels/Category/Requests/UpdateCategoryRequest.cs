using backend.Entities;
using FluentValidation;

namespace backend.ViewModels.Category.Requests;
public class UpdateCategoryRequest
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }
}



public static class UpdateCategoryRequestExtension
{
    extension(UpdateCategoryRequest request)
    {
        public backend.Entities.Category ToCategoryEntity(backend.Entities.Category category)
        {
            category.Name = request.Name;
            category.Description = request.Description;
            category.Title = request.Title;
            category.Image = request.Image;
            category.CreatedDate = DateTimeOffset.UtcNow;
            category.UpdatedDate = DateTimeOffset.UtcNow;
            return category;
        }
    }
}


public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(ApplicationMessage.NameIsRequired);
        RuleFor(x => x.Description).NotEmpty().WithMessage(ApplicationMessage.DescriptionIsRequired);
        RuleFor(x => x.Title).NotEmpty().WithMessage(ApplicationMessage.TitleIsRequired);
        RuleFor(x => x.Image).NotEmpty().WithMessage(ApplicationMessage.ImageIsRequired);
    }
}
