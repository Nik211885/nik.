using backend.Entities;

namespace backend.ViewModels.Tag.Responses;
public class TagResponse
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public string Slug { get; set; }
    public int CountRef { get; set; }
    public DateTimeOffset CreatedDate{ get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
}

public static class TagExtensions
{
    extension(IQueryable<backend.Entities.Tag> tag)
    {
        public IQueryable<TagResponse> ToTagResponse()
        {
            return tag.Select(x => new TagResponse
            {
                Id = x.Id,
                Name = x.Name,
                Title = x.Title,
                Description = x.Description,
                Image = x.Image,
                Slug = x.Slug,
                CountRef = x.CountRef,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate
            });
        }
    }
    extension(backend.Entities.Tag tag)
    {
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
