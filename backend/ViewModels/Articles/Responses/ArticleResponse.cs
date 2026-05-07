using backend.Entities;

namespace backend.ViewModels.Articles.Responses;

public class ArticleTagItem
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Image { get; set; }
}

public class ArticleCategoryItem
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Image { get; set; }
}

public class ArticleAuthorItem
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Slug { get; set; }
}

public class ArticleResponse
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public string Image { get; set; }
    public string Slug { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public int CountCommentRef { get; set; }
    public int CountLikeRef { get; set; }
    public int CountSee { get; set; }
    public int CountHeartRef { get; set; }
    public ArticleAuthorItem Author { get; set; }
    public List<ArticleTagItem> Tags { get; set; }
    public List<ArticleCategoryItem> Categories { get; set; }
}

public static class ArticleResponseExtensions
{
    extension(IQueryable<Article> articles)
    {
        public IQueryable<ArticleResponse> ToArticleResponses()
        {
            return articles.Select(a => new ArticleResponse
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                Content = a.Content,
                Image = a.Image,
                Slug = a.Slug,
                CreatedDate = a.CreatedDate,
                UpdatedDate = a.UpdatedDate,
                CountCommentRef = a.CountCommentRef,
                CountLikeRef = a.CountLikeRef,
                CountSee = a.CountSee,
                CountHeartRef = a.CountHeartRef,
                Author = new ArticleAuthorItem
                {
                    Id = a.Author.Id,
                    UserName = a.Author.UserName,
                    Slug = a.Author.Slug
                },
                Tags = a.ArticleTags.Select(at => new ArticleTagItem
                {
                    Id = at.TagId,
                    Name = at.Tag.Name,
                    Slug = at.Tag.Slug,
                    Image = at.Tag.Image
                }).ToList(),
                Categories = a.ArticleCategories.Select(ac => new ArticleCategoryItem
                {
                    Id = ac.CategoryId,
                    Name = ac.Category.Name,
                    Slug = ac.Category.Slug,
                    Image = ac.Category.Image
                }).ToList()
            });
        }
    }
}
