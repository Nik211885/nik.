using backend.Entities;
using FluentValidation;

namespace backend.ViewModels.Albums.Requests;
public class CreateAlbumRequest
{
    public string Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string FileDescriptionId { get; set; }
    public int OrderIndex { get; set; }
    public string? AlbumId { get; set; }
}

public class CreateAlbumRequestValidator : AbstractValidator<CreateAlbumRequest>
{
    public CreateAlbumRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.FileDescriptionId).NotEmpty();
        RuleFor(x => x.OrderIndex).GreaterThanOrEqualTo(0);
    }
}

public static class CreateAlbumRequestExtensions
{
    extension(CreateAlbumRequest request)
    {
        public Album ToAlbum()
        {
            return new Album
            {
                Name = request.Name,
                Title = request.Title,
                Description = request.Description,
                FileDescriptionId = request.FileDescriptionId,
                OrderIndex = request.OrderIndex,
                AlbumId = request.AlbumId
            };
        }
    }
}

