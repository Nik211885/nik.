using backend.Entities;
using FluentValidation;

namespace backend.ViewModels.Albums.Requests;

public class UpdateAlbumRequest
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string FileDescriptionId { get; set; }
    public int OrderIndex { get; set; }
    public string? AlbumId { get; set; }
}

public class UpdateAlbumRequestValidator : AbstractValidator<UpdateAlbumRequest>
{
    public UpdateAlbumRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().WithMessage(ApplicationMessage.NameIsRequired);
        RuleFor(x => x.Title).NotEmpty().WithMessage(ApplicationMessage.TitleIsRequired);
        RuleFor(x => x.Description).NotEmpty().WithMessage(ApplicationMessage.DescriptionIsRequired);
        RuleFor(x => x.FileDescriptionId).NotEmpty();
        RuleFor(x => x.OrderIndex).GreaterThanOrEqualTo(0);
    }
}

public static class UpdateAlbumRequestExtensions
{
    extension(UpdateAlbumRequest request)
    {
        public Album ApplyTo(Album album)
        {
            album.Name = request.Name;
            album.Title = request.Title;
            album.Description = request.Description;
            album.FileDescriptionId = request.FileDescriptionId;
            album.OrderIndex = request.OrderIndex;
            album.AlbumId = request.AlbumId;
            return album;
        }
    }
}
