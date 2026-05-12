using backend.Entities;

namespace backend.ViewModels.Users.Responses;

/// <summary>Public user profile returned by user endpoints.</summary>
public class UserResponse
{
    /// <summary>User ID.</summary>
    public string Id { get; set; }

    /// <summary>Display name.</summary>
    public string UserName { get; set; }

    /// <summary>Email address.</summary>
    public string? Email { get; set; }

    /// <summary>Phone number.</summary>
    public string? Phone { get; set; }

    /// <summary>Short biography.</summary>
    public string Bio { get; set; }

    /// <summary>URL-friendly slug for the public profile page.</summary>
    public string Slug { get; set; }

    /// <summary>UTC account creation timestamp.</summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>UTC last-modification timestamp.</summary>
    public DateTimeOffset UpdatedDate { get; set; }
}

/// <summary>Mapping extensions for <see cref="User"/> → <see cref="UserResponse"/>.</summary>
public static class UserResponseExtensions
{
    extension(User user)
    {
        /// <summary>Maps a <see cref="User"/> entity to a <see cref="UserResponse"/>.</summary>
        public UserResponse ToUserResponse()
        {
            return new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                Bio = user.Bio,
                Slug = user.Slug,
                CreatedDate = user.CreatedDate,
                UpdatedDate = user.UpdatedDate
            };
        }
    }

    extension(IQueryable<User> users)
    {
        /// <summary>Projects an <see cref="IQueryable{User}"/> to <see cref="UserResponse"/> objects.</summary>
        public IQueryable<UserResponse> ToUserResponses()
        {
            return users.Select(u => new UserResponse
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Phone = u.Phone,
                Bio = u.Bio,
                Slug = u.Slug,
                CreatedDate = u.CreatedDate,
                UpdatedDate = u.UpdatedDate
            });
        }
    }
}
