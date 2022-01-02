using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NewOrMapper_if19b098;
using SampleApp.School;



namespace SampleApp.Show
{
    /// <summary>This show case loads and modifies an object and saves it to database.</summary>
    public static class ModifyObject
    {
        /// <summary>Implements the show case.</summary>
        public static void Show()
        {
            Console.WriteLine("(2) Load and modify object");
            Console.WriteLine("--------------------------");

            Teacher t = Orm.Get<Teacher>("t.0");

            Console.WriteLine();
            Console.WriteLine("Salary for " + t.FirstName + " " + t.Name + " is " + t.Salary.ToString() + " Pesos.");

            Console.WriteLine("Give rise of 12000.");
            t.Salary += 12000;

            Console.WriteLine("Salary for " + t.FirstName + " " + t.Name + " is now " + t.Salary.ToString() + " Pesos.");

            Orm.Save(t);

            Console.WriteLine("\n");
        }
    }
}
