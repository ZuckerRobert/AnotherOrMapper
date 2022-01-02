using System;
using System.Collections.Generic;

using NewOrMapper_if19b098;
using NewOrMapper_if19b098.Attributes;

namespace SampleApp.School
{
    /// <summary>This is a teacher implementation (from School example).</summary>
    [Entity(TableName = "TEACHERS")]
    public class Teacher: Person
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // public properties                                                                                                //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Gets or sets the teacher's salary.</summary>
        public int Salary { get; set; }


        /// <summary>Gets or sets the teacher's hire date.</summary>
        [Field(ColumnName = "HDATE")]
        public DateTime HireDate { get; set; }


        /// <summary>Gets the teacher's courses.</summary>
        [ForeignKey(ColumnName = "KTEACHER")]
        public List<Class> Classes { get; private set; } = new List<Class>();
    }
}
