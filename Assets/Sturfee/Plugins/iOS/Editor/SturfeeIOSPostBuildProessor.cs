using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;


public class SturfeeIOSPostBuildProessor 
{

	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
	{
		if (target == BuildTarget.iOS) {
			OnPostprocessBuildIOS (pathToBuiltProject);
		}
	}

	private static void OnPostprocessBuildIOS(string pathToBuiltProject)
	{
		// We use UnityEditor.iOS.Xcode API which only exists in iOS editor module
		#if UNITY_IOS
		string projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";

		UnityEditor.iOS.Xcode.PBXProject proj = new UnityEditor.iOS.Xcode.PBXProject();
		proj.ReadFromString(File.ReadAllText(projPath));
		proj.AddFrameworkToProject(proj.TargetGuidByName("Unity-iPhone"), "AssetsLibrary.framework", false);
        proj.AddFrameworkToProject(proj.TargetGuidByName("Unity-iPhone"), "Accelerate.framework", false);
        string target = proj.TargetGuidByName("Unity-iPhone");

		Directory.CreateDirectory(Path.Combine(pathToBuiltProject, "Libraries/Sturfee/Plugins/iOS/Shared"));

		string[] sturGLoaderfilesToCopy = new string[]
		{
			"PlatformBase.hpp",
			"UnityInterface.hpp",
		};

		string[] sharedFilesToCopy = new string[]
		{
			"SturgProcesser.hpp",
			"SFDataStream.hpp",
			"SFLogger.hpp",
		};

        string[] indiosFilesToCopy = new string[]
        {
            "IndiosUnityInterface.hpp",
            "Indios.hpp"
        };

		for(int i = 0 ; i < sturGLoaderfilesToCopy.Length ; ++i)
		{
			var srcPath = Path.Combine(Path.Combine(Application.dataPath, "Sturfee/Plugins/iOS/SturGLoader/Include"), sturGLoaderfilesToCopy[i]);
			var dstLocalPath = "Libraries/Sturfee/Plugins/iOS/SturGLoader/Include/" + sturGLoaderfilesToCopy[i];
			var dstPath = Path.Combine(pathToBuiltProject, dstLocalPath);
			File.Copy(srcPath, dstPath, true);
			proj.AddFileToBuild(target, proj.AddFile(dstLocalPath, dstLocalPath));
		}

		for(int i = 0 ; i < sharedFilesToCopy.Length ; ++i)
		{
			var srcPath = Path.Combine(Path.Combine(Application.dataPath, "Sturfee/Plugins/iOS/Shared"), sharedFilesToCopy[i]);
			var dstLocalPath = "Libraries/Sturfee/Plugins/iOS/Shared/" + sharedFilesToCopy[i];
			var dstPath = Path.Combine(pathToBuiltProject, dstLocalPath);
			File.Copy(srcPath, dstPath, true);
			proj.AddFileToBuild(target, proj.AddFile(dstLocalPath, dstLocalPath));
		}

        for (int i = 0; i < indiosFilesToCopy.Length; ++i)
        {
            var srcPath = Path.Combine(Path.Combine(Application.dataPath, "Sturfee/Plugins/iOS/Indios/Include"), indiosFilesToCopy[i]);
            var dstLocalPath = "Libraries/Sturfee/Plugins/iOS/Indios/Include/" + indiosFilesToCopy[i];
            var dstPath = Path.Combine(pathToBuiltProject, dstLocalPath);
            File.Copy(srcPath, dstPath, true);
            proj.AddFileToBuild(target, proj.AddFile(dstLocalPath, dstLocalPath));
        }

        File.WriteAllText(projPath, proj.WriteToString());
		#endif // #if UNITY_IOS
	}
}
