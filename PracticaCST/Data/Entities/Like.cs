namespace PracticaCST.Data.Entities
{
    public class Like
    {
        public int Id { get; set; }
        public User User { get; set; }
        public Post Post { get; set; }
    }
}
