using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using System.Diagnostics;

using StaDyn.StaDynProject;


namespace StaDyn.StaDynBuildTasks {
	/// <summary>
	/// This task is responsible for the MSBuild Clean Target.
	/// </summary>

	public class StaDynCleanTask : Task {
#region Properties

				/// <summary>
				/// Directories to clean.
				/// </summary>
				/// <remarks>
				/// Absolute paths or relative to the project's filepath.
				/// </remarks>
				private string[] directories;
				[Required]
				public string[] Directories {

				    get {
						    return directories;
					    }

				    set {
						    directories = value;
					    }
			}

#endregion

#region Execute

				/// <summary>
				/// Recursively delete files inside the passed directories.
				/// Does not delete any directory.
				/// </summary>
				/// <returns>true if succesful.</returns>
				public override bool Execute() {
					if (directories.Length == 0)
						return false;

					string projectPath = ProjectConfiguration.Instance.GetActiveProjectFilePath();

					foreach (string dir in Directories) {
						DirectoryInfo directory;

						if (Path.IsPathRooted(dir))
							directory = new DirectoryInfo(dir);
						else
							directory = new DirectoryInfo(Path.Combine(projectPath, dir));

						deleteDir(directory);
					}

					//Program.ClearMemory();

					return true;
				}

#endregion

#region deleteDir

				private void deleteDir(DirectoryInfo directory) {
					if (directory.Exists) {
							foreach (DirectoryInfo subdir in directory.GetDirectories())
							deleteDir(subdir);
							foreach (FileInfo file in directory.GetFiles()) {
								try {
										file.Delete();

									} catch (Exception ex) {
										Trace.WriteLine("[StaDynCleanTask]: " + ex.Message);
										Log.LogWarning(
										    String.Empty, String.Empty, String.Empty, String.Empty, 0, 0, 0, 0,
										    "[StaDynCleanTask]: Could not delete file" + file.FullName,
										    null);
									}
							}
						}
				}

#endregion

		}
}
