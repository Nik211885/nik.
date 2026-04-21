using FluentValidation;

namespace backend.ViewModels.Tag.Requests;
public class CreateTagRequest
{
    public string Name{ get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
}

public static class CreateTagRequestExtensions
{
    extension(CreateTagRequest request)
    {
        public backend.Entities.Tag ToTag()
        {
            return new backend.Entities.Tag
            {
                Name = request.Name,
                Title = request.Title,
                Description = request.Description,
                Image = request.Image
            };
        }
    }
}
public class CreateTagRequestValidator : AbstractValidator<CreateTagRequest>
{
    public CreateTagRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(ApplicationMessage.NameIsRequired);
        RuleFor(x => x.Title).NotEmpty().WithMessage(ApplicationMessage.TitleIsRequired);
        RuleFor(x => x.Description).NotEmpty().WithMessage(ApplicationMessage.DescriptionIsRequired);
        RuleFor(x => x.Image).NotEmpty().WithMessage(ApplicationMessage.ImageIsRequired);
    }
}
