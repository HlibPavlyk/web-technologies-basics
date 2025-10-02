using Lab6.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab6.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<SubscriberEntity> Subscribers { get; set; }
    public DbSet<MailingListEntity> MailingLists { get; set; }
    public DbSet<MailingListSubscriberEntity> MailingListSubscribers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MailingListSubscriberEntity>()
            .HasKey(mls => new { mls.MailingListId, mls.SubscriberId });

        modelBuilder.Entity<MailingListSubscriberEntity>()
            .HasOne(mls => mls.MailingList)
            .WithMany(m => m.MailingListSubscribers)
            .HasForeignKey(mls => mls.MailingListId);

        modelBuilder.Entity<MailingListSubscriberEntity>()
            .HasOne(mls => mls.Subscriber)
            .WithMany(s => s.MailingListSubscribers)
            .HasForeignKey(mls => mls.SubscriberId);

        base.OnModelCreating(modelBuilder);
    }
}