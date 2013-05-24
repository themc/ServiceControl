namespace ServiceBus.Management.Lucene.Analyzers
{
    using System.IO;
    using global::Lucene.Net.Analysis;
    using global::Lucene.Net.Analysis.Standard;
    using global::Lucene.Net.Util;

    public class NGramAnalyzer : Analyzer
    {
        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            var tokenizer = new StandardTokenizer(Version.LUCENE_30, reader) { MaxTokenLength = 255 };
            TokenStream filter = new StandardFilter(tokenizer);
            filter = new LowerCaseFilter(filter);
            filter = new StopFilter(false, filter, StandardAnalyzer.STOP_WORDS_SET);
            return new NGramTokenFilter(filter, 2, 6);
        }
    }
}