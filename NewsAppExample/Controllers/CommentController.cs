using Microsoft.AspNetCore.Authorization;
using System.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using NewsAppExample.DTO;
using NewsAppExample.Helper;
using NewsAppExample.Model;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;


namespace NewsAppExample.Controllers
{
    public class CommentController(NewsContext context, IConfiguration configuration) : Controller
    {
        private readonly NewsContext _context = context;
        private readonly IConfiguration _configuration = configuration;
        private readonly IPasswordHashService _passwordHashService = new PasswordHashService();



        [HttpGet("Comment")]
        [AllowAnonymous]
        public async Task<IActionResult> GetComments(int articleId)
        {
            var comments = await _context.Comments
                                     .Where(c => c.ArticleId == articleId)
                                     .ToListAsync();

            if (comments.Count == 0)
                return NotFound();


            return Ok(comments);

        }


        [HttpPost("CreateComment")]
        [Authorize]
        public async Task<IActionResult> CreateComment([FromBody] CommentDto commentDto)
        {
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            int userId = Int32.Parse(userIdClaim.Value);


            //find user in DB
            User user = await _context.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return Unauthorized();



            //check if user has the needed role
            bool authorized = false;

            if (user.Role.Name == "Subscriber")
                authorized = true;

            if (!authorized)
                return Unauthorized("You do not have access to this functionality as a " + user.Role.Name);


            //create new comment
            Comment comment = new Comment();

            comment.User = user;
            comment.ArticleId = commentDto.articleId;
            comment.Content = commentDto.Content;
            comment.CreatedAt = DateTime.Now;

            try
            {
                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }



        [HttpPatch("EditComment")]
        [Authorize]
        public async Task<IActionResult> EditComment([FromBody] CommentDto commentDto)
        {
            //find user ID
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            int userId = Int32.Parse(userIdClaim.Value);


            //find user in DB
            User user = await _context.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return Unauthorized();


            //find comment 
            Comment comment = await _context.Comments.SingleOrDefaultAsync(c => c.CommentId == commentDto.Id);
            
            if (comment == null)
                return NotFound();


            // check if user is editor
            bool authorized = false;

            if (user.Role.Name == "Editor")
                authorized = true;

            if (!authorized)
                return Unauthorized("You do not have access to this functionality");



            //edit comment
            comment.Content = commentDto.Content;
            
            try
            {
                _context.Comments.Update(comment);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpDelete("DeleteComment")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            int userId = Int32.Parse(userIdClaim.Value);


            //find user in DB
            User user = await _context.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return Unauthorized();


            //find comment
            Comment comment = await _context.Comments.SingleOrDefaultAsync(c => c.CommentId == commentId);


            if (comment == null)
                return NotFound();


            //check if user is either article author or editor
            bool authorized = false;

            if (user.Role.Name == "Editor")
                authorized = true;

            if (!authorized)
                return Unauthorized("you are not an editor. You are a" + user.Role.Name);


            try
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
