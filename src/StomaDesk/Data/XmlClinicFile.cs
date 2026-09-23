using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace StomaDesk.Data
{
    /// <summary>Reads and writes <see cref="ClinicData"/> as an XML file.</summary>
    internal static class XmlClinicFile
    {
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(ClinicData));

        public static ClinicData Load(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                return (ClinicData)Serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// Writes to a temporary file first and swaps it in only when complete, so a crash or a full disk
        /// in the middle of a save never leaves a half-written data file. The previous version stays as .bak.
        /// </summary>
        public static void Save(string path, ClinicData data)
        {
            string folder = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(folder))
                Directory.CreateDirectory(folder);

            string temp = path + ".tmp";
            var settings = new XmlWriterSettings { Indent = true, Encoding = new UTF8Encoding(false) };
            using (XmlWriter writer = XmlWriter.Create(temp, settings))
            {
                Serializer.Serialize(writer, data);
            }

            if (File.Exists(path))
            {
                string backup = path + ".bak";
                if (File.Exists(backup))
                    File.Delete(backup);
                File.Replace(temp, path, backup);
            }
            else
            {
                File.Move(temp, path);
            }
        }
    }
}
