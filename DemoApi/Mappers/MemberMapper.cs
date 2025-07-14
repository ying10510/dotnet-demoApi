using AutoMapper;
using DemoApi.Dtos.Member;
using DemoApi.Models;

namespace DemoApi.Mappers;

public class MemberMapper : Profile
{
    public MemberMapper()
    {
        CreateMap<Member, MemberReadDto>();
        CreateMap<MemberCreateDto, Member>().ForMember(dest => dest.Password, opt => opt.Ignore()); // 密碼由程式處理
        CreateMap<MemberUpdateDto, Member>();
        CreateMap<MemberReadDto, Member>();
    }
}