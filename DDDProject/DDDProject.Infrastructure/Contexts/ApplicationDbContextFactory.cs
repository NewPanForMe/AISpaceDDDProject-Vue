using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DDDProject.Infrastructure.Contexts;

/// <summary>
/// 设计时数据库上下文工厂
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=AiSpace;Persist Security Info=True;User ID=sa;Password=123456;Encrypt=False");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
