using System;
using System.Collections.Generic;
using System.Text;

namespace Stuffort.Configuration
{
    public class ConfigurationType
    {
        public ConfigurationType() { }
        public ConfigurationType(string lang, bool darkm)
        {
            this.language = lang;
            this.darkMode = darkm;
        }

        private string language;
        public string Language
        {
            get { return language; }
            set { language = value; }
        }

        private bool darkMode;
        public bool DarkMode
        {
            get { return darkMode; }
            set { darkMode = value; }
        }
    }
}
