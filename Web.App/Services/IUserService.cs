namespace Web.App.Services
{
    public interface IUserService
    {
        string GetUserId();
        bool IsAuthenticated();
    }
}