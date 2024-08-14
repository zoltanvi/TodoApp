using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Modules.Common;
using Modules.Common.Database;
using Modules.Tasks.Contracts.Models;

namespace Modules.Tasks.Repositories;

public class TaskItemDbContext : DbContext
{
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<TaskItemVersion> TaskItemVersions { get; set; }
    public DbSet<Reminder> Reminders { get; set; }
    public DbSet<TagItem> Tags { get; set; }

    public DbSet<TaskItemsDbInfo> TasksDbInfo { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(DbConfiguration.ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var dateTimeConverter = new ValueConverter<DateTime, string>(
            v => v.ToString(Constants.SortableDateFormat),
            v => DateTime.ParseExact(v, Constants.SortableDateFormat, null)
        );

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.CategoryId);

            entity
                .Property(e => e.CategoryId)
                .IsRequired();

            entity
                .Property(e => e.Content)
                .IsRequired();

            entity
                .Property(e => e.IsContentPlainText)
                .IsRequired();

            entity
                .Property(e => e.ContentPreview)
                .IsRequired();

            entity
                .HasMany(e => e.Versions)
                .WithOne(e => e.TaskItem)
                .HasForeignKey(e => e.TaskId);

            entity
                .Property(e => e.ListOrder)
                .HasDefaultValue(0);

            entity
                .Property(e => e.CreationDate)
                .HasConversion(dateTimeConverter);

            entity
                .Property(e => e.ModificationDate)
                .HasConversion(dateTimeConverter);

            entity
                .Property(e => e.DeletedDate)
                .HasConversion(dateTimeConverter);

            entity
                .HasMany(e => e.Reminders)
                .WithOne(e => e.TaskItem)
                .HasForeignKey(e => e.TaskId);
        });


        modelBuilder.Entity<TaskItemVersion>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity
                .Property(e => e.Content)
                .IsRequired();

            entity
                .Property(e => e.IsContentPlainText)
                .IsRequired();

            entity
                .Property(e => e.ContentPreview)
                .IsRequired();

            entity
                .Property(e => e.VersionDate)
                .HasConversion(dateTimeConverter)
                .IsRequired();

            entity
                .HasOne(e => e.TaskItem)
                .WithMany(e => e.Versions)
                .HasForeignKey(e => e.TaskId);
        });


        modelBuilder.Entity<Reminder>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity
                .Property(e => e.ReminderDate)
                .HasConversion(dateTimeConverter)
                .IsRequired();

            entity
                .Property(e => e.TaskId)
                .IsRequired();
        });


        modelBuilder.Entity<TaskItemsDbInfo>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity
                .Property(e => e.Initialized)
                .HasDefaultValue(false);
        });


        modelBuilder.Entity<TagItem>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity
                .Property(e => e.Name)
                .IsRequired();

            entity
                .Property(e => e.Color)
                .IsRequired();
        });
    }
}