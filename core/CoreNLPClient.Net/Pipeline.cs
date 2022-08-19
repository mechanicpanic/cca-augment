﻿// The code is based on the work of Antonio Miras: https://github.com/AMArostegui/CoreNLPClient.Net

/*
MIT License

Copyright (c) 2022 Antonio Miras

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

namespace CoreNLPClientDotNet
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class Pipeline
    {
        public static string GetLang(this string lang)
        {
            switch (lang.ToLower())
            {
                case Lang.Arabic:
                case Lang.ArabicShort:
                    return Lang.Arabic;
                case Lang.Chinese:
                case Lang.ChineseShort:
                    return Lang.Chinese;
                case Lang.English:
                case Lang.EnglishShort:
                    return Lang.English;
                case Lang.French:
                case Lang.FrenchShort:
                    return Lang.French;
                case Lang.German:
                case Lang.GermanShort:
                    return Lang.German;
                case Lang.Spanish:
                case Lang.SpanishShort:
                    return Lang.Spanish;
                default:
                    return string.Empty;
            }
        }

        public static bool IsLang(this string lang)
        {
            switch (lang.ToLower())
            {
                case Lang.Arabic:
                case Lang.ArabicShort:
                    return true;
                case Lang.Chinese:
                case Lang.ChineseShort:
                    return true;
                case Lang.English:
                case Lang.EnglishShort:
                    return true;
                case Lang.French:
                case Lang.FrenchShort:
                    return true;
                case Lang.German:
                case Lang.GermanShort:
                    return true;
                case Lang.Spanish:
                case Lang.SpanishShort:
                    return true;
                default:
                    return false;
            }
        }

        public static string GetLangDefaultAnnotators(this string lang)
        {
            switch (lang.ToLower())
            {
                case Lang.Arabic:
                case Lang.ArabicShort:
                    return "tokenize,ssplit,pos,parse";
                case Lang.Chinese:
                case Lang.ChineseShort:
                    return "tokenize,ssplit,pos,lemma,ner,parse,coref";
                case Lang.English:
                case Lang.EnglishShort:
                    return "tokenize,ssplit,pos,lemma,ner,depparse";
                case Lang.French:
                case Lang.FrenchShort:
                    return "tokenize,ssplit,pos,depparse";
                case Lang.German:
                case Lang.GermanShort:
                    return "tokenize,ssplit,pos,ner,parse";
                case Lang.Spanish:
                case Lang.SpanishShort:
                    return "tokenize,ssplit,pos,lemma,ner,depparse,kbp";
                default:
                    return string.Empty;
            }
        }

        public static JObject GetEnglishDefaultReqProperties()
        {
            var defaultEnProps = @"{\""annotators\"": \""tokenize,ssplit,pos,lemma,ner,depparse\"",
                \""tokenize.language\"": \""en\"",
                \""pos.model\"": \""edu/stanford/nlp/models/pos-tagger/english-left3words-distsim.tagger\"",
                \""ner.model\"": \""edu/stanford/nlp/models/ner/english.all.3class.distsim.crf.ser.gz,\""
                                 \""edu/stanford/nlp/models/ner/english.muc.7class.distsim.crf.ser.gz,\""
                                 \""edu/stanford/nlp/models/ner/english.conll.4class.distsim.crf.ser.gz\"",
                \""sutime.language\"": \""english\"",
                \""sutime.rules\"": \""edu/stanford/nlp/models/sutime/defs.sutime.txt,\""
                                    \""edu/stanford/nlp/models/sutime/english.sutime.txt,\""
                                    \""edu/stanford/nlp/models/sutime/english.holidays.sutime.txt\"",
                \""ner.applyNumericClassifiers\"": \""true\"",
                \""ner.useSUTime\"": \""true\"",

                \""ner.fine.regexner.mapping\"": \""ignorecase=true,validpospattern=^(NN|JJ).*,\""
                                                 \""edu/stanford/nlp/models/kbp/english/gazetteers/regexner_caseless.tab;\"",
                                                 \""edu/stanford/nlp/models/kbp/english/gazetteers/regexner_cased.tab\""
                \""ner.fine.regexner.noDefaultOverwriteLabels\"": \""CITY\"",
                \""ner.language\"": \""en\"",
                \""depparse.model\"": \""edu/stanford/nlp/models/parser/nndep/english_UD.gz\""
                }";

            return (JObject)JsonConvert.DeserializeObject(defaultEnProps);
        }

        public static class Lang
        {
            public const string Arabic = "arabic";
            public const string Chinese = "chinese";
            public const string English = "english";
            public const string French = "french";
            public const string German = "german";
            public const string Spanish = "spanish";

            public const string ArabicShort = "ar";
            public const string ChineseShort = "zh";
            public const string EnglishShort = "en";
            public const string FrenchShort = "fr";
            public const string GermanShort = "de";
            public const string SpanishShort = "es";
        }
    }
}
