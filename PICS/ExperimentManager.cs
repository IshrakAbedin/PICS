using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExperimentDetail = System.Collections.Generic.Dictionary<string, string>;

namespace PICS
{
    public class ExperimentManager
    {
        private readonly string experimentDataPath;
        private readonly ExperimentData eData;

        public int ExperimentCount { get; private set; }
        public int IterationCount { get; private set; }
        public List<bool> DoneList { get; private set; }
        public string SaveDir { get; private set; }

        public int CurrentExperimentIndex { get; private set; }
        public int CurrentIterationCount { get; private set; }

        public string CurrentExperimentInfo => GetStringFromExperimentDetail(eData.ExperimentList[CurrentExperimentIndex]);
        public string CurrentExperimentSaveTag => eData.ExperimentList[CurrentExperimentIndex]["Save Tag"].ToString();
        public string ProgressString => $"{CurrentExperimentIndex + 1} / {ExperimentCount}";
        public float ProgressValue => ((float)(CurrentExperimentIndex + 1)) / ExperimentCount * 100;
        public bool InFirstExperiment => CurrentExperimentIndex == 0;
        public bool InLastExperiment => CurrentExperimentIndex + 1 == ExperimentCount;
        public bool IsCurrentOneDone => DoneList[CurrentExperimentIndex];

        public ExperimentManager(string experimentDataPath)
        {
            this.experimentDataPath = experimentDataPath;
            eData = new ExperimentData(experimentDataPath, true);

            ExperimentCount = eData.ExperimentList.Count;
            IterationCount = eData.IterationCount;
            DoneList = new List<bool>(ExperimentCount);
            for (int i = 0; i < ExperimentCount; i++)
            {
                DoneList.Add(false);
            }
            SaveDir = eData.SaveDir;

            CurrentExperimentIndex = 0;
            CurrentIterationCount = eData.IterationCount;
        }

        public void ResetExperiment()
        {
            for (int i = 0; i < DoneList.Count; i++)
            {
                DoneList[i] = false;
            }
            CurrentExperimentIndex = 0;
            CurrentIterationCount = eData.IterationCount;
        }

        public bool PreviousExperiment()
        {
            if(CurrentExperimentIndex == 0)
            {
                return false;
            }
            else
            {
                CurrentExperimentIndex--;
                CurrentIterationCount = IterationCount;
                return true;
            }
        }

        public bool NextExperiment()
        {
            if (CurrentExperimentIndex + 1 == ExperimentCount)
            {
                return false;
            }
            else
            {
                CurrentExperimentIndex++;
                CurrentIterationCount = IterationCount;
                return true;
            }
        }


        // Returns true if all trials are completed
        public bool CompleteSingleTrial()
        {
            if (CurrentIterationCount > 0)
            {
                CurrentIterationCount--;
                if (CurrentIterationCount == 0)
                {
                    DoneList[CurrentExperimentIndex] = true;
                    if (InLastExperiment)
                    {
                        return true;
                    }
                    else
                    {
                        CurrentIterationCount = IterationCount;
                        CurrentExperimentIndex++;
                    }
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        private string GetStringFromExperimentDetail(ExperimentDetail experimentDetail)
        {
            List<string> detailList = (from kvp in experimentDetail
                                       select $"{kvp.Key} : {kvp.Value}").ToList();
            return string.Join("\n", detailList);
        }
    }
}
