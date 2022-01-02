using System;
using System.Collections.Generic;

using NewOrMapper_if19b098;
using NewOrMapper_if19b098.Attributes;

namespace SampleApp.School
{
    /// <summary>This class represents a course in the school model.</summary>
    [Entity(TableName = "COURSES")]
    public class Course
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // public properties                                                                                                //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Gets or sets the course ID.</summary>
        [PrimaryKey]
        public string ID { get; set; }


        /// <summary>Gets or sets the course name.</summary>
        public string Name { get; set; }


        /// <summary>Gets or sets the course teacher.</summary>
        [ForeignKey(ColumnName = "KTEACHER")]
        public Teacher Teacher { get; set; }


        /// <summary>Gets or sets the students in this course.</summary>
        [ForeignKey(AssignmentTable = "STUDENT_COURSES", ColumnName = "KCOURSE", RemoteColumnName = "KSTUDENT")]
        public List<Student> Students { get; set; } = new List<Student>();
    }
}
