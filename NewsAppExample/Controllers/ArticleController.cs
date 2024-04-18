using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsAppExample.DTO;
using NewsAppExample.Helper;
using NewsAppExample.Model;

namespace NewsAppExample.Controllers
{
    
    public class ArticleController(NewsContext context, IConfiguration configuration) : Controller
    {
        private readonly NewsContext _context = context;
        private readonly IConfiguration _configuration = configuration;
        private readonly IPasswordHashService _passwordHashService = new PasswordHashService();
        private readonly string[] advertisements = 
        {
            "Get to know hot milfs in your area!!!!" ,
            "Miracle Weight Loss Pill: Shed Pounds Overnight!",
            "Exclusive Offer: Free iPhone 15 Pro (Limited Stock)!",
            "Earn $100/hr Working from Home - No Skills Required!"
        };



        [HttpGet("Article")]
        [AllowAnonymous]
        public async Task<IActionResult> GetArticle(int id)
        {
            User user = null;
            bool withAdds = true;


            //find user
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null)
                 user = await _context.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.UserId == Int32.Parse(userIdClaim.Value));

           

            if (user != null)
                if (user.Role.Name == "subscriber")
                    withAdds = false;
                




            Article article = await _context.Articles.SingleOrDefaultAsync(a => a.ArticleId == id);

            if (article == null)
                return NotFound();
            
            ArticleDto articleDto = new ArticleDto()
            {
                Id = article.ArticleId,
                Title = article.Title,
                Content = article.Content
            };

            //show ads for non subscribed users
            if (withAdds)
            {
                Random random = new Random();
                int randomIndex = random.Next(0, 4);
                articleDto.Advertisement = advertisements[randomIndex];
            }

            return Ok(articleDto);
        }


        [HttpPost ("CreateArticle")]
        [Authorize]
        public async Task<IActionResult> CreateArticle([FromBody] ArticleDto articleDto)
        {
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            int userId = Int32.Parse(userIdClaim.Value);


            //find user in DB
            User user = await _context.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return Unauthorized();



            //check if user is either article author or editor
            bool authorized = false;

            if (user.Role.Name == "Editor")
                authorized = true;

            if (user.Role.Name == "Writer")
                authorized = true;

            if (!authorized)
                return Unauthorized("You do not have access to this functionality as a " + user.Role.Name);


            //create new article
            Article article = new Article();

            article.Title = articleDto.Title;
            article.Content = articleDto.Content;
            article.Author = user;
            article.CreatedAt = DateTime.Now;


            try
            {
                _context.Articles.Add(article);
                await _context.SaveChangesAsync();
                return Ok(article.ArticleId);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }



        [HttpPatch ("EditArticle")]
        [Authorize]
        public async Task<IActionResult> EditArticle([FromBody] ArticleDto articleDto)
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


            //find article
            Article article = await _context.Articles.Include(a => a.Author).SingleOrDefaultAsync(a => a.ArticleId == articleDto.Id);
            
            if(article == null)
                return NotFound();


            //check if user is either article author or editor
            bool authorized = false;

            if (user.Role.Name == "Editor")
                authorized = true;

            if (article.Author.UserId == user.UserId)
                authorized = true;

            if (!authorized)
                return Unauthorized("you are not the author of this article or not an editor");



            //edit article
            article.Title = articleDto.Title;
            article.Content = articleDto.Content;

            try
            {
                _context.Articles.Update(article);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpDelete("DeleteArticle")]
        [Authorize]
        public async Task<IActionResult> DeleteArticle(int articleId)
        {
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            int userId = Int32.Parse(userIdClaim.Value);


            //find user in DB
            User user = await _context.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return Unauthorized();



            //find article
            Article article = await _context.Articles.Include(a => a.Author).SingleOrDefaultAsync(a => a.ArticleId == articleId);

            if (article == null)
                return NotFound();


            //check if user is either article author or editor
            bool authorized = false;

            if (user.Role.Name == "Editor")
                authorized = true;

            if (!authorized)
                return Unauthorized("you are not an editor. You are a" + user.Role.Name);


            try
            {
                _context.Articles.Remove(article);
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
