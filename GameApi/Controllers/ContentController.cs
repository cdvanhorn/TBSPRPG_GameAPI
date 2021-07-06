using System;
using System.Threading.Tasks;
using GameApi.Services;
using GameApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using TbspRpgLib.InterServiceCommunication.RequestModels;

namespace GameApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]/{gameId:guid}")]
    public class ContentController : ControllerBase
    {
        private readonly IContentService _contentService;

        public ContentController(IContentService contentService)
        {
            _contentService = contentService;
        }
        
        [Authorize, Route("latest")]
        public async Task<IActionResult> GetLatestForGame(Guid gameId) {
            var content = await _contentService.GetLatestForGame(gameId);
            //the new game event may not have been handled yet, I should add that to the tests
            if(content != null)
                return Ok(new ContentViewModel(content));
            return Ok();
        }
        
        [Authorize, Route("after/{position}")]
        public async Task<IActionResult> GetContentAfterPosition(Guid gameId, ulong position) {
            var content = await _contentService.GetContentForGameAfterPosition(gameId, position);
            if(content.Count > 0)
                return Ok(new ContentViewModel(content));
            return Ok();
        }

        [Authorize, HttpGet("filter")]
        public async Task<IActionResult> FilterContent(Guid gameId, [FromQuery] ContentFilterRequest filterRequest) {
            try
            {
                // var content = await _contentService.GetPartialContentForGame(gameId, filterRequest);
                // if(content.Count > 0)
                //     return Ok(new ContentViewModel(content));
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "invalid filter arguments" });
            }
        }
    }
}