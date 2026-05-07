using backend.Entities;

namespace backend.ViewModels.Tag.Responses;

/// <summary>Tag response returned by all tag endpoints.</summary>
public class TagResponse
{
    /// <summary>Tag ID.</summary>
    public string Id { get; set; }

    /// <summary>Lowercase unique name.</summary>
    public string Name { get; set; }

    /// <summary>Human-readable display title.</summary>
    public string Title { get; set; }

    /// <summary>Short description.</summary>
    public string Description { get; set; }

    /// <summary>Cover image URL.</summary>
    public string Image { get; set; }

    /// <summary>URL-friendly slug.</summary>
    public string Slug { get; set; }

    /// <summary>Denormalised count of articles tagged with this tag.</summary>
    public int CountRef { get; set; }

    /// <summary>UTC creation timestamp.</summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>UTC last-modification timestamp.</summary>
    public DateTimeOffset UpdatedDate { get; set; }
}

/// <summary>EF Core-translatable projection extensions for <see cref="Tag"/>.</summary>
public static class TagExtensions
{
    extension(IQueryable<backend.Entities.Tag> tag)
    {
        /// <summary>Projects an <see cref="IQueryable{Tag}"/> to <see cref="TagResponse"/> objects.</summary>
        public IQueryable<TagResponse> ToTagResponse()
        {
            return tag.Select(t => t.ToTagResponse());
        }
    }

    extension(backend.Entities.Tag tag)
    {
        /// <summary>Maps a single <see cref="Tag"/> entity to <see cref="TagResponse"/>.</summary>
        public TagResponse ToTagResponse()
        {
            return new TagResponse
            {
                Id = tag.Id,
                Name = tag.Name,
                Title = tag.Title,
                Description = tag.Description,
                Image = tag.Image,
                Slug = tag.Slug,
                CountRef = tag.CountRef,
                CreatedDate = tag.CreatedDate,
                UpdatedDate = tag.UpdatedDate
            };
        }
    }
}
