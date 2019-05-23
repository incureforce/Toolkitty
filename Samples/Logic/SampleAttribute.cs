using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Logic
{
    public class SampleAttribute : Attribute
    {
        public string DisplayName {
            get => Translator.GetTranslation(GetType(), TranslationIntent.DisplayName);
        }

        public string Description {
            get => Translator.GetTranslation(GetType(), TranslationIntent.Description);
        }
    }
}
