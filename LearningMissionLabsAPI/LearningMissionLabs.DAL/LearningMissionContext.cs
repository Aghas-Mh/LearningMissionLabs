using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using LearningMissionLabs.DAL.Models;

namespace LearningMissionLabs.DAL
{
    public partial class LearningMissionContext : DbContext, ILearningMissionContext
    {
        public LearningMissionContext() 
        {
        }

        public LearningMissionContext(DbContextOptions<LearningMissionContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public DbSet<ServerOption> ServerOptions { get; set; }
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserInGroup> usersInGroup { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                .AddJsonFile("appsettings.json").Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(e => e.groupID);

                entity.Property(e => e.groupID).ValueGeneratedOnAdd().HasColumnName("groupID");
                entity.Property(e => e.groupName).HasMaxLength(50).HasColumnName("groupName");
                entity.Property(e => e.creatorID).HasColumnName("creatorID");
            });

            modelBuilder.Entity<UserInGroup>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID).ValueGeneratedOnAdd().HasColumnName("ID");
                entity.Property(e => e.groupID).HasColumnName("groupID");
                entity.Property(e => e.userID).HasColumnName("userID");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e =>  e.id);

                entity.Property(e => e.id).ValueGeneratedOnAdd().HasColumnName("Id");
                entity.Property(e => e.senderID).HasColumnName("SenderID");
                entity.Property(e => e.reciverID).HasColumnName("ReciverID");
                entity.Property(e => e.message).HasColumnName("Message");
            });

            modelBuilder.Entity<ServerOption>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("Id");
                entity.Property(e => e.RSAKey).HasColumnName("RSAKey");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.ImagePath)
                    .HasMaxLength(200)
                    .HasColumnName("ImagePath")
                    .IsFixedLength();

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title")
                    .IsFixedLength();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Email);

                entity.ToTable("User");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("email");

                entity.Property(e => e.Hash)
                    .HasMaxLength(64)
                    .HasColumnName("hash");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Role)
                    .HasMaxLength(10)
                    .HasColumnName("role");

                entity.Property(e => e.Salt)
                    .HasMaxLength(128)
                    .HasColumnName("salt");

                entity.Property(e => e.Token).HasColumnName("token");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public void SaveChanges()
        {
            base.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        public async Task<string> GetRSAKey()
        {
            var result = await ServerOptions.ToListAsync();
            return result.FirstOrDefault()?.RSAKey;
        }

        public async Task<bool> CreateRSA(string _privateKey)
        {
            await ServerOptions.AddAsync(new ServerOption{ RSAKey = _privateKey });
            await SaveChangesAsync();
            return true;
        }

    }
}
