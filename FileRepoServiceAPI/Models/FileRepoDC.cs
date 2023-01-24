using Microsoft.EntityFrameworkCore;


namespace FileRepoServiceApi.Models
{   
    public class FileRepoDC : DbContext
    {

        public FileRepoDC(DbContextOptions<FileRepoDC> options) : base (options)
        {
            Database.Migrate();
        }



        public DbSet<FileItem> Files => Set<FileItem>();
    }
}
 