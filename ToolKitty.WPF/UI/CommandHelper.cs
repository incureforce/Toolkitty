using System;
using System.Globalization;
using System.Linq;
using System.Windows.Media;

namespace System.Windows.Input
{
    public static class CommandHelper
    {
        public static UICommand CreateCommand(Enum code, UICommand.ExecuteHandler executeHandler, UICommand.EnabledHandler enabledHandler = null)
        {
            var description = Translator.GetTranslation(code, UICommandIntent.Description);
            var displayName = Translator.GetTranslation(code, UICommandIntent.DisplayName);

            return new UICommand(executeHandler, enabledHandler) {
                Header = displayName,
                Description = description,
                Icon = GlyphIcons.GetIconModel(code),
            };
        }
    }
}
