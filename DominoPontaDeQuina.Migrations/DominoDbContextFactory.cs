using DominoPontaDeQuina.Repository.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DominoPontaDeQuina.Migrations;

public class DominoDbContextFactory : IDesignTimeDbContextFactory<DominoDbContext>
{
    public DominoDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<DominoDbContext>()
            .UseSqlite("Data Source=domino.db")
            .Options;

        return new DominoDbContext(options);
    }
}
