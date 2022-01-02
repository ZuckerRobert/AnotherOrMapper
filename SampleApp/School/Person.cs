using System;

using NewOrMapper_if19b098;
using NewOrMapper_if19b098.Attributes;

namespace SampleApp.School
{
    /// <summary>This is a person implementation (from School example).</summary>
    public abstract class Person
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // protected static members                                                                                         //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Instance number counter.</summary>
        protected static int _N = 1;



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // public properties                                                                                                //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Gets or sets the person ID.</summary>
        [PrimaryKey]
        public string ID { get; set; }


        /// <summary>Gets or sets the person's name.</summary>
        public string Name { get; set; }


        /// <summary>Gets or sets the person's first name.</summary>
        public string FirstName { get; set; }


        /// <summary>Gets or sets the person's birth date.</summary>
        [Field(ColumnName = "BDATE")]
        public DateTime BirthDate { get; set; }


        /// <summary>Gets or sets the person gender.</summary>
        public Gender Gender { get; set; }


        /// <summary>Gets the instance number.</summary>
        [Ignore]
        public int InstanceNumber { get; protected set; } = _N++;
    }



    /// <summary>This enumeration defines genders.</summary>
    public enum Gender: int
    {
        FEMALE = 0,
        MALE = 1
    }
}
