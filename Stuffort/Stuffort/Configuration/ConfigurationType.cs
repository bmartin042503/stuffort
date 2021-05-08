using System;
using System.Collections.Generic;
using System.Text;

namespace Stuffort.Configuration
{
    public class ConfigurationType
    {
        public ConfigurationType() { }
        public ConfigurationType(string lang, bool notificationenabled)
        {
            this.language = lang;
            this.NotificationEnabled = notificationenabled;
        }

        private string language;
        public string Language
        {
            get { return language; }
            set { language = value; }
        }

        private bool notificationenabled;
        public bool NotificationEnabled
        {
            get { return notificationenabled; }
            set { notificationenabled = value; }
        }
    }
}
