using backend.Entities;

namespace backend.ViewModels.Reactions.Responses;
public class ReactionResponse
{
    public string Id { get; set; }
    public string CreatedByUserId { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public ReactionType ReactionType{ get; set; }
    public string ArticleId { get; set; }
}

public static class ReactionResponseExtensions
{
    extension(Reaction reaction)
    {
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
        public IQueryable<ReactionResponse> ToReactionResponses()
        {
            return reactions.Select(reaction => reaction.ToReactionResponse());
        }
    }
}

