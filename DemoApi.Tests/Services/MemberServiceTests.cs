using AutoMapper;
using Demo.Data;
using Demo.Mappers;
using Demo.Models;
using Demo.Repositories;
using Demo.Services.Impl;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Demo.Tests.Services;

public class MemberServiceTests
{
    // Add your test methods here
    private readonly IMapper _mapper;
    private readonly Mock<IMemberRepository> _memberRepoMock;
    private readonly AppDbContext _dbContext;
    private readonly MemberServiceImpl _memberService;

    public MemberServiceTests()
    {
        // 建立 InMemory DbContext
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // 每次唯一
            .Options;

        _dbContext = new AppDbContext(options);

        // 自動註冊 AutoMapper
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MemberMapper>(); // 確保用到你定義的 MemberMapper
        });
        _mapper = mapperConfig.CreateMapper();

        // 模擬 Repository
        _memberRepoMock = new Mock<IMemberRepository>();

        // 建立 Service 實例
        _memberService = new MemberServiceImpl(_dbContext, _mapper, _memberRepoMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnListOfMemberReadDto()
    {
        var members = new List<Member>();
        var m1 = new Member("Amy@example.com", "pw1");
        typeof(Member).GetProperty("Id")!.SetValue(m1, "1");
        m1.SetName("Amy");

        var m2 = new Member("Bob@example.com", "pw2");
        typeof(Member).GetProperty("Id")!.SetValue(m2, "2");
        m2.SetName("Bob");

        members.Add(m1);
        members.Add(m2);

        _memberRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(members);

        // Act
        var result = await _memberService.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Amy", result[0].Name);
        Assert.Equal("Bob@example.com", result[1].Email);
    }
}