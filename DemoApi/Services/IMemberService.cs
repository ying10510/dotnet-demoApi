using DemoApi.Dtos.Member;

namespace DemoApi.Services;

public interface IMemberService : IAppService
{
    Task<MemberReadDto> Create(MemberCreateDto memberCreateDto);
    Task<bool> Delete(string id);
    Task<List<MemberReadDto>> GetAll();
    Task<MemberReadDto?> GetByEmail(string email);
    Task<MemberReadDto?> GetById(string id);
    Task<MemberReadDto?> Update(string id, MemberUpdateDto memberUpdateDto);
}