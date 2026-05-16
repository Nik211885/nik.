using backend.Entities;

namespace backend.ViewModels.Articles.Responses;

/// <summary>Slim tag projection embedded inside <see cref="ArticleResponse"/>.</summary>
public class ArticleTagItem
{
    /// <summary>Tag ID.</summary>
    public string Id { get; set; }

    /// <summary>Lowercase unique tag name.</summary>
    public string Name { get; set; }

    /// <summary>Localised display title.</summary>
    public string Title { get; set; }

    /// <summary>URL-friendly slug for the tag page.</summary>
    public string Slug { get; set; }

    /// <summary>Cover image URL of the tag.</summary>
    public string Image { get; set; }
}

/// <summary>Slim category projection embedded inside <see cref="ArticleResponse"/>.</summary>
public class ArticleCategoryItem
{
    /// <summary>Category ID.</summary>
    public string Id { get; set; }

    /// <summary>Lowercase unique category name.</summary>
    public string Name { get; set; }

    /// <summary>Localised display title.</summary>
    public string Title { get; set; }

    /// <summary>URL-friendly slug for the category page.</summary>
    public string Slug { get; set; }

    /// <summary>Cover image URL of the category.</summary>
    public string Image { get; set; }
}

/// <summary>Slim author projection embedded inside <see cref="ArticleResponse"/>.</summary>
public class ArticleAuthorItem
{
    /// <summary>User ID.</summary>
    public string Id { get; set; }

    /// <summary>Display name of the author.</summary>
    public string UserName { get; set; }

    /// <summary>URL-friendly slug for the author profile page.</summary>
    public string Slug { get; set; }

    /// <summary>Profile picture URL of the author.</summary>
    public string? Avatar { get; set; }

    /// <summary>Short biography of the author.</summary>
    public string Bio { get; set; }
}

/// <summary>
/// Full article response returned by detail, create, update, and list endpoints.
/// Includes nested author, tags, and categories.
/// </summary>
public class ArticleResponse
{
    /// <summary>Article ID.</summary>
    public string Id { get; set; }

    /// <summary>Display title.</summary>
    public string Title { get; set; }

    /// <summary>Short excerpt.</summary>
    public string Description { get; set; }

    /// <summary>Full HTML or Markdown body.</summary>
    public string Content { get; set; }

    /// <summary>Cover image URL.</summary>
    public string Image { get; set; }

    /// <summary>URL-friendly slug.</summary>
    public string Slug { get; set; }

    /// <summary>UTC creation timestamp.</summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>UTC last-modification timestamp.</summary>
    public DateTimeOffset UpdatedDate { get; set; }

    /// <summary>Denormalised comment count.</summary>
    public int CountCommentRef { get; set; }

    /// <summary>Denormalised Like reaction count.</summary>
    public int CountLikeRef { get; set; }

    /// <summary>Cumulative view count.</summary>
    public int CountSee { get; set; }

    /// <summary>Denormalised Heart reaction count.</summary>
    public int CountHeartRef { get; set; }

    /// <summary>Author summary.</summary>
    public ArticleAuthorItem Author { get; set; }

    /// <summary>Tags associated with this article.</summary>
    public List<ArticleTagItem> Tags { get; set; }

    /// <summary>Categories associated with this article.</summary>
    public List<ArticleCategoryItem> Categories { get; set; }
}

/// <summary>EF Core-translatable projection extensions for <see cref="Article"/>.</summary>
public static class ArticleResponseExtensions
{
    extension(IQueryable<Article> articles)
    {
        /// <summary>
        /// Projects an <see cref="IQueryable{Article}"/> to <see cref="ArticleResponse"/> objects.
        /// The projection is fully translatable by EF Core — no client-side evaluation occurs.
        /// </summary>
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
                    Slug = a.Author.Slug,
                    Avatar = a.Author.Avatar,
                    Bio = a.Author.Bio
                },
                Tags = a.ArticleTags.Select(at => new ArticleTagItem
                {
                    Id = at.TagId,
                    Name = at.Tag.Name,
                    Title = at.Tag.Title,
                    Slug = at.Tag.Slug,
                    Image = at.Tag.Image
                }).ToList(),
                Categories = a.ArticleCategories.Select(ac => new ArticleCategoryItem
                {
                    Id = ac.CategoryId,
                    Name = ac.Category.Name,
                    Title = ac.Category.Title,
                    Slug = ac.Category.Slug,
                    Image = ac.Category.Image
                }).ToList()
            });
        }
    }
}
