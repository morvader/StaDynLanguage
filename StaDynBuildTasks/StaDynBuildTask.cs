using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using RecognizerTest;
using TargetPlatforms;
using ErrorManagement;
using Microsoft.VisualStudio.Shell.Interop;
using DynVarManagement;
using StaDynLanguage_Project;
using StaDyn.StaDynProject;




namespace StaDynBuildTasks {
	/// <summary>
	/// Task responsible for the Build MSBuild target.
	/// This task calls the Inference compiler.
	/// </summary>

	public class StaDynBuildTask : Task {

#region Properties

				/// <summary>
				/// Files to compile. Usually, these are project items with BuildAction="Compile", which are marked
				/// in the ".sdproj" file as "Compile" elements.
				/// </summary>
				private string[] files;
				[Required]
				public string[] Files {

				    get {
						    return files;
					    }

				    set {
						    files = value;
					    }
			}

				/// <summary>
				/// Output filename. This is the name for the executable generated file.
				/// </summary>
				private string outputFileName;
				[Required]
				public string OutputFileName {

				    get {
						    return outputFileName;
					    }

				    set {
						    outputFileName = value;
					    }
			}

				/// <summary>
				/// Target platform for the compilation. This must be one of the <see cref="TargetPlatform"/> string values.
				/// </summary>
				private string target;
				[Required]
				public string Target
				{

				    get {
						    return target;
					    }

				    set {
						    target = value;
					    }
			}

				/// <summary>
				/// Option for var references dynamism management. This must be on of the <see cref="DynVarOption"/> string values.
				/// </summary>
				private string dynVarOption;
				[Required]
				public string DynVarOption
				{

				    get {
						    return dynVarOption;
					    }

				    set {
						    dynVarOption = value;
					    }
			}

#endregion

#region Execute

				/// <summary>
				/// Execute the Build task. This will call the Inference compiler passing the files obtained in the
				/// Files property of this task and other received arguments.
				/// </summary>
				/// <remarks>
				/// Error log is stored in: [OutputPath]\ErrorLog.txt
				/// Type table log is stored in: [OutputPath]\TypeTable.txt
				/// </remarks>
				/// <returns>true if succesful.</returns>
				public override bool Execute() {
					if (files.Length == 0)
						return false;

					string outputPath = ProjectConfiguration.Instance.GetProperty(PropertyTag.OutputPath.ToString());

					if (!Path.IsPathRooted(outputPath))
						outputPath = Path.Combine(ProjectConfiguration.Instance.GetActiveProjectFilePath(), outputPath);

					string debugFilesPath = outputPath;

					string typeTableFileName = Path.Combine(outputPath, Resources.TypeTable);

					//string ilasmFileName = Path.Combine(ProjectConfiguration.Instance.GetProperty("LibPath"), Resources.Ilasm);
					string ilasmFileName = Path.Combine(ProjectConfiguration.Instance.GetProperty("StaDynPath"), Resources.Ilasm);

					ErrorManager.Instance.LogFileName = Path.Combine(outputPath, Resources.ErrorLog);

					ErrorManager.Instance.Clear();

					StaDynLanguage.Errors.ErrorPresenter.Instance.ClearErrors();


					TargetPlatform targetPlatform = (TargetPlatform)Enum.Parse(typeof(TargetPlatform), Target);

					DynVarOption option = (DynVarOption)Enum.Parse(typeof(DynVarOption), DynVarOption);

					DynVarOptions.Instance.EverythingDynamic = option == StaDynLanguage_Project.DynVarOption.EverythingDynamic;

					DynVarOptions.Instance.EverythingStatic = option == StaDynLanguage_Project.DynVarOption.EverythingStatic;

					Program app = new Program();

					Program.ClearMemory();

					IDictionary<string, string> dictionary = new Dictionary<string, string>();


					string directory = Path.GetDirectoryName(outputPath);

					//req.FileName.Remove(req.FileName.Length - ast.Location.FileName.Length);

					IDictionary<string, string> name = new Dictionary<string, string>();

					//name.Add(outputPath, directory);

					//app.LoadFile(new FileInfo(ProjectConfiguration.Instance.GetActiveProjectFilePath()), dictionary);

					foreach (string file in Files) {
						var fileInfo = new FileInfo(file);
						app.LoadFile(fileInfo, dictionary);

						name.Add(fileInfo.FullName, directory);
					}

					string fileName = Path.GetFileName(outputFileName);
					string target = Path.Combine(outputPath, fileName);



					//app.Run(name, target + ".exe", debugFilesPath, ilasmFileName, typeTableFileName, targetPlatform, false);


					//app.Run(dictionary, OutputFileName + ".exe", debugFilesPath, typeTableFileName, ilasmFileName, targetPlatform, false);
					//app.Run(dictionary, null, debugFilesPath, typeTableFileName, null, targetPlatform, false);

					DirectoryInfo outputDir = new DirectoryInfo(outputPath);

					if (!outputDir.Exists)
						outputDir.Create();

					if (ErrorManager.Instance.ErrorFound) {
							handleErrors();
							return false;

						} else
						//app.Run(dictionary, OutputFileName + Resources.ExecutableExt, debugFilesPath, typeTableFileName, ilasmFileName, targetPlatform, false);
						app.Run(name, target + ".exe", debugFilesPath, ilasmFileName, typeTableFileName, targetPlatform, false);

					if (ErrorManager.Instance.ErrorFound) {
							handleErrors();
							return false;
						}


					return true;
				}

#endregion



#region handleErrors

				private void handleErrors() {
					StaDynLanguage.Errors.ErrorPresenter.Instance.createErrors();
					StaDynLanguage.Errors.ErrorPresenter.Instance.showErrorList();


					//for (int i = 0; i < ErrorManager.Instance.ErrorCount; i++)
					//{
					//    IError error = ErrorManager.Instance.GetError(i);
					//    ErrorAdapter errorAdapter = error as ErrorAdapter;
					//    if (errorAdapter != null)
					//    {
					//        int line = errorAdapter.Location.Line;
					//        int column = errorAdapter.Location.Column;
					//       // TextTools.InferenceToVs(ref line, ref column);

					//        Log.LogError(
					//            errorAdapter.ErrorType,
					//            String.Empty, String.Empty,
					//            errorAdapter.Location.FileName, line, column,
					//            0, 0,
					//            errorAdapter.Description,
					//            null);
					//    }
					//    else
					//    {
					//        Log.LogError(
					//            error.ErrorType,
					//            String.Empty, String.Empty,
					//            String.Empty, 0, 0,
					//            0, 0,
					//            error.Description,
					//            null);
					//    }
					//}
				}

#endregion

		}
}
