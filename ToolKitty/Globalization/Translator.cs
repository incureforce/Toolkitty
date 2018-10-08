using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace System.Globalization
{
    public static class Translator
    {
        private static readonly Dictionary<Type, ResourceManager>
            cacheMap = new Dictionary<Type, ResourceManager>();

        public static string GetTranslation(Enum code, object intent = null, bool throwOnMissing = false)
        {
            var codeType = code.GetType();
            var codeTypeField = codeType.GetField(code.ToString());

            return GetTranslation(codeTypeField, intent, throwOnMissing);
        }

        public static string GetTranslation(ICustomAttributeProvider attributeProvider, object intent = null, bool throwOnMissing = false)
        {
            var attributes = attributeProvider
                .GetCustomAttributes(typeof(TranslationKeyAttribute), true)
                .OfType<TranslationKeyAttribute>();

            if (attributes.FirstOrDefault(x => Equals(x.Intent, intent ?? x.Intent)) is TranslationKeyAttribute attribute) {
                var resourceType = attribute.Type;

                if (cacheMap.TryGetValue(resourceType, out var resourceManager) == false) {
                    cacheMap[resourceType] = resourceManager = new ResourceManager(resourceType);
                }

                return resourceManager.GetString(attribute.Identifier);
            }

            if (throwOnMissing) {
                throw new KeyNotFoundException($"No TranslationKey attribute found on Element '{attributeProvider}' with intent » {intent} «");
            }

            return null;
        }
    }
}
