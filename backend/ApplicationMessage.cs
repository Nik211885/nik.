namespace backend;

public static class ApplicationMessage
{
    public static readonly string NotFound = "exception.not_found";
    public static readonly string BadRequest = "exception.bad_request"; 
    public static readonly string ExitsCode = "exception.exits_code";

    public static readonly string NameIsRequired = "validation.name_is_required";
    public static readonly string TitleIsRequired = "validation.title_is_required";
    public static readonly string DescriptionIsRequired = "validation.description_is_required";
    public static readonly string ImageIsRequired = "validation.image_is_required";

    public static readonly string ReactionTypeIsInvalid = "validation.reaction_type_is_invalid";
}
