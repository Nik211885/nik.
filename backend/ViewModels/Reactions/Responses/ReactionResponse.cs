using backend.Entities;

namespace backend.ViewModels.Reactions.Responses;

/// <summary>Reaction response returned by list and create endpoints.</summary>
public class ReactionResponse
{
    /// <summary>Reaction ID.</summary>
    public string Id { get; set; }

    /// <summary>ID of the user who left the reaction.</summary>
    public string CreatedByUserId { get; set; }

    /// <summary>UTC timestamp when the reaction was recorded.</summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>Type of reaction (Heart or Like).</summary>
    public ReactionType ReactionType { get; set; }

    /// <summary>ID of the article that was reacted to.</summary>
    public string ArticleId { get; set; }
}

/// <summary>EF Core-translatable projection extensions for <see cref="Reaction"/>.</summary>
public static class ReactionResponseExtensions
{
    extension(Reaction reaction)
    {
        /// <summary>Maps a single <see cref="Reaction"/> entity to <see cref="ReactionResponse"/>.</summary>
        public ReactionResponse ToReactionResponse()
        {
            return new ReactionResponse
            {
                Id = reaction.Id,
                CreatedByUserId = reaction.CreatedByUserId,
                CreatedDate = reaction.CreatedDate,
                ReactionType = reaction.ReactionType,
                ArticleId = reaction.ArticleId
            };
        }
    }

    extension(IQueryable<Reaction> reactions)
    {
        /// <summary>
        /// Projects an <see cref="IQueryable{Reaction}"/> to <see cref="ReactionResponse"/> objects.
        /// </summary>
        public IQueryable<ReactionResponse> ToReactionResponses()
        {
            return reactions.Select(reaction => reaction.ToReactionResponse());
        }
    }
}
