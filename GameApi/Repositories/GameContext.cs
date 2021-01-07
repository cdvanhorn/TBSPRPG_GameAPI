using Microsoft.EntityFrameworkCore;
using GameApi.Entities;

namespace GameApi.Repositories {
    public class GameContext : DbContext{
        public GameContext(DbContextOptions<GameContext> options) : base(options){}

        public DbSet<Adventure> Adventures { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<GameEvent> GameEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<Adventure>().ToTable("adventures");
            modelBuilder.Entity<Game>().ToTable("games");
            modelBuilder.Entity<GameEvent>().ToTable("game_events");

            modelBuilder.Entity<Adventure>().HasKey(a => a.Id);
            modelBuilder.Entity<Adventure>().Property(a => a.Id)
                .HasColumnType("uuid")
                .HasDefaultValueSql("uuid_generate_v4()")
                .IsRequired();

            modelBuilder.Entity<Game>().HasKey(a => a.Id);
            modelBuilder.Entity<Game>().Property(a => a.Id)
                .HasColumnType("uuid")
                .HasDefaultValueSql("uuid_generate_v4()")
                .IsRequired();

            modelBuilder.Entity<GameEvent>().HasKey(a => a.Id);
            modelBuilder.Entity<GameEvent>().Property(a => a.Id)
                .HasColumnType("uuid")
                .HasDefaultValueSql("uuid_generate_v4()")
                .IsRequired();

            modelBuilder.Entity<Adventure>()
                 .HasMany(a => a.Games)
                 .WithOne(g => g.Adventure)
                 .HasForeignKey(g => g.AdventureId);

            modelBuilder.Entity<Game>()
                .HasMany(g => g.Events)
                .WithOne();
        }
    }
}