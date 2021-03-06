﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace UMS_Alpha.Core.Academic
{
    public class StudentModule
    {
        public class Student
        {
            public string RollNo { get; set; }
            public int NumberInClass { get; set; }
            public string Name { get; set; }
            public int YearJoined { get; set; }
            public EnggBranchType EnggBranch { get; set; }
            public DegreeType DegreeType { get; set; }
            public int Section { get; set; }
            public List<ClassModule.Class> RegisteredClasses { get; set; }
            public int ClassesCount
            {
                get
                {
                    return RegisteredClasses.Count;
                }
            }
            public Student(string name, string rollNo)
            {

            }
            

            private EnggBranchType decodeBranch(string branchClassifier)
            {
                switch (branchClassifier)
                {
                    case "EIE": return EnggBranchType.EIE;
                    case "EEE": return EnggBranchType.EEE;
                    case "ECE": return EnggBranchType.ECE;
                    case "CSE": return EnggBranchType.CSE;
                    case "MEE": return EnggBranchType.MEE;
                    case "AEE": return EnggBranchType.AEE;
                    case "CIE": return EnggBranchType.CIE;
                    default: return EnggBranchType.Others;
                }
            }
        }

        public enum DegreeType
        {
            Engineering,
            Science,
            Business,
            Communication,
            SocialWork
        }

        public enum EnggBranchType
        {
            EIE, ECE, EEE, CSE, MEE, AEE, CIE, Others
        }
    }
}
