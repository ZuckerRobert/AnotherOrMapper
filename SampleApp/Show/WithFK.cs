using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NewOrMapper_if19b098;
using  SampleApp.School;



namespace SampleApp.Show
{
    /// <summary>This show case demonstrates working with simple foreign keys.</summary>
    public static class WithFK
    {
        /// <summary>Implements the show case.</summary>
        public static void Show()
        {
            Console.WriteLine("(3) Create and load an object with foreign key");
            Console.WriteLine("----------------------------------------------");

            Teacher t = Orm.Get<Teacher>("t.0");

            Class c = new Class();
            c.ID = "c.0";
            c.Name = "Demonolgy 101";
            c.Teacher = t;

            Orm.Save(c);

            c = Orm.Get<Class>("c.0");
            Console.WriteLine((c.Teacher.Gender == Gender.MALE ? "Mr. " : "Ms. ") + c.Teacher.Name + " teaches " + c.Name + ".");

            Console.WriteLine("\n");
        }
    }
}
