using System.Linq.Expressions;
using DemoApi.Models;

namespace DemoApi.Repositories;

public interface IMemberRepository : IAppRepository
{
    Task Create(Member member);
    Task Delete(Member member);
    Task<IEnumerable<Member>> GetAll();
    Task<Member?> GetById(string id);
    Task<Member?> GetByEmail(string email);
    Task Update(Member member);
    Task UpdatePartial(Member member, params Expression<Func<Member, object>>[] updatedProperties);
    Task<bool> IsEmailUsed(string email);
}