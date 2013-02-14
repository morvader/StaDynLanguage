using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using StaDynLanguage;
using AST;
using StaDynLanguage.Utils;
using StaDynLanguage.StaDynAST;
using StaDynLanguage.Visitors;
using ErrorManagement;
using System.IO;
using TypeSystem;
using StaDynLanguage.TypeSystem;

namespace StaDynLanguage {

  [Export(typeof(IQuickInfoSourceProvider))]
  [ContentType("StaDynLanguage")]
  [Name("StaDyn Quick Info")]
  class StaDynQuickInfoSourceProvider: IQuickInfoSourceProvider {

    [Import]
    IBufferTagAggregatorFactoryService aggService = null;

    public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer) {
      return new StaDynQuickInfoSource(textBuffer, aggService.CreateTagAggregator<StaDynTokenTag>(textBuffer));
    }
  }

  class StaDynQuickInfoSource: IQuickInfoSource {
    private ITagAggregator<StaDynTokenTag> _aggregator;
    private ITextBuffer _buffer;
    private bool _disposed = false;


    public StaDynQuickInfoSource(ITextBuffer buffer, ITagAggregator<StaDynTokenTag> aggregator) {
      _aggregator = aggregator;
      _buffer = buffer;
    }

    public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan) {
      applicableToSpan = null;

      if (_disposed)
        throw new ObjectDisposedException("StaDynQuickInfoSource");

      var triggerPoint = (SnapshotPoint)session.GetTriggerPoint(_buffer.CurrentSnapshot);

      char c = triggerPoint.GetChar();

      if (triggerPoint == null || Char.IsWhiteSpace(c))
        return;

      //StaDynParser parser = new StaDynParser(session.TextView.TextBuffer, FileUtilities.Instance.getCurrentOpenDocumentFilePath());

      ////Parse source
      //StaDynSourceFileAST parseResult = parser.parseSource();
      ////parseResult = DecorateAST.Instance.completeDecorateAST(parseResult);
      ////parseResult= DecorateAST.Instance.completeDecorateAndUpdate(parseResult.FileName, true);
      //parseResult = DecorateAST.Instance.completeDecorateAndUpdate(parseResult);
      StaDynParser parser = new StaDynParser();
      parser.parseAll();
      StaDynSourceFileAST parseResult = ProjectFileAST.Instance.getAstFile(FileUtilities.Instance.getCurrentOpenDocumentFilePath());

      if (parseResult == null || parseResult.Ast == null)
        return;

      foreach (IMappingTagSpan<StaDynTokenTag> curTag in _aggregator.GetTags(new SnapshotSpan(triggerPoint, triggerPoint))) {
        var tagType = curTag.Tag.type;

        var tagSpan = curTag.Span.GetSpans(_buffer).First();
        applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(tagSpan, SpanTrackingMode.EdgeExclusive);

        string output = String.Empty;

        switch (tagType) {
          case StaDynTokenTypes.Text:
          case StaDynTokenTypes.Identifier:
          case StaDynTokenTypes.String:
          case StaDynTokenTypes.TypeDefinition:
          case StaDynTokenTypes.DynamicVar:
          case StaDynTokenTypes.Literal:
            //Gets the tag info
            output = this.getQuickInfo(curTag, parseResult);
            break;
          case StaDynTokenTypes.Operator:
          case StaDynTokenTypes.Delimiter:
          case StaDynTokenTypes.WhiteSpace:
            //Nothing to do
            continue;
          case StaDynTokenTypes.Keyword:
          case StaDynTokenTypes.LineComment:
          case StaDynTokenTypes.Comment:
          case StaDynTokenTypes.Number:
            if (curTag.Tag.StaDynToken.getText() == ".")
              continue;
            //Show the same tag
            output = curTag.Tag.StaDynToken.getText();
            break;

          default:
            break;
        }

        quickInfoContent.Add(output);

      }
    }

    private string getQuickInfo(IMappingTagSpan<StaDynTokenTag> curTag, StaDynSourceFileAST parseResult) {
      var tagSpan = curTag.Span.GetSpans(_buffer).First();

      int line = curTag.Tag.StaDynToken.getLine() + 1;
      //column = curTag.Tag.StaDynToken.getColumn() - curTag.Tag.StaDynToken.getText().Length + 1;
      int column = curTag.Tag.StaDynToken.getColumn();


      //foundNode = (AstNode)parseResult.Ast.Accept(new VisitorFindNode(), new Location(Path.GetFileName(parseResult.FileName), line, column));
      var foundNode = (AstNode)parseResult.Ast.Accept(new VisitorFindNode(), new Location(parseResult.FileName, line, column));

      //Gets the Node Type
      TypeExpression type = (TypeExpression)foundNode.AcceptOperation(new GetNodeTypeOperation(), null);

      //Show the token name
      string output = curTag.Tag.StaDynToken.getText() + ": ";

      if (type != null) {
        output += type.FullName;
        //output +=type.AcceptOperation(new GetTypeSystemName(), null) as string;
      }
      else
        output += foundNode.GetType().FullName;

      return output;
    }

    public void Dispose() {
      _disposed = true;
    }
  }
}

