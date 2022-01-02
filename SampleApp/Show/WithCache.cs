using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewOrMapper_if19b098;
using  SampleApp.School;



namespace  SampleApp.Show
{
    /// <summary>This show case demonstrates cache functionality.</summary>
    public static class WithCache
    {
        /// <summary>Implements the show case.</summary>
        public static void Show()
        {
            Console.WriteLine("(6) Cache demonstration");
            Console.WriteLine("-----------------------");

            Console.WriteLine("\rWithout cache:");
            _ShowInstances();

            Console.WriteLine("\rWith cache:");
            Orm.Cache = new DefaultCache();
            _ShowInstances();
        }


        /// <summary>Shows instances.</summary>
        private static void _ShowInstances()
        {
            for(int i = 0; i < 7; i++)
            {
                Teacher t = Orm.Get<Teacher>("t.0");
                Console.WriteLine("Object [" + t.ID + "] instance no: " + t.InstanceNumber.ToString());
            }
        }
    }
}
