using System.Xml;
using System.Collections.Generic;
using System.Linq;

namespace AmbientSounds.Core
{
    public static class ProfileUtils
    {
        public static bool AddProfile(string name, double[] values)
        {
            // Decodifica la stringa xml salvata in memoria.
            XmlDocument document = new XmlDocument();
            document.LoadXml(Properties.Settings.Default.Profiles);
            XmlNode root = document.DocumentElement;

            // Controlla che non ci siano altri profili con lo stesso nome.
            foreach(XmlNode node in root.ChildNodes) {
                if (node.Attributes["name"].Value == name) {
                    return false;
                }
            }

            // Crea il nuovo nodo.
            XmlNode newNode = document.CreateElement("profile");
            XmlAttribute nameAttribute = document.CreateAttribute("name");
            nameAttribute.Value = name;
            XmlAttribute valuesAttribute = document.CreateAttribute("values");
            valuesAttribute.Value = string.Format("{0} {1} {2} {3} {4} {5}",
                values.Select(x => x.ToString()).ToArray());

            // Aggiunge il nuovo nodo.
            newNode.Attributes.Append(nameAttribute);
            newNode.Attributes.Append(valuesAttribute);
            root.AppendChild(newNode);

            // Salva tutti i profili in memoria.
            Properties.Settings.Default.Profiles = document.OuterXml;
            Properties.Settings.Default.Save();
            return true;
        }

        public static void RemoveProfile(string name)
        {
            // Decodifica la stringa xml salvata in memoria.
            XmlDocument document = new XmlDocument();
            document.LoadXml(Properties.Settings.Default.Profiles);
            XmlNode root = document.DocumentElement;

            // Rimuove il nodo dal documento.
            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.Attributes["name"].Value == name)
                {
                    root.RemoveChild(node);
                }
            }

            // Salva tutti i profili in memoria.
            Properties.Settings.Default.Profiles = document.OuterXml;
            Properties.Settings.Default.Save();
        }

        public static string[] GetProfiles()
        {
            // Decodifica la stringa xml salvata in memoria.
            XmlDocument document = new XmlDocument();
            document.LoadXml(Properties.Settings.Default.Profiles);
            XmlNode root = document.DocumentElement;

            // Recupera la lista dei profili.
            List<string> profiles = new List<string>();
            foreach (XmlNode node in root.ChildNodes)
            {
                profiles.Add(node.Attributes["name"].Value);
            }

            // Ritorna la lista dei profili.
            return profiles.ToArray();
        }

        public static double[] GetProfileValues(string name)
        {
            // Decodifica la stringa xml salvata in memoria.
            XmlDocument document = new XmlDocument();
            document.LoadXml(Properties.Settings.Default.Profiles);
            XmlNode root = document.DocumentElement;

            // Rimuove il nodo dal documento.
            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.Attributes["name"].Value == name)
                {
                    return node.Attributes["values"].Value.Split(' ')
                        .Select(double.Parse).ToArray();
                }
            }

            return null;
        }

        public static string GetSelectedProfile()
        {
            return Properties.Settings.Default.SelectedProfile;
        }

        public static void SetSelectedProfile(string name)
        {   
            Properties.Settings.Default.SelectedProfile = name;
            Properties.Settings.Default.Save();
        }
    }
}
