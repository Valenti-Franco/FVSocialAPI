using Microsoft.AspNetCore.Mvc;
using SocialAPI.DTO.Image;
using SocialAPI.DTO.User;
using SocialAPI.Entities;

namespace SocialAPI.Services.Interface
{
    public interface UserInterface
    {

        public Task<ActionResult<UserResponseDTO>> GetUsers(int page, string? name);

        bool SendVerify(string email, string token);

        EUser? VerifyEmail(string token);
        public EUser GetUser(int id);
        public Task<ActionResult<UserResponseAdminDTO>> GetUsersAdmin(int page, string? name);

        EUser? ValidateUser(UserLoginDTO authParams);

        public Task<EUser> RefreshToken(string refreshToken);
        public void AddUser(EUser user);
        public void SaveChange();
        public bool ValidateEmail(string email);
        public bool ValidateUsername(string username);

        public bool ValidatePassword(string password);
            public EUser GetUserByEmail(string email);

        //image
        public ActionResult<ImageUserDTO> GetImageHeader(int id);
        public ActionResult<ImageUserDTO> GetImage(int id);

        //follow
        public EFollowing GetFollow(int FollowerId, int FollowedId);
        public void Follow(EFollowing follow);
        public void UnFollow(EFollowing follow);

        //get who to follow
        public Task<UserResponseDTO> GetWhoToFollow( int id,int page);

    }
}
