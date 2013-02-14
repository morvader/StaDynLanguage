//////////////////////////////////////////////////////////////////////////////
// -------------------------------------------------------------------------- 
// Project rROTOR                                                            
// -------------------------------------------------------------------------- 
// File: Test.cs                                                             
// Author: Francisco Ortin  -  francisco.ortin@gmail.com                    
// Description:                                                               
//    Abstract class that generalizes all the tests.                                     
// -------------------------------------------------------------------------- 
// Create date: 01-06-2007                                                    
// Modification date: 01-06-2007                                              
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

using ErrorManagement;

namespace Tests {
    abstract class Test {

        #region Fields
        /// <summary>
        /// The two indexes of actual errors
        /// </summary>
        private int fromError, toError;

        /// <summary>
        /// The expected number of errors
        /// </summary>
        private int expectedErrors;

        /// <summary>
        /// The test has been run correctly
        /// </summary>
        private bool success;

        /// <summary>
        /// If the code generation phase will be executed. A false value implies only semantic test.
        /// </summary>
        private bool generateCode;

        /// <summary>
        /// Once the file is assembled, this boolean indicates it the executable file will be run.
        /// </summary>
        private bool run;

        /// <summary>
        /// The name of the target platform
        /// </summary>
        private string target;
        #endregion


        #region Properties
        /// <summary>
        /// The lower index of actual errors
        /// </summary>
        public int FromError {
            get { return this.fromError; }
            set { this.fromError = value; }
        }

        /// <summary>
        /// The upper index of actual errors
        /// </summary>
        public int ToError {
            get { return this.toError; }
            set { this.toError = value; }
        }

        /// <summary>
        /// The expected number of errors
        /// </summary>
        public int ExpectedErrors {
            get { return this.expectedErrors; }
            set { this.expectedErrors = value; }
        }

        /// <summary>
        /// The test has been run correctly
        /// </summary>
        public bool Success {
            get { return this.success; }
            set { this.success = value; }
        }

        /// <summary>
        /// If the code generation phase will be executed. A false value implies only semantic test.
        /// </summary>
        public bool GenerateCode {
            get { return this.generateCode; }
        }

        /// <summary>
        /// Once the file is assembled, this boolean indicates it the executable file will be run.
        /// </summary>
        public bool Run {
            get { return this.run; }
        }


        /// <summary>
        /// The name of the target platform
        /// </summary>
        public string Target {
            get { return this.target; }
        }
        #endregion

        #region Constructor
        public Test(bool generateCode, bool run, string target) {
            this.generateCode = generateCode;
            this.run = run;
            this.target = target;
        }
        #endregion

        #region runTest()
        /// <summary>
        /// Executes a test
        /// </summary>
        /// <param name="fileNames">The set of file names</param>
        /// <param name="outputFileName">The name of the output file name. Null implies no code generation.</param>
        protected void runTest(string[] fileNames, string outputFileName) {
            this.FromError = ErrorManager.Instance.ErrorCount;
            Parse.parse(fileNames, this.generateCode?outputFileName:null, this.generateCode?this.target:null, this.run);
            this.ToError = ErrorManager.Instance.ErrorCount;
            int expectedNumberOfErrors;
            this.Success = ErrorFile.CheckErrors(fileNames, this.FromError, this.ToError, out expectedNumberOfErrors);
            this.ExpectedErrors = expectedNumberOfErrors;
        }
        #endregion

   }
}
