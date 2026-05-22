namespace Weather.Domain.Models
{
   public interface IEntity
   {
      int Id { get; set; }
   }
   public class Entity : IEntity
   {
      public virtual int Id { get; set; }
   }
}
