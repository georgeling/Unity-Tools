using UnityEngine;
using System.Collections;
using System.IO;

public class FileOption {

	/// <summary>
	/// 删除路径下所有文件
	/// </summary>
	/// <returns><c>true</c>, if path files was deleted, <c>false</c> otherwise.</returns>
	/// <param name="path">Path.</param>
	public bool DeletePathFiles(string path){
		try{
			foreach (string s in Directory.GetFileSystemEntries(path)) {
				if(File.Exists(s)){
					File.Delete(s);
				}else{
					DeletePathFiles(s);
					Directory.Delete(s);
				}
			}
		}catch(IOException e){
			Debug.Log(e.Message);
			return false;
		}

		return true;
	}

	/// <summary>
	/// 拷贝文件夹(及其子文件)
	/// </summary>
	/// <returns><c>true</c>, if folder was copyed, <c>false</c> otherwise.</returns>
	/// <param name="sourcePath">Source path.</param>
	/// <param name="destPath">Destination path.</param>
	public bool CopyFolder(string sourcePath, string desPath){
		if (!Directory.Exists (sourcePath)) {
			Debug.LogError(sourcePath + "not exist");
			return false;
		}
		return RecursiveCopy(sourcePath, desPath);
	}

	private bool RecursiveCopy(string sourcePath, string desPath){
		try{
			if(!Directory.Exists(desPath)){
				Directory.CreateDirectory(desPath);
			}
			
			// copy file
			DirectoryInfo sDir = new DirectoryInfo(sourcePath);
			foreach(var file in sDir.GetFiles()){
				file.CopyTo(desPath + "\\" + file.Name, true);
			}
			
			//copy follder
			DirectoryInfo[] subDir = sDir.GetDirectories();
			foreach(var dir in subDir){
				RecursiveCopy(dir.FullName, desPath + "//" + dir.Name);
			}
			
		}catch(IOException e){
			Debug.Log(e.Message);
			return false;
		}

		return true;
	}
}
