using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using ErrorManagement;
using StaDynLanguage.Utils;

namespace StaDynLanguage.Errors
{
    internal class StaDynErrorList
    {
        //Implements singleton
        private List<ValidationError> errorList = new List<ValidationError>();
        static StaDynErrorList instance = null;

        private StaDynErrorList()
        {
        }

        public static StaDynErrorList Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StaDynErrorList();
                }
                return instance;
            }
        }
        public void addError(
             ValidationErrorSeverity severity,
             ValidationErrorType errorType,
             string text,
             String file,
             int line,
             int column)
        {
            ValidationError error = new ValidationError(text, severity, errorType, file, line, column);

            this.errorList.Add(error);
        }

        public int errorCount()
        {
            return this.errorList.Count;
        }
        public List<ValidationError> GetErrors()
        {
            return this.errorList;
        }
        public void clearErrors()
        {
            this.errorList.Clear();
        }
        //public bool checkErrors(ITextBuffer source)
        //{

        //    int errorCount = ErrorManager.Instance.ErrorCount;

        //    if (errorCount > 0)
        //    {
        //        IError errorFound = null;
        //        Span errorSpan;
        //        int errorPosition = 0, errorLineNumber;
        //        string filePath = source.Properties.GetProperty<ITextDocument>(typeof(ITextDocument)).FilePath;

        //        for (int i = 0; i < errorCount; i++)
        //        {
        //            errorFound = ErrorManager.Instance.GetError(i);

        //            if (errorFound is ErrorAdapter)
        //            {

        //                ErrorAdapter error = (ErrorAdapter)errorFound;

        //                var textView = FileUtilities.Instance.GetIVsTextView(error.Location.FileName);

        //                if (filePath != error.Location.FileName) continue;

        //                var file = StaDynLanguage.StaDynAST.ProjectFileAST.Instance.getAstFile(error.Location.FileName);

        //                var textView = StaDynLanguage.Utils.FileUtilities.Instance.GetIVsTextView(file.FileName);

        //                errorLineNumber = error.Location.Line > 0 ? error.Location.Line - 1 : 0;

        //                int v;

        //                textView.GetNearestPosition(errorLineNumber, error.Location.Column, out errorPosition, out v);

        //                errorPosition = source.CurrentSnapshot.GetLineFromLineNumber(errorLineNumber).Start.Position + error.Location.Column;
        //                errorSpan = new Span(errorPosition, 3);
        //                this.addError(ValidationErrorSeverity.Error, ValidationErrorType.Semantic, error.Description, errorSpan);
        //            }
        //            else
        //                this.addError(ValidationErrorSeverity.Error, ValidationErrorType.Semantic, "Parse Error", new Span(0, 3));
        //        }
        //        return true;
        //    }
        //    return false;
        //}

        public void refreshErrorList()
        {
            int errorCount = ErrorManager.Instance.ErrorCount;

            if (errorCount > 0)
            {
                IError errorFound = null;
                int errorLineNumber;

                for (int i = 0; i < errorCount; i++)
                {
                    errorFound = ErrorManager.Instance.GetError(i);

                    if (errorFound is ErrorAdapter)
                    {

                        ErrorAdapter error = (ErrorAdapter)errorFound;

                        errorLineNumber = error.Location.Line > 0 ? error.Location.Line - 1 : 0;

                        this.addError(ValidationErrorSeverity.Error, ValidationErrorType.Semantic, error.Description, error.Location.FileName, errorLineNumber, error.Location.Column);
                    }
                    else
                        this.addError(ValidationErrorSeverity.Error, ValidationErrorType.Semantic, "Parse Error","", 0, 0);
                }
            }
        }
    }
}
