////////////////////////////////////////////////////////////////////////////////
// -------------------------------------------------------------------------- //
// Project rROTOR                                                             //
// -------------------------------------------------------------------------- //
// File: Parse.cs                                                             //
// Author: Francisco Ortin  -  francisco.ortin@gmail.com                    //
// Description:                                                               //
//    Refactor parsing a set of source files.                        //
// -------------------------------------------------------------------------- //
// Create date: 04-04-2007                                                    //
// Modification date: 04-04-2007                                              //
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

using RecognizerTest;
using ErrorManagement;

using System.IO;

namespace Tests {
    /// <summary>
    /// Parsing of a set of files
    /// </summary>
    public class Parse {
        /// <summary>
        /// Compiles a project
        /// </summary>
        /// <param name="files">The list of file names (including subdirectory)</param>
        /// <param name="outputFileName">The output file name. A null value means no executable generation.</param>
        /// <param name="target">The name of the target platform (clr, rrotor...). A null value means no executable generation.</param>
        /// <param name="run">If the program must be executed after compilation</param>
        public static void parse(string[] files, string outputFileName, string target, bool run) {
            if (files == null)
                return;
#if DEBUG
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Error.WriteLine("Compiling...");
            Console.ForegroundColor = previousColor;
            long startTime = DateTime.Now.Ticks;
#endif
            try {
                // if we have at least one command-line argument
                if (files.Length > 0) {
                    ErrorManager.Instance.ShowInConsole = true;
                    Program initApp = new Program();
                    // * filename : directoryname map
                    IDictionary<string, string> directories = new Dictionary<string, string>();

                    // for each directory/file specified on the command line
                    for (int i = 0; i < files.Length; i++) {
                        if ((File.Exists(files[i])) || (Directory.Exists(files[i])))
                            initApp.LoadFile(new FileInfo(files[i]), directories);
                        else {
                            ErrorManager.Instance.NotifyError(new FileNotFoundError(files[i]));
                            return;
                        }
                    }
                    // starts the compilation process
                    initApp.Run(directories, outputFileName, target, run);

                }
                else
                    ErrorManager.Instance.NotifyError(new CommandLineArgumentsError());
            } catch (System.Exception e) {
               Program.ClearMemory();
                Console.Error.WriteLine("Exception: " + e);
                Console.Error.WriteLine(e.StackTrace);
            }
#if DEBUG
            double elapsedTime = ((DateTime.Now.Ticks - startTime) / TimeSpan.TicksPerMillisecond) / 1000.0;
            previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            System.Console.Out.WriteLine("Total compilation time: {0} seconds.", elapsedTime);
            Console.ForegroundColor = previousColor;
#endif

        }

    }
}
 