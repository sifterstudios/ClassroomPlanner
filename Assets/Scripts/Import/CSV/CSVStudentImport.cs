using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sifter.Degree;
using Sifter.Tools;
using Sifter.Tools.Logger;
using UnityEngine;

namespace Sifter.Import.CSV
{
    public class CSVStudentImport : MonoBehaviour
    {
        List<StudentStruct> _allStudents;

        void Start()
        {
            _allStudents = ES3.Load(PersistenceConstants.AllStudents, new List<StudentStruct>());
        }

        public void AddCsvToAllStudents(string path)
        {
            var _sheet = new ES3Spreadsheet();
            _sheet.Load(path);

            var newStudents = ParseStudentCSV(_sheet);
            CheckForDuplicates(ref newStudents, _allStudents);
            ES3.Save(PersistenceConstants.AllStudents, _allStudents);

            foreach (var student in newStudents)
                SifterLog.Print(JsonConvert.SerializeObject(student, Formatting.Indented));
        }

        static void CheckForDuplicates(ref List<StudentStruct> newStudents, List<StudentStruct> allStudents)
        {
            foreach (var student in from student in newStudents
                     from oldStudent in allStudents
                     where student.LastName == oldStudent.LastName && student.FirstName == oldStudent.FirstName
                     select student)
                newStudents.Remove(student);
        }

        List<StudentStruct> ParseStudentCSV(ES3Spreadsheet sheet)
        {
            var newStudents = new List<StudentStruct>();
            for (var row = 2; row < sheet.RowCount; row++)
            {
                var allData = sheet.GetCell<string>(0, row);
                var splitData = allData.Split(';');

                var newStudent = new StudentStruct
                {
                    FirstName = splitData[2],
                    LastName = splitData[3],
                    Email = splitData[4],
                    PhoneNumber = int.Parse(splitData[5]),
                    Degree = ParseStudentDegree(splitData[6])
                };

                // TODO: Find way to gather info about accessibility, or scrap the feature
                newStudents.Add(newStudent);
            }

            return newStudents;
        }

        DegreeEnum ParseStudentDegree(string degreeString)
        {
            if (degreeString.Contains("låtskriving"))
                return DegreeEnum.UML;
            if (degreeString.Contains("korledelse"))
                return DegreeEnum.UMK;
            if (degreeString.Contains("MML")) return DegreeEnum.MML;
            return DegreeEnum.NotFound;
            // TODO: Implement parsing of the missing degrees. Need more example students
        }
    }
}