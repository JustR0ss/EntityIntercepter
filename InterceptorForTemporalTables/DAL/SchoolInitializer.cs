using System;
using System.Collections.Generic;
using System.Linq;
using InterceptorForTemporalTables.DAL;
using InterceptorForTemporalTables.Models;
using InterceptorForTemporalTables.Models.ContosoUniversity.Models;

namespace InterceptorForTemporalTables.DAL
{
    public class SchoolInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<SchoolContext>
    {
        private const string SqlSystemVersionedDbFormat =
  @"ALTER TABLE dbo.{0}
    ADD SysStartTime datetime2(0)
    GENERATED ALWAYS AS ROW START HIDDEN CONSTRAINT
      DF_{0}_SysStart DEFAULT SYSUTCDATETIME(),
    SysEndTime datetime2(0)
    GENERATED ALWAYS AS ROW END HIDDEN CONSTRAINT
      DF_{0}_SysEnd DEFAULT CONVERT(datetime2 (0),
      '9999-12-31 23:59:59'),
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime)
  ALTER TABLE dbo.{0}
    SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.{0}History))";
        protected override void Seed(SchoolContext context)
        {
            // Grab the SQL Server version number
            var data = context
              .Database
              .SqlQuery<string>(@"select
  left(cast(serverproperty('productversion')
       as varchar), 4)")
              .FirstOrDefault();
            if (data != null)
            {
                var version = Convert.ToDecimal(data);
                if (version < 13)
                    throw new Exception("Invalid version of SQL Server");
            }
            // Prepare the SQL to turn system versioning on SQL Server 2016
            var cmd = String.Format(SqlSystemVersionedDbFormat, "Student");
            context.Database.ExecuteSqlCommand(cmd);
            cmd = String.Format(SqlSystemVersionedDbFormat, "StudentBook");
            context.Database.ExecuteSqlCommand(cmd);
            cmd = String.Format(SqlSystemVersionedDbFormat, "Course");
            context.Database.ExecuteSqlCommand(cmd);
            cmd = String.Format(SqlSystemVersionedDbFormat, "Enrollment");
            context.Database.ExecuteSqlCommand(cmd);
            cmd = String.Format(SqlSystemVersionedDbFormat, "Book");
            context.Database.ExecuteSqlCommand(cmd);
            var students = new List<Student>
            {
            new Student{FirstMidName="Carson",LastName="Alexander",EnrollmentDate=DateTime.Parse("2005-09-01")},
            new Student{FirstMidName="Meredith",LastName="Alonso",EnrollmentDate=DateTime.Parse("2002-09-01")},
            new Student{FirstMidName="Arturo",LastName="Anand",EnrollmentDate=DateTime.Parse("2003-09-01")},
            new Student{FirstMidName="Gytis",LastName="Barzdukas",EnrollmentDate=DateTime.Parse("2002-09-01")},
            new Student{FirstMidName="Yan",LastName="Li",EnrollmentDate=DateTime.Parse("2002-09-01")},
            new Student{FirstMidName="Peggy",LastName="Justice",EnrollmentDate=DateTime.Parse("2001-09-01")},
            new Student{FirstMidName="Laura",LastName="Norman",EnrollmentDate=DateTime.Parse("2003-09-01")},
            new Student{FirstMidName="Nino",LastName="Olivetto",EnrollmentDate=DateTime.Parse("2005-09-01")}
            };

            students.ForEach(s => context.Students.Add(s));
            context.SaveChanges();
            var courses = new List<Course>
            {
            new Course{CourseID=1050,Title="Chemistry",Credits=3,},
            new Course{CourseID=4022,Title="Microeconomics",Credits=3,},
            new Course{CourseID=4041,Title="Macroeconomics",Credits=3,},
            new Course{CourseID=1045,Title="Calculus",Credits=4,},
            new Course{CourseID=3141,Title="Trigonometry",Credits=4,},
            new Course{CourseID=2021,Title="Composition",Credits=3,},
            new Course{CourseID=2042,Title="Literature",Credits=4,}
            };
            courses.ForEach(s => context.Courses.Add(s));
            context.SaveChanges();
            var enrollments = new List<Enrollment>
            {
            new Enrollment{StudentID=1,CourseID=1050,Grade=Grade.A},
            new Enrollment{StudentID=1,CourseID=4022,Grade=Grade.C},
            new Enrollment{StudentID=1,CourseID=4041,Grade=Grade.B},
            new Enrollment{StudentID=2,CourseID=1045,Grade=Grade.B},
            new Enrollment{StudentID=2,CourseID=3141,Grade=Grade.F},
            new Enrollment{StudentID=2,CourseID=2021,Grade=Grade.F},
            new Enrollment{StudentID=3,CourseID=1050},
            new Enrollment{StudentID=4,CourseID=1050,},
            new Enrollment{StudentID=4,CourseID=4022,Grade=Grade.F},
            new Enrollment{StudentID=5,CourseID=4041,Grade=Grade.C},
            new Enrollment{StudentID=6,CourseID=1045},
            new Enrollment{StudentID=7,CourseID=3141,Grade=Grade.A},
            };
            enrollments.ForEach(s => context.Enrollments.Add(s));
            context.SaveChanges();
        }
    }
}