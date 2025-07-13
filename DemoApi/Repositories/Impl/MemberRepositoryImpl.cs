using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Demo.Data;
using Demo.Dtos.Member;
using Demo.Models;
using Demo.Services;

namespace Demo.Repositories.Impl;

public class MemberRepositoryImpl : IMemberRepository
{
    private readonly AppDbContext _appDbContext;

    public MemberRepositoryImpl(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    /// <summary>
    /// 新增會員
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public async Task Create(Member member)
    {
        await _appDbContext.Members.AddAsync(member);
    }

    /// <summary>
    /// 刪除會員
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public Task Delete(Member member)
    {
        _appDbContext.Members.Remove(member);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 查詢全部會員資料
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Member>> GetAll()
    {
        return await _appDbContext.Members.ToListAsync();
    }

    /// <summary>
    /// 查詢指定會員資料
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public async Task<Member?> GetById(string id)
    {
        return await _appDbContext.Members.FindAsync(id);
    }

    /// <summary>
    /// 根據 Email 查詢會員資料
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public async Task<Member?> GetByEmail(string email)
    {
        return await _appDbContext.Members.FirstOrDefaultAsync(m => m.Email == email);
    }

    /// <summary>
    /// 更新會員資料
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public Task Update(Member member)
    {
        _appDbContext.Members.Update(member);
        return Task.CompletedTask;

    }

    /// <summary>
    /// 更新會員資料-只更新異動欄位
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public Task UpdatePartial(Member member, params Expression<Func<Member, object>>[] updatedProperties)
    {
        _appDbContext.Members.Attach(member);
        foreach (var property in updatedProperties)
        {
            _appDbContext.Entry(member).Property(property).IsModified = true;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 檢查 Email 是否已被使用
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public async Task<bool> IsEmailUsed(string email)
    {
        return await _appDbContext.Members.AnyAsync(m => m.Email == email);
    }
}