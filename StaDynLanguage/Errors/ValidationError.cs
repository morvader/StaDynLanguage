using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;

namespace StaDynLanguage.Errors
{
    public enum ValidationErrorSeverity
    {
        Message,
        Warning,
        Error
    }

    public enum ValidationErrorType
    {
        Syntactic,
        Semantic
    }

    public class ValidationError
    {
        public string Description { get; private set; }
        public ValidationErrorSeverity Severity { get; private set; }
        public ValidationErrorType Type { get; private set; }
        public string File { get; private set; }
        public int Line { get; set; }
        public int Column { get; set; }

        public ValidationError(string description)
        {
           
            Description = description;
            Severity = ValidationErrorSeverity.Error;
            Type = ValidationErrorType.Syntactic;
        }

        public ValidationError(string description, ValidationErrorSeverity severity, ValidationErrorType type, string file,int line, int column)
            : this(description)
        {
            Severity = severity;
            Type = type;
            File = file;
            Line = line;
            Column = column;
        }
    }
}