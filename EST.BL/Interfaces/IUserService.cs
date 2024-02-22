using EST.DAL.Models;

namespace EST.BL.Interfaces
{
    public interface IUserService : IBaseService<User>
    {
        Task<List<User>> GetAll();
    }
}
