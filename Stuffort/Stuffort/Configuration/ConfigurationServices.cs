using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;

namespace Stuffort.Configuration
{
    static public class ConfigurationServices
    {
        public static string FileName = "config.xml";
        public static string FilePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),FileName);
        static public void SaveConfigurationFile(ConfigurationType ct)
        {
            using (FileStream fs = new FileStream(FilePath, FileMode.Create))
            {
                XmlSerializer ser = new XmlSerializer(typeof(ConfigurationType));
                ser.Serialize(fs, new ConfigurationType(ct.Language, ct.NotificationEnabled));
            }
            
        }
        static public ConfigurationType GetConfigurationData()
        {
            ConfigurationType ct = new ConfigurationType();
            if (File.Exists(FilePath))
            {
                using (FileStream fs = new FileStream(FilePath, FileMode.Open))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(ConfigurationType));
                    ct = (ConfigurationType)ser.Deserialize(fs);
                }
                return ct;
            }
            else
            {
                SaveConfigurationFile(new ConfigurationType("undefined", false));
                return new ConfigurationType("undefined", false);
            }
        }

    }
}
