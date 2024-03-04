using System.IO;
using System;
using Sunday;
using UnityEditor.Android;
using UnityEngine;

public class UpdateGradleConfig : IPostGenerateGradleAndroidProject
{
    public int callbackOrder
    {
        get
        {
            return 999;
        }
    }

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        if (!Settings.Instance.detailedLogging)
        {
            Debug.Log("Detailed Logging is disabled");
            return;
        }
        
        string ApplicationPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
        string unityGradlefile = "";
        string ProjectGradleFile = "";

#if  UNITY_2021
        unityGradlefile =  ApplicationPath + "/" + "Library/Bee/Android/Prj/IL2CPP/Gradle/launcher"  + "/build.gradle";
        ProjectGradleFile =  ApplicationPath + "/" + "Library/Bee/Android/Prj/IL2CPP/Gradle" + "/build.gradle";
#elif UNITY_2020
        unityGradlefile =  ApplicationPath + "/" + "Temp/gradleOut/launcher"  + "/build.gradle";
        ProjectGradleFile =  ApplicationPath + "/" + "Temp/gradleOut" + "/build.gradle";
#endif
        
        if (!File.Exists(unityGradlefile))
        {
            Debug.LogError("build.gradle file not found - Path:" + unityGradlefile);
            throw new Exception("build.gradle file not found");
        }

        if (!File.Exists(ProjectGradleFile))
        {
            Debug.LogError("build.gradle file not found - Path:" + unityGradlefile);
            throw new Exception("build.gradle file not found");
        }

        // activate the gradle plugin in launcher project
        string text = File.ReadAllText(unityGradlefile);
        string insertPoint = "apply plugin: 'com.android.application'";
        int index = text.IndexOf(insertPoint) + insertPoint.Length;
        text = text.Insert(index, Environment.NewLine 
            + "apply plugin: 'ExtraPlugin'" + Environment.NewLine +
            "configurations.all {resolutionStrategy.cacheChangingModulesFor 0, 'seconds'}" + Environment.NewLine
            + "configurations.all {resolutionStrategy.cacheDynamicVersionsFor 0, 'seconds'}" + Environment.NewLine);

        File.WriteAllText(unityGradlefile, text);

        // activate the gradle plugin in main project
        var sr = new StreamReader(ProjectGradleFile);
        var sw = new StreamWriter(ProjectGradleFile+".temp");
        
        // remove the classpath config, to add later a new buildscript section that includes it.
        // classpath 'com.android.tools.build:gradle:4.0.1'
        // todo: proper error handling
        string line;
        string backupClathpath = null;
        while ((line = sr.ReadLine()) != null)
        {
            if(!line.Contains("com.android.tools.build:gradle")){
                sw.WriteLine(line);
            } else {
                backupClathpath = line;
            }
        }
        sr.Close();
        sw.Close();
        
        if (backupClathpath == null)
        {
            Debug.LogError("Couldn't find classpath section in " + ProjectGradleFile);
            throw new Exception("Gradle File has wrong format: " + ProjectGradleFile);
        } 

        // replace the gradle file with manipulated one
        File.Delete(ProjectGradleFile);
        File.Move(ProjectGradleFile+".temp", ProjectGradleFile);
        
        // append plugin 
        StreamWriter file2 = new StreamWriter(@ProjectGradleFile, append: true);
        file2.WriteLine(Environment.NewLine);
        file2.WriteLine("buildscript {");
        file2.WriteLine("   repositories {");
        file2.WriteLine("       maven {");
        file2.WriteLine("           url = 'https://7709c830-7c8e-11ed-a1eb-0242ac120002.s3.eu-central-1.amazonaws.com'");
        file2.WriteLine("       }");
        file2.WriteLine("       mavenCentral()");
        file2.WriteLine("       maven{url 'https://dl.google.com/dl/android/maven2'}");
        file2.WriteLine("   }");
        file2.WriteLine("   dependencies {");
        file2.WriteLine("       classpath group: 'com.extra.plugin', name:'ExtraPlugin', version:'latest.release', changing: true");
        file2.WriteLine("       "+backupClathpath);
        file2.WriteLine("   }");
        file2.WriteLine("}");
        file2.Close();
        Debug.Log("gradle Plugin successfully updated");
    }
}