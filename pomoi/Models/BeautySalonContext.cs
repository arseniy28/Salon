using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace pomoi.Models;

public partial class BeautySalonContext : DbContext
{
    public BeautySalonContext()
    {
    }

    public BeautySalonContext(DbContextOptions<BeautySalonContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Visit> Visits { get; set; }
    public DbSet<View> View { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localDB)\\MSSQLLocalDB;Initial Catalog=BeautySalon;Integrated Security=True;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PK__Clients__E67E1A24F5ADCE31");

            entity.HasIndex(e => e.Phone, "UQ__Clients__5C7E359E950FAFCF").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });
        {
            modelBuilder.Entity<View>().ToView("View").HasNoKey();
        }

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Services__C51BB00A523D9036");

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });


        modelBuilder.Entity<Visit>(entity =>
        {
            entity.HasKey(e => e.VisitId).HasName("PK__Visits__4D3AA1DE8B9342CE");

            entity.Property(e => e.VisitDate).HasColumnType("datetime");

            entity.HasOne(d => d.Client).WithMany(p => p.Visits)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__Visits__ClientId__29572725");

            entity.HasMany(d => d.Services).WithMany(p => p.Visits)
                .UsingEntity<Dictionary<string, object>>(
                    "VisitService",
                    r => r.HasOne<Service>().WithMany()
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__VisitServ__Servi__2D27B809"),
                    l => l.HasOne<Visit>().WithMany()
                        .HasForeignKey("VisitId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__VisitServ__Visit__2C3393D0"),
                    j =>
                    {
                        j.HasKey("VisitId", "ServiceId").HasName("PK__VisitSer__F16B1ADE0BD6BD2A");
                        j.ToTable("VisitServices");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
