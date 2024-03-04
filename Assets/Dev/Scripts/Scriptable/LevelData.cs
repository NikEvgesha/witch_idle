using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class LevelData : ScriptableObject
{
   public List<LevelMachinesData> data;


   [Button]
   public void ResetAndSave()
   {
      foreach (var levelData in data)
      {
         levelData.data.Clear();
      }
      ES3.Save("tutorial",false);
      ES3.Save("levelData",data);
   }
   [Button]
   public void Save()
   {
      
      ES3.Save("levelData",data);
   }

   public void Load()
   {
      if (ES3.KeyExists("levelData"))
      {
         data = ES3.Load("levelData") as List<LevelMachinesData>;

      }
   }
   public void MachineAction(MachinesData machineData)
   {
      
      foreach (var levelData in data[SceneManager.GetActiveScene().buildIndex-1].data)
      {
         if (levelData.position == machineData.position && levelData.type==machineData.type)
         {
            levelData.state = machineData.state;
            levelData.incomeLevel = machineData.incomeLevel;
            levelData.speedLevel = machineData.speedLevel;
            ES3.Save("levelData",data);

            return;
         }
      }
      
      data[SceneManager.GetActiveScene().buildIndex-1].data.Add(machineData);
      
      ES3.Save("levelData",data);
   }
   
   public void StackableMachineAction(MachinesData machineData)
   {
      
      foreach (var levelData in data[SceneManager.GetActiveScene().buildIndex-1].data)
      {
         if (levelData.position == machineData.position)
         {
            levelData.state = machineData.state;
            ES3.Save("levelData",data);

            return;
         }
      }
      
      data[SceneManager.GetActiveScene().buildIndex-1].data.Add(machineData);
      
      ES3.Save("levelData",data);
   }
   public MachinesData GetStackableMachineData(Vector3 position  )
   {
     
      foreach (var levelData in data[SceneManager.GetActiveScene().buildIndex-1].data)
      {
         if (levelData.position == position )
         {
            return levelData;
         }
      }

      return null;
   }
   public MachinesData GetMachineData(Vector3 position, WorkoutMachineTypes type)
   {
     
      foreach (var levelData in data[SceneManager.GetActiveScene().buildIndex-1].data)
      {
         if (levelData.position == position && levelData.type==type)
         {
            return levelData;
         }
      }

      return null;
   }
}
