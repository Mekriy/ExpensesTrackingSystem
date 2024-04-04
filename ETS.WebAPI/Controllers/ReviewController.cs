using EST.BL.Interfaces;
using EST.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ETS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        [HttpGet("{reviewId:Guid}")]
        public async Task<IActionResult> GetReviewById([FromRoute] Guid reviewId, CancellationToken token)
        {
            if (reviewId == Guid.Empty)
                return BadRequest("No guid");
            if (!await _reviewService.Exist(reviewId))
                return NotFound("No review found");
            return Ok(await _reviewService.GetReveiwDTO(reviewId, token));
        }
        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] ReviewDTO reviewDTO)
        {
            if (reviewDTO.ItemId == Guid.Empty)
                return BadRequest("No guid");
            if (reviewDTO == null)
                return BadRequest("No review");

            if (await _reviewService.Create(reviewDTO.ItemId, reviewDTO.UserId, reviewDTO))
                return Ok("Created successfully");
            else
                return StatusCode(500, "Error occured while creating review on server");
        }
        [HttpDelete("{reviewId:Guid}")]
        public async Task<IActionResult> DeleteReview([FromRoute] Guid reviewId)
        {
            if (reviewId == Guid.Empty)
                return BadRequest("No guid");
            if (!await _reviewService.Exist(reviewId))
                return NotFound("No review found to delete");
            if (await _reviewService.Delete(reviewId))
                return Ok("Review deleted!");
            else
                return StatusCode(500, "Error occured while deleting review on server");
        }
        [HttpGet("{itemId:Guid}")]
        public async Task<IActionResult> GetItemsRating([FromRoute] Guid itemId, CancellationToken token)
        {
            var rating = await _reviewService.GetItemsRating(itemId, token);
            if (rating == null || rating == 0)
                return NotFound("There is no rating for this item");

            return Ok(rating);
        }
    }
}
