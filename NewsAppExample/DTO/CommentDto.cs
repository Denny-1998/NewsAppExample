namespace NewsAppExample.DTO
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int articleId { get; set; }
        public int userId { get; set; }

    }
}
