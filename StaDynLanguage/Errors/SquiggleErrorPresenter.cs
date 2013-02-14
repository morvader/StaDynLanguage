using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Tagging;
using System.Timers;
using System.Diagnostics;
using StaDynLanguage.Utils;

namespace StaDynLanguage.Errors
{
    /// <summary>
    /// Shows errors in the error list
    /// </summary>
    class SquiggleErrorPresenter
    {
        private IWpfTextView textView;
        private StaDynErrorListProvider errorListProvider;
        private SimpleTagger<ErrorTag> squiggleTagger;

        private List<TrackingTagSpan<IErrorTag>> previousSquiggles;

        public SquiggleErrorPresenter(IWpfTextView textView, IErrorProviderFactory squiggleProviderFactory, IServiceProvider serviceProvider)
        {
            this.textView = textView;
            this.textView.TextBuffer.Changed += OnTextBufferChanged;
            //this.textView.Closed += new EventHandler(OnTextViewClosed);
            textView.GotAggregateFocus += new EventHandler(OnTextViewGotFocus);


            this.errorListProvider = new StaDynErrorListProvider();
            this.squiggleTagger = squiggleProviderFactory.GetErrorTagger(textView.TextBuffer);
            previousSquiggles = new List<TrackingTagSpan<IErrorTag>>();

            //CreateErrors();

        }

        void OnTextViewClosed(object sender, EventArgs e)
        {
            // when a text view is closed we want to remove the corresponding errors from the error list
            ClearErrors();
        }
        void OnTextViewGotFocus(object sender, EventArgs e)
        {
            // when a text view is closed we want to remove the corresponding errors from the error list
            CreateErrors();
        }

        void OnTextBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            // keep the list of errors updated every time the buffer changes
            CreateErrors();
        }

        private void ClearErrors()
        {
            previousSquiggles.ForEach(tag => squiggleTagger.RemoveTagSpans(t => tag.Span == t.Span));
            previousSquiggles.Clear();      
        }

        private void CreateErrors()
        {
            StaDynErrorList.Instance.refreshErrorList();
            var errors = StaDynErrorList.Instance.GetErrors();

            // remove any previously created errors to get a clean start
            ClearErrors();

            foreach (ValidationError error in errors)
            {
               string currentPath = FileUtilities.Instance.getFilePath(textView);
                
                if (currentPath == error.File)
                {
                    try
                    {
                        
                        //ITrackingSpan span = textView.TextSnapshot.CreateTrackingSpan(error.Span, SpanTrackingMode.EdgeNegative);
                        
                        bool writeSquiggle = true;

                        int startIndex = textView.TextBuffer.CurrentSnapshot.GetLineFromLineNumber(error.Line).Start.Position + error.Column;
                        int endIndex = textView.TextBuffer.CurrentSnapshot.GetLineFromLineNumber(error.Line).Start.Position + error.Column + 4;

                        SnapshotPoint point  = new SnapshotPoint(textView.TextSnapshot, startIndex +1);

                        Span span = new Span(startIndex, endIndex - startIndex);
                        ITrackingSpan trackingSpan = textView.TextSnapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeNegative);
                        SnapshotSpan snapSpan = new SnapshotSpan(textView.TextSnapshot, span);
                        var currentSquiggles = squiggleTagger.GetTaggedSpans(snapSpan);
                        if (currentSquiggles.Count() == 0)
                            writeSquiggle = true;
                        else
                        {
                            foreach (var item in currentSquiggles)
                            {
                                if ((string)item.Tag.ToolTipContent == error.Description)
                                {
                                    writeSquiggle = false;
                                    break;
                                }
                            }
                        }

                        if (writeSquiggle)
                        {
                            squiggleTagger.CreateTagSpan(trackingSpan, new ErrorTag("syntax error", error.Description));
                            previousSquiggles.Add(new TrackingTagSpan<IErrorTag>(trackingSpan, new ErrorTag("syntax error", error.Description)));
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }
    }
}