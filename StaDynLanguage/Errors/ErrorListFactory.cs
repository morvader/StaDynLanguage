﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Shell;

namespace StaDynLanguage.Errors
{
	/// <summary>
	/// Adds the error list support when the view is created
	/// </summary>
	[Export(typeof(IWpfTextViewCreationListener))]
	[ContentType("StaDynLanguage")]
	[TextViewRole(PredefinedTextViewRoles.Document)]
	class ErrorListPresenterFactory : IWpfTextViewCreationListener
	{
		[Import]
		private IErrorProviderFactory SquiggleProviderFactory { get; set; }

		[Import(typeof(SVsServiceProvider))]
		private IServiceProvider ServiceProvider { get; set; }

        public void TextViewCreated(IWpfTextView textView)
        {
            // Add the error list support to the just created view
            textView.Properties.GetOrCreateSingletonProperty<SquiggleErrorPresenter>(() =>
                new SquiggleErrorPresenter(textView, SquiggleProviderFactory, ServiceProvider)
            );
        }
	}
}