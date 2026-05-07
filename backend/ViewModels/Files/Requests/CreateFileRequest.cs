using FluentValidation;
using EFile = backend.Entities.File;

namespace backend.ViewModels.Files.Requests;

public class CreateFileRequest
{
    public string Name { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
}

public class CreateFileRequestValidator : AbstractValidator<CreateFileRequest>
{
    public CreateFileRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(ApplicationMessage.NameIsRequired);
        RuleFor(x => x.Title).NotEmpty().WithMessage(ApplicationMessage.TitleIsRequired);
        RuleFor(x => x.Url).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().WithMessage(ApplicationMessage.DescriptionIsRequired);
    }
}

public static class CreateFileRequestExtensions
{
    extension(CreateFileRequest request)
    {
        public EFile ToFile()
        {
            return new EFile
            {
                Name = request.Name,
                Title = request.Title,
                Url = request.Url,
                Description = request.Description
            };
        }
    }
}
