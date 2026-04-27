using Google.Protobuf.WellKnownTypes;
using JogsifoglaloApi.Model;
using Microsoft.EntityFrameworkCore;

namespace JogsifoglaloApi.Data
{
    public class JogsifoglaloContext : DbContext
    {
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Instruct> Instructs { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Pay> Pays { get; set; }
        public DbSet<User> Users { get; set; }
        public JogsifoglaloContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                        .Property(s => s.Szerepkor_Nev)
                        .HasConversion<string>();

            modelBuilder.Entity<User>()
                        .HasIndex(e => e.Email)
                        .IsUnique();

            modelBuilder.Entity<Appointment>()
                        .Property(a => a.Allapot)
                        .HasConversion<string>();

            modelBuilder.Entity<Appointment>()
                        .HasOne(a => a.Tanulo)
                        .WithMany(u => u.BookedAppointments)
                        .HasForeignKey(a => a.Tanulo_Id)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Appointment>()
                        .HasOne(a => a.Oktat)
                        .WithMany(i => i.Appointments)
                        .HasForeignKey(a => a.Oktat_Id)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Instructor>()
                        .HasIndex(f => f.Felhasznalo_Id)
                        .IsUnique();

            modelBuilder.Entity<Instructor>()
                        .HasOne(f => f.Felhasznalo)
                        .WithOne(i => i.Instructors)
                        .HasForeignKey<Instructor>(f => f.Felhasznalo_Id)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Instruct>().ToTable("oktat")
                        .HasIndex(o => new { o.Oktato_Id, o.Kategoria_Id })
                        .IsUnique();

            modelBuilder.Entity<Instruct>()
                        .HasOne(o => o.Oktato)
                        .WithMany(o => o.Instructs)
                        .HasForeignKey(o => o.Oktato_Id)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pay>()
                        .HasIndex(i => i.Idopont_Id)
                        .IsUnique();

            modelBuilder.Entity<Pay>()
                        .HasOne(p => p.Idopont)
                        .WithOne(a => a.Pay)
                        .HasForeignKey<Pay>(p => p.Idopont_Id)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>()
                        .Property(l => l.Leiras)
                        .HasColumnType("text");

        }
    }
}
