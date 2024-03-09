using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using DG.Tweening.Core.Easing;

namespace SolarFalcon
{
    public class JSONDataService : IDataService
    {
        public T LoadData<T>(string RelativePath, bool Encrypted)
        {
            string path = Application.persistentDataPath + RelativePath;

            if (!File.Exists(path))
            {
                Debug.LogError($"Cannot find file at {path}. File does not exist.");
                throw new FileNotFoundException($"{path} does not exist.");
            }

            try
            {
                T data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
                return data;
            }

            catch (Exception e)
            {
                if (e is FileNotFoundException)
                {
                    Debug.Log("Save file not found. Creating new save!");
                    SaveData(RelativePath, GameManager.instance.GetCurrentSave(), false);
                    throw;
                }

                Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
                throw e;
            }
        }

        public bool SaveData<T>(string RelativePath, T Data, bool Encrypted)
        {
            string path = Application.persistentDataPath + RelativePath;

            try
            {
                if (File.Exists(path))
                {
                    Debug.Log("Data exists. Overwriting save data.");
                }

                else
                {
                    Debug.Log("Writing file for the first time!");
                }

                using FileStream stream = File.Create(path);
                stream.Close();
                File.WriteAllText(path, JsonConvert.SerializeObject(Data));
                return true;
            }

            catch (Exception e)
            {
                Debug.LogError($"Unable to save data due to: {e.Message} {e.StackTrace}");
                return false;
            }
        }
    }
}
