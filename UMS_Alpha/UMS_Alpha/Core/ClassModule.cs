using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UMS_Alpha.Core.Academic.StudentModule;

namespace UMS_Alpha.Core.Academic
{
    public class ClassModule
    {
       public class Class
        {
            public string ClassTitle { get; set; }//
            public string ClassCode { get; set; }//
            public ClassBranch AssociatedBranch { get; set; }
            public List<Student> RegisteredStudents { get; set; }//
            public ClassType Type { get; set; }//
            public int Credit { get; set; }//
            public int RegisteredCount
            {
                get
                {
                    return RegisteredStudents.Count;
                }
            }
            public Class(string classTitle, string classCode,int credit, List<Student> StudentsList, ClassType type)
            {
                //Initalizer
                ClassTitle = classTitle;
                ClassCode = classCode;
                RegisteredStudents = StudentsList;
                Credit = credit;
                Type = type;
                var BranchClassifier = classCode.Substring(0, 3);
                AssociatedBranch = decodeBranch(BranchClassifier);
            }

            private ClassBranch decodeBranch(string branchClassifier)
            {
                switch(branchClassifier)
                {
                    case "EIE": return ClassBranch.EIE;
                    case "EEE": return ClassBranch.EEE;
                    case "ECE": return ClassBranch.ECE;
                    case "CSE": return ClassBranch.CSE;
                    case "MEE": return ClassBranch.MEE;
                    case "AEE": return ClassBranch.AEE;
                    case "CIE": return ClassBranch.CIE;
                    default: return ClassBranch.Others;
                }
            }
        }

        public enum ClassType
        {
            Core,
            SoftElective,
            Humanities,
            Elective,
            Miscellaneous
        }

        public enum ClassBranch
        {
            EIE, ECE, EEE, CSE, MEE, AEE, CIE, Others
        }
    }
}
