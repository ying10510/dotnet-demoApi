using Microsoft.AspNetCore.Mvc;
using Demo.Dtos.Member;
using Demo.Services;
using Microsoft.AspNetCore.Authorization;

namespace Demo.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MemberController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MemberController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    /// <summary>
    /// 建立會員
    /// </summary>
    /// <param name="memberCreateDto"></param>
    /// <returns></returns>
    /// <exception cref="AppException"></exception>
    [HttpPost]
    public async Task<ActionResult<MemberReadDto>> CreateMember([FromBody] MemberCreateDto memberCreateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("request body is invalid");
        }

        var created = await _memberService.Create(memberCreateDto);
        return CreatedAtAction(nameof(GetMember), new { id = created.Id }, created);
    }

    /// <summary>
    /// 刪除會員
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMember(string id)
    {
        await _memberService.Delete(id);
        return NoContent();
    }

    /// <summary>
    /// 查詢所有會員
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<List<MemberReadDto>>> GetMember()
    {
        var members = await _memberService.GetAll();
        return Ok(members);
    }

    /// <summary>
    /// 查詢指定會員
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<MemberReadDto>> GetMemberById(string id)
    {
        var member = await _memberService.GetById(id);
        return Ok(member);
    }

    /// <summary>
    /// 更新指定會員資料
    /// </summary>
    /// <param name="id">會員Id</param>
    /// <param name="memberUpdateDto">會員更新資料</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<MemberReadDto>> UpdateMember(string id, [FromBody] MemberUpdateDto memberUpdateDto)
    {
        // 檢查 null
        if (memberUpdateDto == null)
            return BadRequest("請提供更新資料。");

        // 檢查模型驗證（例如 [Required]、[MaxLength] 等）
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var member = await _memberService.Update(id, memberUpdateDto);
        return Ok(member);
    }

}
