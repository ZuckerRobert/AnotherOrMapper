using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NewOrMapper_if19b098;
using SampleApp.School;



namespace SampleApp.Show
{
    /// <summary>This show case uses lazy loading.</summary>
    public static class WithLazyList
    {
        /// <summary>Implements the show case.</summary>
        public static void Show()
        {
            Console.WriteLine("(6) Use Lazy loading for student list");
            Console.WriteLine("-------------------------------------");

            Class c = Orm.Get<Class>("c.0");
            c.Students.Add(Orm.Get<Student>("s.0"));
            c.Students.Add(Orm.Get<Student>("s.1"));

            Orm.Save(c);

            c = Orm.Get<Class>("c.0");

            Console.WriteLine("Students in " + c.Name + ":");
            foreach(Student i in c.Students)
            {
                Console.WriteLine(i.FirstName + " " + i.Name);
            }

            Console.WriteLine("\n");
        }
    }
}
