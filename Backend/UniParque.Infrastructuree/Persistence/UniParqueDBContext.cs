using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniParque_Domain.Entities;

namespace UniParque_Infrastructure.Persistence;

public class UniParqueDBContext : IdentityDbContext<AppUser>
{
    public UniParqueDBContext(DbContextOptions options) : base(options) { }

    public DbSet<ParkingBranch> ParkingBranches => Set<ParkingBranch>();
    public DbSet<ParkingSection> ParkingSections => Set<ParkingSection>();
    public DbSet<ParkingSubSection> ParkingSubSections => Set<ParkingSubSection>();
    public DbSet<ParkingPlace> ParkingPlaces => Set<ParkingPlace>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<ParkingReservation> Reservations => Set<ParkingReservation>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<UserVerificationCode> UserVerificationCodes => Set<UserVerificationCode>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ParkingBranch
        modelBuilder.Entity<ParkingBranch>(
            entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BranchName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

        // ParkingSection
        modelBuilder.Entity<ParkingSection>(
            entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Section)
                    .IsRequired()
                    .HasMaxLength(5);
                entity.HasOne(ps => ps.Branch)
                      .WithMany(b => b.Sections)
                      .HasForeignKey(ps => ps.BranchId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

        // ParkingSubSection
        modelBuilder.Entity<ParkingSubSection>(
            entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SubSection)
                    .IsRequired()
                    .HasMaxLength(5);
                entity.HasOne(pss => pss.Section)
                      .WithMany(ps => ps.SubSections)
                      .HasForeignKey(pss => pss.SectionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

        // ParkingPlace
        modelBuilder.Entity<ParkingPlace>(
            entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PlaceName)
                    .IsRequired()
                    .HasMaxLength(20);
                entity.HasOne(pp => pp.SubSection)
                      .WithMany(pss => pss.Places)
                      .HasForeignKey(pp => pp.SubSectionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

        // RefreshToken
        modelBuilder.Entity<RefreshToken>(
            entity =>
            {
                entity
                    .HasKey(e => e.Id);

                entity
                    .HasIndex(e => e.JwtId)
                    .IsUnique();

                entity
                    .Property(e => e.JwtId)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);
            });

        // ParkingReservation
        modelBuilder.Entity<ParkingReservation>(
            entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(r => r.EstimatedArrivalTime);
                entity.Property(e => e.CarNumber)
                    .IsRequired();
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Reservations)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Place)
                    .WithMany(p => p.Reservations)
                    .HasForeignKey(e => e.PlaceId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Property(p => p.TotalPrice)
                    .HasColumnType("decimal(18,2)");
            });

        // Photo
        modelBuilder.Entity<Photo>(
            entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Url)
                    .IsRequired();
                entity.Property(e => e.PublicId)
                    .IsRequired();
                entity.HasOne(e => e.User)
                    .WithOne(u => u.Photo)
                    .HasForeignKey<Photo>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

        modelBuilder.Entity<AppUser>(
            entity =>
            {
                entity.Property(p => p.Balance)
                    .HasPrecision(18, 2);
                entity.HasOne(e => e.Photo)
                    .WithOne(u => u.User)
                    .HasForeignKey<Photo>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

        // UserVerificationCode
        modelBuilder.Entity<UserVerificationCode>(
            entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code)
                    .IsRequired();
                entity.Property(e => e.ExpiresAt)
                    .IsRequired();
                entity.HasOne(e => e.User)
                    .WithOne(u => u.CodeVerification)
                    .HasForeignKey<UserVerificationCode>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

        // UserVerificationCode
        modelBuilder.Entity<Payment>(
            entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CardNumber)
                    .IsRequired();
                entity.Property(p => p.Amount)
                    .HasPrecision(18, 2);
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Payments)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
    }
}
