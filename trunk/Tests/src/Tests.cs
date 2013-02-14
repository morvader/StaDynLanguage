//////////////////////////////////////////////////////////////////////////////
// -------------------------------------------------------------------------- 
// Project rROTOR                                                             
// -------------------------------------------------------------------------- 
// File: Tests.cs                                                             
// Author: Francisco Ortin  -  francisco.ortin@gmail.com                    
// Description:                                                               
//    Encapsulates testing features with introspection.                        
// -------------------------------------------------------------------------- 
// Create date: 04-04-2007                                                    
// Modification date: 04-04-2007                                              
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace Tests {
    /// <summary>
    /// The main class to launch the tests
    /// </summary>
    class Tests {

        static Type[] testClasses = { 
            /**** Semantic tests *****/
            //typeof(UnificationTest), // * Deprecated
            
            typeof(SemanticSampleTest),

            typeof(SemanticDynamicsTest), 
            typeof(SemanticExplicitTest), 
            typeof(SemanticVarTest),
            
            typeof(WrongExplicitTests),
            typeof(WrongVarTests), 
            typeof(WrongDynamicsTests), 

            /**** Code Generation tests *****/
            typeof(CLRCGSampleTest),
            
            typeof(CLRCGTest),
            typeof(CLRCGVarTests), 
            typeof(CLRCGVarSemanticTests), 

            /**** Tutorial *****/
            typeof(GettingStartedTest),

            };

        static void Main() {
            IDictionary<string, int[]> errorsFound = new Dictionary<string, int[]>();
            int numberOfTests = 0;
            foreach (Type klass in testClasses)
                numberOfTests += test(klass, errorsFound);
            showErrors(errorsFound, numberOfTests);
        }

        /// <summary>
        /// To run tests by means of introspection.
        /// Executes all the methods that start with "test"
        /// </summary>
        /// <param name="klass">The class where testing methods are placed</param>
        /// <param name="errorsFound">The list of errors. Key=methodName, Value=[expectedErrors, errorsFound]</param>
        private static int test(Type klass, IDictionary<string, int[]> errorsFound) {
            object obj = klass.GetConstructor(new Type[0]).Invoke(null);
            MethodInfo[] methods = klass.GetMethods();
            int n = 0;
#if DEBUG
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("> Executing tests the '{0}' class:", klass.Name);
#endif
            for (int i = 0; i < methods.Length; i++)
                if (methods[i].Name.StartsWith("test")) {
#if DEBUG
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(">\tRunning the '{0}' method:", methods[i].Name);
                    Console.ForegroundColor = previousColor;
#endif
                    methods[i].Invoke(obj, null);
                    Test test = obj as Test;
                    if (!test.Success)
                        errorsFound[methods[i].Name] = new int[] { test.ExpectedErrors, test.ToError - test.FromError };
                    n++;
                }
#if DEBUG
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("> {0} tests of the '{1}' class executed successfully.\n", n, klass.Name);
            Console.ForegroundColor = previousColor;
#endif
            return n;
        }

        /// <summary>
        /// Shows the errors founded (if any)
        /// </summary>
        /// <param name="errorsFound">The list of errors. Key=methodName, Value=[expectedErrors, errorsFound]</param>
        /// <param name="numberOfTests">Number of tests</param>
        private static void showErrors(IDictionary<string, int[]> errorsFound, int numberOfTests) {
#if DEBUG
            Console.WriteLine();
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("> Successful tests: {0}.", numberOfTests - errorsFound.Count);
            Console.WriteLine("> Erroneous tests: {0}.", errorsFound.Count);
            Console.WriteLine("> Total executed tests: {0}.", numberOfTests);
            if (errorsFound.Count > 0) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("> Errors:");
                foreach (KeyValuePair<string, int[]> pair in errorsFound)
                    Console.WriteLine(">\t Method {0}: {1} errors expected, {2} found.", pair.Key, pair.Value[0], pair.Value[1]);
            }
            Console.ForegroundColor = previousColor;
            Console.WriteLine();
#endif
        }
    }
}
