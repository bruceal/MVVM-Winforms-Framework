namespace MVVMWinformsFramework.Inheritance
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Binding;
    using Newtonsoft.Json;

    public abstract partial class ABoundViewModel
    {
        protected Dictionary<String, List<CaptionData>> LoadCaptions()
        {
            var captionsFile = GetCaptionFilePath();

            if (!File.Exists(captionsFile))
            {
                var result = new Dictionary<String, List<CaptionData>>();
                SaveCaptions(result);
                return result;
            }

            using (StreamReader file = new StreamReader(captionsFile))
            using (JsonReader reader = new JsonTextReader(file))
            {
                JsonSerializer captionLoader = new JsonSerializer();
                return captionLoader.Deserialize<Dictionary<String, List<CaptionData>>>(reader);

            }
        }

        protected void SaveCaptions(Dictionary<String, List<CaptionData>> Captions)
        {
            using (StreamWriter file = new StreamWriter(GetCaptionFilePath()))
            using (JsonWriter writer = new JsonTextWriter(file))
            {
                JsonSerializer captionSaver = new JsonSerializer();
                captionSaver.Serialize(writer, Captions);
            }
        }

        protected void SetLanguage(CultureInfo Culture)
        {
            SetLanguage(Culture.Name);
        }

        protected void SetLanguage(String Language)
        {
            var languages = LoadCaptions();
            if (languages.ContainsKey(Language))
            {
                foreach (CaptionData captionData in languages[Language])
                {
                    var objectToSetCaption = GetDynamicMember(ViewAccess, captionData.FieldName) as object;
                    if (objectToSetCaption != null)
                    {
                        var objectType = objectToSetCaption.GetType();
                        var objectInfo = objectType.GetSetablePublicAttribute(captionData.CaptionProperty);
                        if (objectInfo != null)
                            objectInfo.SetValue(objectToSetCaption, captionData.Caption);
                    }
                }
            }
        }

        private String GetCaptionFilePath()
        {
            var classBinding = GetType().GetCustomAttributes().FirstOrDefault(a => a is ClassBindingAttribute);

            var attribute = classBinding as ClassBindingAttribute;
            if (attribute != null)
            {
                return $@"Language\{attribute.ViewType.Name}.captions.json";
            }

            return $@"Language\{GetType().Name}.captions.json";
        }

        protected class CaptionData
        {
            public String FieldName { get; set; }
            public String CaptionProperty { get; set; }
            public String Caption { get; set; }
        }
    }
}
