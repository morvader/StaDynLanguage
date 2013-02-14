using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;
using StaDynLanguage.Utils;
using Microsoft.VisualStudio.Text.Editor;

namespace StaDynLanguage.Errors
{
    [ContentType("StaDynLanguage")]
    public class ErrorPresenter
    {

        static ErrorPresenter instance = null;

        private ErrorPresenter()
        {
            previousErrors = new List<ErrorTask>();
            var DTEObj = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            ServiceProvider sp = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)DTEObj);
            errorList = new Microsoft.VisualStudio.Shell.ErrorListProvider(sp);
        }

        public static ErrorPresenter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ErrorPresenter();
                }
                return instance;
            }
        }

        Microsoft.VisualStudio.Shell.ErrorListProvider errorList;

        private List<ErrorTask> previousErrors;

        public void ClearErrors()
        {
            StaDynErrorList.Instance.clearErrors();

            //previousErrors.ForEach(task => errorList.Tasks.Remove(task));
            //previousErrors.Clear();
            var DTEObj = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;

          if( EnvDTE.vsBuildState.vsBuildStateInProgress!=DTEObj.Solution.SolutionBuild.BuildState){
            
            DTEObj.Solution.SolutionBuild.Clean(false);
          }

          errorList.Tasks.Clear();
          errorList.Refresh();

        }

        public void ClearErrorsAndRemoveSquiggles()
        {
            ClearErrors();
            SourceHelper.refreshHighlighting();
        }

        public void createErrors()
        {

            StaDynErrorList.Instance.refreshErrorList();
            var errors = StaDynErrorList.Instance.GetErrors();

            //// Check if we should update the error list based on the error count to avoid refreshing the list without changes
            //if (errors.Count != this.previousErrors.Count)
            //{
            // remove any previously created errors to get a clean start
            //ClearErrors();

            foreach (ValidationError error in errors)
            {
                // creates the instance that will be added to the Error List
                ErrorTask task = new ErrorTask();
                task.Category = TaskCategory.All;
                task.Priority = TaskPriority.Normal;
                task.Document = error.File;
                task.ErrorCategory = TranslateErrorCategory(error.Severity);
                task.Text = error.Description;
                task.Line = error.Line;
                task.Column = error.Column;
                task.Navigate += OnTaskNavigate;
                errorList.Tasks.Add(task);
                previousErrors.Add(task);
            }



            //}
        }

        public void showErrorList()
        {
            this.errorList.ResumeRefresh();
            this.errorList.Show();
            this.errorList.ForceShowErrors();
            //this.errorList.BringToFront();
        }
        private void OnTaskNavigate(object source, EventArgs e)
        {
            ErrorTask task = source as ErrorTask;

            if (task == null) return;
            FileUtilities.Instance.OpenDocument(task.Document);

            var errorView = StaDynLanguage.Utils.FileUtilities.Instance.GetIWpfTextView(task.Document);

            if (errorView != null)
            {
                // move the caret to position of the error
                errorView.Caret.MoveTo(new SnapshotPoint(errorView.TextSnapshot, errorView.TextSnapshot.GetLineFromLineNumber(task.Line).Start + task.Column));
                // set focus to make sure the error is visible to the user
                errorView.VisualElement.Focus();
                errorView.Caret.EnsureVisible();
                
                //this.createErrors();
            }
        }

        private TaskErrorCategory TranslateErrorCategory(ValidationErrorSeverity validationErrorSeverity)
        {
            switch (validationErrorSeverity)
            {
                case ValidationErrorSeverity.Error:
                    return TaskErrorCategory.Error;
                case ValidationErrorSeverity.Message:
                    return TaskErrorCategory.Message;
                case ValidationErrorSeverity.Warning:
                    return TaskErrorCategory.Warning;
            }

            return TaskErrorCategory.Error;
        }
    }
}
