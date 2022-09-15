using System.Collections.Generic;
using System.IO;
using Sifter.Tools;
using Sifter.Tools.Logger;
using TMPro;
using UnityEngine;

namespace Sifter.Import.CSV
{
    public class CSVUpdateFilesFromPersistentDataPath : MonoBehaviour
    {
        [SerializeField] TMP_Dropdown _dropdown;
        List<string> CSVFilePaths;


        void Start()
        {
            UpdateFiles();
            _dropdown.AddOptions(CSVFilePaths);
        }

        public void UpdateFiles()
        {
            CSVFilePaths = ES3.Load(PersistenceConstants.CSVFiles, new List<string>());
            var files = Directory.GetFiles(Application.persistentDataPath, "*.csv");
            foreach (var file in files)
            {
                if (CSVFilePaths.Contains(file)) return;

                CSVFilePaths.Add(file);
                SifterLog.Print($"Added csv file: {file}");
            }

            ES3.Save(PersistenceConstants.CSVFiles, CSVFilePaths);
        }
    }
}