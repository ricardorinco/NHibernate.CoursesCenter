namespace Persistence.POCO
{
    public class RequestDetail
    {
        public virtual int Id { get; set; }
        
        public virtual Request Request { get; set; }
        public virtual Course Course { get; set; }
    }
}