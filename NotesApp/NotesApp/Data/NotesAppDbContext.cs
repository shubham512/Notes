using Microsoft.EntityFrameworkCore;
using NotesApp.Models;

namespace NotesApp.Data
{
    public class NotesAppDbContext : DbContext
    {
        public DbSet<Note> Notes { get; set; }

        public NotesAppDbContext(DbContextOptions<NotesAppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Note>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Title).HasMaxLength(200).IsRequired();
                entity.Property(n => n.Content);
                entity.Property(n => n.Priority).HasConversion<int>();
                entity.Property(n => n.CreatedAt);
                entity.Property(n => n.UpdatedAt);
            });
        }
    }
}
