using backend.Entities;
using FluentValidation;

namespace backend.ViewModels.Tag.Requests;

public class UpdateTagRequest
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
}

public abstract class UpdateTagValidator : AbstractValidator<UpdateTagRequest>
{
    public UpdateTagValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(ApplicationMessage.NameIsRequired);
        RuleFor(x => x.Title).NotEmpty().WithMessage(ApplicationMessage.TitleIsRequired);
        RuleFor(x => x.Description).NotEmpty().WithMessage(ApplicationMessage.DescriptionIsRequired);
        RuleFor(x => x.Image).NotEmpty().WithMessage(ApplicationMessage.ImageIsRequired);
    }
}

public static class UpdateTagExtensions
{
    extension(UpdateTagRequest model)
    {
        public backend.Entities.Tag ToTag(backend.Entities.Tag tag)
        {
            tag.Name = model.Name;
            tag.Title = model.Title;
            tag.Description = model.Description;
            tag.Image = model.Image;
            return tag;
        }
    }
}