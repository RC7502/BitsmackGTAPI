using System.Data.Entity;

namespace BitsmackGTAPI
{
    public interface IBSGTEntities
    {
        DbSet<Cardio> Cardio { get; set; }
        DbSet<Pedometer> Pedometer { get; set; }
    }
}