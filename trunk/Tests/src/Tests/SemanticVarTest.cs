//////////////////////////////////////////////////////////////////////////////
// -------------------------------------------------------------------------- 
// Project rROTOR                                                            
// -------------------------------------------------------------------------- 
// File: VarTest.cs                                                             
// Author: Francisco Ortin  -  francisco.ortin@gmail.com                    
// Description:                                                               
//    Testing of free variables.                                     
// -------------------------------------------------------------------------- 
// Create date: 12-04-2007                                                    
// Modification date: 12-04-2007                                              
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

using ErrorManagement;

namespace Tests {
    class SemanticVarTest : SemanticTest {
        public void testVarRecursiveTypes() {
            runTest(new string[] { "tests/semantic/testVar.Recursion.cs" });
        }

        public void testVarCast() {
            runTest(new string[] { "tests/semantic/testVar.Cast.cs" });
        }

        public void testVarAttributes() {
            runTest(new string[] { "tests/semantic/testVar.Attributes.cs" });
        }

        public void testMemberAccessConstraints() {
            runTest(new string[] { "tests/semantic/testVar.MemberAccessConstraints.cs" });
        }

        public void testVarBracketConstraints() {
            runTest(new string[] { "tests/semantic/testVar.BracketConstraints.cs" });
        }

        public void testVarCommonOperatorConstraints() {
            runTest(new string[] { "tests/semantic/testVar.CommonOperatorConstraints.cs" });
        }
        
        public void testVarStaticUnionTypes() {
            runTest(new string[] { "tests/semantic/Figures.cs", "tests/semantic/testVar.StaticUnionTypes.cs" });
        }
        
        public void testVarAssignmentConstraint() {
            runTest(new string[] { "tests/semantic/testVar.AssignmentConstraint.cs" });
        }

        public void testVarParametersAndReturn() {
            runTest(new string[] { "tests/semantic/testVar.ParametersAndReturn.cs" });
        }
        
        public void testVarSSAsequential() {
            runTest(new string[] { "tests/semantic/testVar.SSA.sequential.cs" });
        }

        public void testVarSSAwhile() {
            runTest(new string[] { "tests/semantic/Figures.cs", "tests/semantic/testVar.SSA.while.cs" });
        }

        public void testVarSSAdo() {
            runTest(new string[] { "tests/semantic/Figures.cs", "tests/semantic/testVar.SSA.do.cs" });
        }

        public void testVarSSAfor() {
            runTest(new string[] { "tests/semantic/Figures.cs", "tests/semantic/testVar.SSA.for.cs" });
        }
        
        public void testVarSSAif() {
            runTest(new string[] { "tests/semantic/Figures.cs", "tests/semantic/VarWrap.cs", "tests/semantic/testVar.SSA.if.cs" });
        }

        public void testVarSSAswitch() {
            runTest(new string[] { "tests/semantic/VarWrap.cs", "tests/semantic/testVar.SSA.switch.cs" });
        }

        public void testVarSSAnested() {
            runTest(new string[] { "tests/semantic/Figures.cs", "tests/semantic/VarWrap.cs", "tests/semantic/testVar.SSA.nested.cs" });
        }
    }
}