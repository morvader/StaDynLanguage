﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using StaDynLanguage.Utils;
using StaDynLanguage.StaDynAST;
using AST;
using StaDynLanguage.Visitors;
using ErrorManagement;
using System.IO;
using TypeSystem;
using TypeSystem.Operations;

namespace StaDynLanguage.Intellisense.Signature
{

    internal class StaDynParameter : IParameter
    {
        public StaDynParameter(string documentation, Span locus, string name, ISignature signature)
        {
            Documentation = documentation;
            Locus = locus;
            Name = name;
            Signature = signature;
        }
        public string Documentation { get; private set; }
        public Span Locus { get; private set; }
        public string Name { get; private set; }
        public ISignature Signature { get; private set; }
        public Span PrettyPrintedLocus { get; private set; }

    }

    internal class StaDynSignature : ISignature
    {
        private ITextBuffer m_subjectBuffer;
        private IParameter m_currentParameter;
        private string m_content;
        private string m_documentation;
        private ITrackingSpan m_applicableToSpan;
        private ReadOnlyCollection<IParameter> m_parameters;
        private string m_printContent;

        internal StaDynSignature(ITextBuffer subjectBuffer, string content, string doc, ReadOnlyCollection<IParameter> parameters)
        {
            m_subjectBuffer = subjectBuffer;
            m_content = content;
            m_documentation = doc;
            m_parameters = parameters;
            m_subjectBuffer.Changed += new EventHandler<TextContentChangedEventArgs>(OnSubjectBufferChanged);
        }

        public event EventHandler<CurrentParameterChangedEventArgs> CurrentParameterChanged;

        public IParameter CurrentParameter
        {
            get { return m_currentParameter; }
            internal set
            {
                if (m_currentParameter != value)
                {
                    IParameter prevCurrentParameter = m_currentParameter;
                    m_currentParameter = value;
                    this.RaiseCurrentParameterChanged(prevCurrentParameter, m_currentParameter);
                }
            }
        }
        private void RaiseCurrentParameterChanged(IParameter prevCurrentParameter, IParameter newCurrentParameter)
        {
            EventHandler<CurrentParameterChangedEventArgs> tempHandler = this.CurrentParameterChanged;
            if (tempHandler != null)
            {
                tempHandler(this, new CurrentParameterChangedEventArgs(prevCurrentParameter, newCurrentParameter));
            }
        }
        internal void ComputeCurrentParameter()
        {
            if (Parameters.Count == 0)
            {
                this.CurrentParameter = null;
                return;
            }

            //the number of commas in the string is the index of the current parameter
            string sigText = ApplicableToSpan.GetText(m_subjectBuffer.CurrentSnapshot);

            int currentIndex = 0;
            int commaCount = 0;
            while (currentIndex < sigText.Length)
            {
                int commaIndex = sigText.IndexOf(',', currentIndex);
                if (commaIndex == -1)
                {
                    break;
                }
                commaCount++;
                currentIndex = commaIndex + 1;
            }

            if (commaCount < Parameters.Count)
            {
                this.CurrentParameter = Parameters[commaCount];
            }
            else
            {
                //too many commas, so use the last parameter as the current one.
                this.CurrentParameter = Parameters[Parameters.Count - 1];
            }
        }

        internal void OnSubjectBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            this.ComputeCurrentParameter();
        }

        public ITrackingSpan ApplicableToSpan
        {
            get { return (m_applicableToSpan); }
            internal set { m_applicableToSpan = value; }
        }

        public string Content
        {
            get { return (m_content); }
            internal set { m_content = value; }
        }

        public string Documentation
        {
            get { return (m_documentation); }
            internal set { m_documentation = value; }
        }

        public ReadOnlyCollection<IParameter> Parameters
        {
            get { return (m_parameters); }
            internal set { m_parameters = value; }
        }

