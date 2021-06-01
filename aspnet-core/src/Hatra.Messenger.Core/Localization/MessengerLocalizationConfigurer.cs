using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace Hatra.Messenger.Localization
{
    public static class MessengerLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(MessengerConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(MessengerLocalizationConfigurer).GetAssembly(),
                        "Hatra.Messenger.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
