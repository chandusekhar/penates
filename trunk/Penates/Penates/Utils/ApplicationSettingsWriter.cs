using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ApplicationSettingsWriter
{
    public class SettingsWriter
    {
        public string this[string key]
        {
            set
            {
                updateConfigurationFile(key, value);
            }
        }
        private void updateConfigurationFile(string key, string newValue)
        {
            lock (this)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                XmlNode settingsNode = doc.GetElementsByTagName("applicationSettings")[0];
                string applicationName = System.Reflection.Assembly.GetCallingAssembly().GetName().Name;

                XmlNode node = settingsNode.SelectSingleNode(String.Format(applicationName + ".Properties.Settings/setting[@name='{0}']", key));
                if (node != null)
                {
                    XmlNode valueNode = null;
                    valueNode=node.SelectSingleNode("value");
                    if (valueNode != null)
                    {
                        valueNode.FirstChild.Value = newValue;
                    }

                }
                doc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                Penates.Properties.Settings.Default.Save();
            }
        }

    }
}