        public string PrettyPrintedContent
        {
            get { return (m_printContent); }
            internal set { m_printContent = value; }
        }
    }

    internal class StaDynSignatureHelpSource : ISignatureHelpSource
    {
        private ITextBuffer m_textBuffer;

        public StaDynSignatureHelpSource(ITextBuffer textBuffer)
        {
            m_textBuffer = textBuffer;
        }
        public void AugmentSignatureHelpSession(ISignatureHelpSession session, IList<ISignature> signatures)
        {

            ITextSnapshot snapshot = m_textBuffer.CurrentSnapshot;
            int position = session.GetTriggerPoint(m_textBuffer).GetPosition(snapshot);

            ITrackingSpan applicableToSpan = m_textBuffer.CurrentSnapshot.CreateTrackingSpan(
             new Span(position, 0), SpanTrackingMode.EdgeInclusive, 0);

            //First try to get a compilable source code
            snapshot = snapshot.TextBuffer.Insert(position, "();");
            Span spanToRemove = new Span(position, 3);

            //StaDynParser parser = new StaDynParser(snapshot.TextBuffer, FileUtilities.Instance.getCurrentOpenDocumentFilePath());

            ////Parse source
            //StaDynSourceFileAST parseResult = parser.parseSource();

            //parseResult = DecorateAST.Instance.completeDecorateAndUpdate(parseResult);
            StaDynParser parser = new StaDynParser();
            parser.parseAll();
            StaDynSourceFileAST parseResult = ProjectFileAST.Instance.getAstFile(FileUtilities.Instance.getCurrentOpenDocumentFilePath());
            if (parseResult == null || parseResult.Ast == null)
            {
                snapshot = snapshot.TextBuffer.Delete(spanToRemove);
                return;
            }

            System.Drawing.Point caretPosition = FileUtilities.Instance.getCurrentCaretPoint();

            int column = caretPosition.Y;
            int line = caretPosition.X + 1;

            //AstNode foundNode = (AstNode)parseResult.Ast.Accept(new VisitorFindNode(), new Location(Path.GetFileName(parseResult.FileName), line, column));
            AstNode foundNode = (AstNode)parseResult.Ast.Accept(new VisitorFindNode(), new Location(parseResult.FileName, line, column));

            //Signature help is not needed on method definition
            if (foundNode is MethodDefinition)
            {
                snapshot = snapshot.TextBuffer.Delete(spanToRemove);
                return;
            }
            AstNode lastInvocationNode = StaDynIntellisenseHelper.Instance.getCurrentInvocation(parseResult, snapshot, position, line, column);
            //StaDynSourceFileAST file = ProjectFileAST.Instance.getAstFile(FileUtilities.Instance.getCurrentOpenDocumentFilePath());
            //if (file.Ast == null) return ;
            //AstNode foundNode = (AstNode)file.Ast.Accept(new VisitorFindNode(), new Location(Path.GetFileName(file.FileName), line, column));
            //Remove added chars
            snapshot = snapshot.TextBuffer.Delete(spanToRemove);

            if (lastInvocationNode == null)
            {
                return;
            }

            this.createSignatures(lastInvocationNode, m_textBuffer, applicableToSpan, signatures);

            /*****************************************/

            //signatures.Add(CreateSignature(m_textBuffer, "add(int firstInt, int secondInt)", "Documentation for adding integers.", applicableToSpan));
            //signatures.Add(CreateSignature(m_textBuffer, "add(double firstDouble, double secondDouble)", "Documentation for adding doubles.", applicableToSpan));

        }

        private void createSignatures(AstNode foundNode, ITextBuffer textBuffer, ITrackingSpan span, IList<ISignature> signatures)
        {
            MethodType methodType = null;
            IntersectionType intersectionType = null;

            if (foundNode is InvocationExpression)
            {
                InvocationExpression invocation = foundNode as InvocationExpression;
                //Not overloaded method:
                methodType = invocation.Identifier.ExpressionType as MethodType;
                //Overloaded method:
                intersectionType = invocation.Identifier.ExpressionType as IntersectionType;
            }

            else if (foundNode is NewExpression)
            {
                NewExpression newExp = foundNode as NewExpression;
                if (newExp.NewType != null)
                {
                    AccessModifier[] members = newExp.NewType.AcceptOperation(new GetMembersOperation(), null) as AccessModifier[];
                    string[] nameParts = newExp.NewType.FullName.Split(new char[] { '.' });
                    string className = nameParts[nameParts.Length - 1];
                    foreach (AccessModifier mod in members)
                    {
                        if (mod.MemberIdentifier.Equals(className))
                        {
                            //Not overloaded method:
                            methodType = mod.Type as MethodType;
                            //Overloaded method:
                            intersectionType = mod.Type as IntersectionType;

                            break;
                        }
                    }
                }
            }

            //Not overloaded method:
            if (methodType != null)
            {
                signatures.Add(this.CreateSignature(textBuffer, methodType, "", span));
            }

            //Overloaded method:
            if (intersectionType != null)
            {
                foreach (TypeExpression type in intersectionType.TypeSet)
                {
                    methodType = type as MethodType;

                    if (methodType != null)
                        signatures.Add(this.CreateSignature(textBuffer, methodType, "", span));
                }
            }
        }
        private StaDynSignature CreateSignature(ITextBuffer textBuffer, MethodType method, string methodDoc, ITrackingSpan span)
        {
            string name = method.MemberInfo.MemberIdentifier;
            string returnType = method.Return.FullName;
            string description = returnType + " " + name;

            string methodSig = description;

            StaDynSignature sig = new StaDynSignature(textBuffer, methodSig, methodDoc, null);
            textBuffer.Changed += new EventHandler<TextContentChangedEventArgs>(sig.OnSubjectBufferChanged);

            List<IParameter> paramList = new List<IParameter>();

            //Generate a correct signature
            methodSig += " ( ";

            int locusSearchStart = 0;
            if (method.ASTNode != null)
            {
                //ParametersInfo available (There is access to the definition of the method)
                foreach (Parameter param in method.ASTNode.ParametersInfo)
                {
                    string pName = param.Identifier;
                    string pDescription = param.ParamType + " " + pName;
                    methodSig += " " + pDescription + " ,";
                    int locusStart = methodSig.IndexOf(pDescription, locusSearchStart);

                    if (locusStart >= 0)
                    {
                        Span locus = new Span(locusStart, pDescription.Length);
                        locusSearchStart = locusStart + pDescription.Length;
                        paramList.Add(new StaDynParameter(pDescription, locus, pDescription, sig));
                    }
                }

            }
            else
            {
                //ParametersInfo not available (There is not access to the definition of the method)
                for (int i = 0; i < method.ParameterListCount; i++)
                {
                    string pDescription = method.GetParameter(i).FullName;
                    methodSig += " " + pDescription + " ,";
                    int locusStart = methodSig.IndexOf(pDescription, locusSearchStart);
                    if (locusStart >= 0)
                    {
                        Span locus = new Span(locusStart, pDescription.Length);
                        locusSearchStart = locusStart + pDescription.Length;
                        paramList.Add(new StaDynParameter(pDescription, locus, pDescription, sig));
                    }
                }
            }

            //Complete the method signature
            int lastComma=methodSig.LastIndexOf(',');
            if (lastComma >= 0)
                methodSig = methodSig.Remove(lastComma);

            methodSig += ")";

            //Update signature
            sig.Content = methodSig;
            sig.Parameters = new ReadOnlyCollection<IParameter>(paramList);
            sig.ApplicableToSpan = span;
            sig.ComputeCurrentParameter();
            return sig;

        }

        //private StaDynSignature CreateSignature(ITextBuffer textBuffer, string methodSig, string methodDoc, ITrackingSpan span)
        //{
        //    StaDynSignature sig = new StaDynSignature(textBuffer, methodSig, methodDoc, null);
        //    textBuffer.Changed += new EventHandler<TextContentChangedEventArgs>(sig.OnSubjectBufferChanged);

        //    //find the parameters in the method signature (expect methodname(one, two)
        //    string[] pars = methodSig.Split(new char[] { '(', ',', ')' });
        //    List<IParameter> paramList = new List<IParameter>();

        //    int locusSearchStart = 0;
        //    for (int i = 1; i < pars.Length; i++)
        //    {
        //        string param = pars[i].Trim();

        //        if (string.IsNullOrEmpty(param))
        //            continue;

        //        //find where this parameter is located in the method signature
        //        int locusStart = methodSig.IndexOf(param, locusSearchStart);
        //        if (locusStart >= 0)
        //        {
        //            Span locus = new Span(locusStart, param.Length);
        //            locusSearchStart = locusStart + param.Length;
        //            paramList.Add(new StaDynParameter("Documentation for the parameter.", locus, param, sig));
        //        }
        //    }

        //    sig.Parameters = new ReadOnlyCollection<IParameter>(paramList);
        //    sig.ApplicableToSpan = span;
        //    sig.ComputeCurrentParameter();
        //    return sig;
        //}

        public ISignature GetBestMatch(ISignatureHelpSession session)
        {
            if (session.Signatures.Count > 0)
            {
                ITrackingSpan applicableToSpan = session.Signatures[0].ApplicableToSpan;
                string text = applicableToSpan.GetText(applicableToSpan.TextBuffer.CurrentSnapshot);

                /******************************************************/

                if (text.Trim().Equals("add"))  //get only "add" 
                    return session.Signatures[0];
            }
            return null;
        }

        private bool m_isDisposed;
        public void Dispose()
        {
            if (!m_isDisposed)
            {
                GC.SuppressFinalize(this);
                m_isDisposed = true;
            }
        }

    }
}
