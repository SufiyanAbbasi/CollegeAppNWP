using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollegeApp.Data.Config
{
    public class StudentConfig : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Students");
            builder.HasKey(x => x.Id);
            builder.Property(x=> x.Id).UseIdentityColumn();
            builder.Property(n => n.StudentName).IsRequired();
            builder.Property(n => n.StudentName).HasMaxLength(150);
            builder.Property(n => n.Address).IsRequired(false);
            builder.Property(n => n.Email).IsRequired().HasMaxLength(200);

            builder.HasData(new List<Student>()
            {
            new Student {
                Id = 1,
                StudentName = "Waqas",
                Address = "Atd",
                Email = "waqas@gmail.com",
                DOB = new DateTime(2000, 12, 12)
            },
            new Student
            {
                Id = 2,
                StudentName = "Sufiyan",
                Address = "Atd",
                Email = "sufi@gmail.com",
                DOB = new DateTime(2002, 12, 12)
            }
            });

            builder.HasOne(n => n.Department)
                .WithMany(n => n.Students)
                .HasForeignKey(n => n.DepartmentId)
                .HasConstraintName("FK_Students_Department");
        }
    }
}
