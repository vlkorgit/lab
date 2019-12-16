using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MyDb;Trusted_Connection=True;");
        }
        public DbSet<File> Files { get; set; }
        public DbSet<Picture> Pictures { get; set; }
    }
    public class Picture
    {
        public int Id { get; set; }
        public byte[] Bytes { get; set; }
    }
    public class File
    {
        [Key]
        [ForeignKey("Blob")]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Hash { get; set; } //лучше записать хэш-код, чем каждый раз его из массива байт создавать
        public int Number { get; set; }
        public float Probability { get; set; }
        public int Statistics { get; set; }
        public virtual Picture Blob { get; set; }
    }
}
