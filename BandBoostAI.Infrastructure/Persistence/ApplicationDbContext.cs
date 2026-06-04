using BandBoostAI.Domain.Common;
using BandBoostAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BandBoostAI.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // Khai báo các bảng trong Database
    public DbSet<User> Users => Set<User>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<ExamSection> ExamSections => Set<ExamSection>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<AiEvaluation> AiEvaluations => Set<AiEvaluation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Cấu hình Global Query Filter cho Soft Delete
        // Khi query db.Users.ToList(), EF Core sẽ tự động gắn thêm điều kiện: WHERE IsDeleted = 0
        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Exam>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ExamSection>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Question>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<AiEvaluation>().HasQueryFilter(e => !e.IsDeleted);

        // 2. Ép kiểu cột FeedbackJson của AI lưu trữ chuỗi không giới hạn
        modelBuilder.Entity<AiEvaluation>()
            .Property(e => e.FeedbackJson)
            .HasColumnType("nvarchar(max)");
    }

    // 3. Tự động can thiệp trước khi lưu xuống Database
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Quét qua tất cả các entity đang có sự thay đổi
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                // Khi tạo mới: Gắn CreatedAt hiện tại
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.IsDeleted = false;
                    break;

                // Khi cập nhật: Gắn UpdatedAt hiện tại
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;

                // TUYỆT CHIÊU: Biến Hard Delete (xóa hẳn) thành Soft Delete (xóa mềm)
                // Khi ai đó gọi hàm db.Users.Remove(user), hệ thống sẽ chặn lại
                case EntityState.Deleted:
                    entry.State = EntityState.Modified; // Đổi trạng thái thành Cập nhật
                    entry.Entity.IsDeleted = true;      // Đánh dấu là đã xóa
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}