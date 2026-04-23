using backend.Entities;

namespace backend.ViewModels.Category.Responses;
public class CategoryResponse
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Image { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public int CountRef { get; set; }
}
public static class CategoryResponseExtension
{
    extension(IQueryable<backend.Entities.Category> query)
    {
        public IQueryable<CategoryResponse> ToCategoryResponse()
        {
            return query.Select(x => x.ToCategoryResponse());
        }
    }
    extension(backend.Entities.Category entity)
    {
        public CategoryResponse ToCategoryResponse()
        {
            return new CategoryResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                Title = entity.Title,
                Slug = entity.Slug,
                Image = entity.Image,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CountRef = entity.CountRef
            };
        }
    }
}
