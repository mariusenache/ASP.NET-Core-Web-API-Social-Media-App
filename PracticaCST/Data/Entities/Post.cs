namespace PracticaCST.Data.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public string AvatarP { get; set; }
        public int Likes { get; set; }
        public User User { get; set; }

    }
}
