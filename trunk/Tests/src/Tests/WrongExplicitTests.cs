//////////////////////////////////////////////////////////////////////////////
// -------------------------------------------------------------------------- 
// Project rROTOR                                                            
// -------------------------------------------------------------------------- 
// File: WrongExplicitTests.cs                                                             
// Author: Francisco Ortin  -  francisco.ortin@gmail.com                    
// Description:                                                               
//    Testing of wrong programs without implicit type inference.
// -------------------------------------------------------------------------- 
// Create date: 12-04-2007                                                    
// Modification date: 12-04-2007                                              
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace Tests {
    class WrongExplicitTests : SemanticTest {
        public void testWrongBCLBuiltInTypes() {
            runTest(new string[] { "tests/wrong/testWrongExplicit.BCLBuiltInTypes.cs" });
        }
        
        public  void testWrongOverload() {
            runTest(new string[] { "tests/wrong/testWrongExplicit.Overload.cs" });
        }

        public  void testWrongBCL() {
            runTest(new string[] { "tests/wrong/testWrongExplicit.BCL.cs" });
        }
        
        public  void testWrongBase() {
            runTest(new string[] { "tests/wrong/testWrongExplicit.Base.cs" });
        }
        
        public  void testWrongTernary() {
            runTest(new string[] { "tests/wrong/testWrongExplicit.Ternary.cs" });
        }

        public void testWrongNamespacesAndMembers() {
            runTest(new string[] { "tests/wrong/testWrongExplicit.NamespaceAndStatic.cs" });
        }

        public void testWrongCast() {
            runTest(new string[] { "tests/wrong/testWrongExplicit.Cast.cs" });
        }
    }
}