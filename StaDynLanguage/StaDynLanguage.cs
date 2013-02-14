using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;


namespace StaDynLanguage
{
   
    [Export(typeof(ITaggerProvider))]
    [ContentType("StaDynLanguage")]
    [TagType(typeof(ClassificationTag))]
    internal sealed class StaDynLanguageClassifierProvider : ITaggerProvider
    {

        [Export]
        [Name("StaDynLanguage")]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition StaDynContentType = null;

        [Export]
        [FileExtension(".stadyn")]
        [ContentType("StaDynLanguage")]
        internal static FileExtensionToContentTypeDefinition StaDynFileType = null;


        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

        [Import]
        internal IBufferTagAggregatorFactoryService aggregatorFactory = null;

        private StaDynLanguage.Utils.VSEventHandler control= new Utils.VSEventHandler();

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {

            ITagAggregator<StaDynTokenTag> StaDynTagAggregator =
                                            aggregatorFactory.CreateTagAggregator<StaDynTokenTag>(buffer);

            return new StaDynClassifier(buffer, StaDynTagAggregator, ClassificationTypeRegistry) as ITagger<T>;
        }
    }

    
}