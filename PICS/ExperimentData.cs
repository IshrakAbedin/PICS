using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ExperimentDetail = System.Collections.Generic.Dictionary<string, string>;

namespace PICS
{
    public class ExperimentData
    {
        private string experimentDataPath;
        public int IterationCount { get; set; }
        public string SaveDir { get; set; }
        public List<ExperimentDetail> ExperimentList { get; set; }

        public ExperimentData()
        {
            this.experimentDataPath = PathUtility.GetAbsolutePath("./ExperimentData.json");
            IterationCount = 1;
            SaveDir = PathUtility.GetAbsolutePath("./Data/");
            PathUtility.CreateDirectoryIfDoesNotExist(SaveDir);
            ExperimentList = new List<ExperimentDetail>();
        }

        public ExperimentData(string experiementDataPath, bool load = false)
        {
            this.experimentDataPath = PathUtility.GetAbsolutePath(experiementDataPath);
            if (load)
            {
                LoadData();
            }
            else
            {
                IterationCount = 1;
                SaveDir = PathUtility.GetAbsolutePath("./Data/");
                PathUtility.CreateDirectoryIfDoesNotExist(SaveDir);
                ExperimentList = new List<ExperimentDetail>();
            }
        }

        public ExperimentData(string experiementDataPath, int iterationCount, string saveDir)
        {
            this.experimentDataPath = PathUtility.GetAbsolutePath(experiementDataPath);
            IterationCount = iterationCount;
            SaveDir = PathUtility.GetAbsolutePath(saveDir);
            PathUtility.CreateDirectoryIfDoesNotExist(SaveDir);
            ExperimentList = new List<ExperimentDetail>();
        }

        public void SaveData()
        {
            if (experimentDataPath != null)
            {
                string jsonString = JsonSerializer.Serialize(this, new JsonSerializerOptions
                {
                    WriteIndented = true,
                });
                File.WriteAllText(experimentDataPath, jsonString);
            }
            else
            {
                throw new DirectoryNotFoundException();
            }
        }

        public void LoadData()
        {
            if (experimentDataPath != null)
            {
                try
                {
                    string jsonString = File.ReadAllText(experimentDataPath);
                    ExperimentData temp = JsonSerializer.Deserialize<ExperimentData>(jsonString);

                    this.IterationCount = temp.IterationCount;
                    this.SaveDir = temp.SaveDir;
                    this.ExperimentList = temp.ExperimentList;
                }
                catch (Exception)
                {
                    SaveData();
                }
            }
            else
            {
                throw new DirectoryNotFoundException();
            }
        }

        public void AddExperimentDetail(ExperimentDetail experimentDetail)
        {
            ExperimentList.Add(experimentDetail);
        }

        public void SetExperimentDataPath(string path)
        {
            this.experimentDataPath = PathUtility.GetAbsolutePath(path);
        }
    }
}
