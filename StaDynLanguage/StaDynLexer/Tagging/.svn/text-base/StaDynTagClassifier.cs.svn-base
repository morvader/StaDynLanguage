using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace StaDynLanguage
{
    internal sealed class StaDynClassifier : ITagger<ClassificationTag>
    {
        ITextBuffer _buffer;
        ITagAggregator<StaDynTokenTag> _aggregator;
        IDictionary<StaDynTokenTypes, IClassificationType> _StaDynTypes;

        internal StaDynClassifier(ITextBuffer buffer,
                               ITagAggregator<StaDynTokenTag> StaDynTagAggregator,
                               IClassificationTypeRegistryService typeService)
        {
            _buffer = buffer;
            _aggregator = StaDynTagAggregator;
            _StaDynTypes = new Dictionary<StaDynTokenTypes, IClassificationType>();

            //Asing StaDynLanguageClassificationDefinition to each TokenType
            foreach (KeyValuePair<StaDynTokenTypes, string> type in StaDynTokenTypeResolver.Instance.getAllTypes())
            {
                _StaDynTypes.Add(type.Key, typeService.GetClassificationType(type.Value));
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add  {}
            remove  { }
        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {

            foreach (var tagSpan in this._aggregator.GetTags(spans))
            {
                var tagSpans = tagSpan.Span.GetSpans(spans[0].Snapshot);
                yield return
                    new TagSpan<ClassificationTag>(tagSpans[0],
                                                   new ClassificationTag(_StaDynTypes[tagSpan.Tag.type]));
                
            }
        }


    }
}
