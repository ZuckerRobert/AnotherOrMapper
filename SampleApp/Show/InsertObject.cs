using NewOrMapper_if19b098;
using SampleApp.School;
using System;



namespace SampleApp.Show
{
    /// <summary>This show case creates an object and saves it to database.</summary>
    public static class InsertObject
    {
        /// <summary>Implements the show case.</summary>
        public static void Show()
        {
            Console.WriteLine("(1) Insert object");
            Console.WriteLine("-----------------");

            Teacher t = new Teacher();

            t.ID = "t.0";
            t.FirstName = "Jerry";
            t.Name = "Mouse";
            t.Gender = Gender.MALE;
            t.BirthDate = new DateTime(1970, 8, 18);
            t.HireDate = new DateTime(2015, 6, 20);
            t.Salary = 50000;

            Orm.Save(t);

            Console.WriteLine("\n");
        }
    }
}
