using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewOrMapper_if19b098;
using SampleApp.School;



namespace  SampleApp.Show
{
    /// <summary>This show case uses an m:n relationship.</summary>
    public static class WithMToN
    {
        /// <summary>Implements the show case.</summary>
        public static void Show()
        {
            Console.WriteLine("(5) Create and load an object with m:n");
            Console.WriteLine("--------------------------------------");

            Course c = new Course();
            c.ID = "x.0";
            c.Name = "Demons 1";
            c.Teacher = Orm.Get<Teacher>("t.0");

            Student s = new Student();
            s.ID = "s.0";
            s.Name = "Aalo";
            s.FirstName = "Alice";
            s.Gender = Gender.FEMALE;
            s.BirthDate = new DateTime(1990, 1, 12);
            s.Grade = 1;
            Orm.Save(s);

            c.Students.Add(s);

            s = new Student();
            s.ID = "s.1";
            s.Name = "Bumblebee";
            s.FirstName = "Bernard";
            s.Gender = Gender.MALE;
            s.BirthDate = new DateTime(1991, 9, 23);
            s.Grade = 2;
            Orm.Save(s);

            c.Students.Add(s);

            Orm.Save(c);

            c = Orm.Get<Course>("x.0");

            Console.WriteLine("Students in " + c.Name + ":");
            foreach(Student i in c.Students)
            {
                Console.WriteLine(i.FirstName + " " + i.Name);
            }

            Console.WriteLine("\n");
        }
    }
}
