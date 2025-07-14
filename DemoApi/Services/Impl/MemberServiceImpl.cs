using AutoMapper;
using Microsoft.EntityFrameworkCore;
using DemoApi.Data;
using DemoApi.Dtos.Member;
using DemoApi.Exceptions;
using DemoApi.Models;
using DemoApi.Repositories;

namespace DemoApi.Services.Impl;

public class MemberServiceImpl : IMemberService
{
    private readonly AppDbContext _appDbContext;
    private readonly IMapper _mapper;
    private readonly IMemberRepository _memberRepository;

    public MemberServiceImpl(AppDbContext appDbContext, IMapper mapper, IMemberRepository memberRepository)
    {
        _appDbContext = appDbContext;
        _mapper = mapper;
        _memberRepository = memberRepository;
    }

    /// <summary>
    /// 建立會員資料
    /// </summary>
    /// <param name="memberCreateDto">會員建立資料dto</param>
    /// <returns></returns>
    public async Task<MemberReadDto> Create(MemberCreateDto memberCreateDto)
    {
        // 檢查 Email 是否重複
        if (await IsEmailUsed(memberCreateDto.Email)) throw new AppException("Email already in use", 409);

        // 密碼加密
        var member = _mapper.Map<Member>(memberCreateDto);

        try
        {
            await _memberRepository.Create(member);
            await _appDbContext.SaveChangesAsync();

            return _mapper.Map<MemberReadDto>(member);
        }
        catch (DbUpdateException dbEx)
        {
            throw new AppException($"Member Create Fail: {dbEx.Message}", 500);
        }
    }

    /// <summary>
    /// 刪除會員資料
    /// </summary>
    /// <param name="id">會員Id</param>
    /// <returns></returns>
    public async Task<bool> Delete(string id)
    {
        var member = await _memberRepository.GetById(id) ?? throw new AppException("Member Not Found.", 404);

        try
        {
            await _memberRepository.Delete(member);
            await _appDbContext.SaveChangesAsync();

            return true;
        }
        catch (DbUpdateException dbEx)
        {
            throw new AppException($"Member Delete Fail: {dbEx.Message}", 500);
        }
    }

    /// <summary>
    /// 查詢全部會員資料
    /// </summary>
    /// <returns>回傳所有會員資料</returns>
    public async Task<List<MemberReadDto>> GetAll()
    {
        var members = await _memberRepository.GetAll();

        return _mapper.Map<List<MemberReadDto>>(members);
    }

    /// <summary>
    /// 查詢指定會員資料
    /// </summary>
    /// <param name="email">信箱</param>
    /// <returns>回傳指定會員資料</returns>
    public async Task<MemberReadDto?> GetByEmail(string email)
    {
        var member = await _memberRepository.GetByEmail(email) ?? throw new AppException("Member Email not found", 404);

        return _mapper.Map<MemberReadDto>(member);
    }

    /// <summary>
    /// 查詢指定會員資料
    /// </summary>
    /// <param name="id">會員Id</param>
    /// <returns>回傳指定會員資料</returns>
    public async Task<MemberReadDto?> GetById(string id)
    {
        var member = await _memberRepository.GetById(id) ?? throw new AppException("Member Id not found", 404);

        return _mapper.Map<MemberReadDto>(member);
    }

    /// <summary>
    /// 更新會員資料
    /// </summary>
    /// <param name="id">會員Id</param>
    /// <param name="memberUpdateDto">會員更新資料dto/param>
    /// <returns></returns>
    public async Task<MemberReadDto?> Update(string id, MemberUpdateDto memberUpdateDto)
    {
        var member = await _memberRepository.GetById(id) ?? throw new AppException("Member not found", 404);

        try
        {
            member.SetName(memberUpdateDto.Name);
            member.SetEmail(memberUpdateDto.Email);
            member.SetPhone(memberUpdateDto.Phone);
            member.SetAddress(memberUpdateDto.Address);
            member.SetUpdateTime(DateTime.Now);

            await _memberRepository.UpdatePartial(member);
            await _appDbContext.SaveChangesAsync();

            return _mapper.Map<MemberReadDto>(member);
        }
        catch (DbUpdateException dbEx)
        {
            throw new AppException($"Member Update Fail: {dbEx.Message}", 500);
        }
    }

    public async Task<bool> IsEmailUsed(string email)
    {
        return await _memberRepository.IsEmailUsed(email);
    }

}