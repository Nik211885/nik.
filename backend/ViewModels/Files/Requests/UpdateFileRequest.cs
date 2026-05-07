using FluentValidation;
using EFile = backend.Entities.File;

namespace backend.ViewModels.Files.Requests;

public class UpdateFileRequest
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}

public class UpdateFileRequestValidator : AbstractValidator<UpdateFileRequest>
{
    public UpdateFileRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().WithMessage(ApplicationMessage.NameIsRequired);
        RuleFor(x => x.Title).NotEmpty().WithMessage(ApplicationMessage.TitleIsRequired);
        RuleFor(x => x.Description).NotEmpty().WithMessage(ApplicationMessage.DescriptionIsRequired);
    }
}

public static class UpdateFileRequestExtensions
{
    extension(UpdateFileRequest request)
    {
        public EFile ApplyTo(EFile file)
        {
            file.Name = request.Name;
            file.Title = request.Title;
            file.Description = request.Description;
            return file;
        }
    }
}
