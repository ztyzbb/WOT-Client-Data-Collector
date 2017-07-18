using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Packed_Section_Reader;
using Primitive_File_Reader;
using System.IO;
using System.Xml;

namespace wottoolslib
{
    public class XmlDecompiler
    {
        private XmlDecompiler() { }
        private static XmlDecompiler _instance = new XmlDecompiler();
        public static XmlDecompiler Instance
        {
            get
            {
                return _instance;
            }
        }

        public Packed_Section PackedSection = new Packed_Section();
        public Primitive_File PackedFile = new Primitive_File();
        public static bool isFileNameChanged;
        public string GetFileXml(string fileName)
        {
            isFileNameChanged = false;
            try
            {
                using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader reader = new BinaryReader(fileStream))
                    {
                        int head = reader.ReadInt32();
                        if (head == Packed_Section.Packed_Header)
                        {
                            return ReadPackedFileAsXml(reader, fileName);
                        }
                        else if (head == Primitive_File.Binary_Header)
                        {
                            return ReadPrimitiveFileAsXml(reader, fileName);
                        }
                        else
                        {
                            throw new Exception("File was not determined to be a packed or primitive file.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not open file '" + fileName + "' " + ex.Message);
            }
        }

        private string ReadPackedFileAsXml(BinaryReader reader, string fileName)
        {
            try
            {
                string file = Path.GetFileName(fileName).ToLower();
                if (file.First() <= '9' && file.First() >= '0')
                {
                    file = "_" + file;
                    isFileNameChanged = true;
                }
                reader.ReadSByte();
                List<string> dictionary = PackedSection.readDictionary(reader);
                if (dictionary.Any())
                {
                    XmlDocument xDoc = new XmlDocument();
                    XmlNode xmlroot = xDoc.CreateNode(XmlNodeType.Element, file, "");

                    PackedSection.readElement(reader, xmlroot, xDoc, dictionary);

                    xDoc.AppendChild(xmlroot);
                    return xDoc.OuterXml;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not read packed file '" + fileName + "' " + ex.Message);
            }

            return string.Empty;
        }

        private string ReadPrimitiveFileAsXml(BinaryReader reader, string fileName)
        {
            try
            {
                string file = Path.GetFileName(fileName).ToLower();
                if (file.First() <= '9' && file.First() >= '0')
                {
                    file = "_" + file;
                    isFileNameChanged = true;
                }
                XmlDocument xDoc = new XmlDocument();
                XmlNode xmlPrimitives = xDoc.CreateNode(XmlNodeType.Element, "primitives", "");
                PackedFile.ReadPrimitives(reader, xmlPrimitives, xDoc);
                xDoc.AppendChild(xmlPrimitives);
                return xDoc.OuterXml;
            }
            catch (Exception ex)
            {
                throw new Exception("Could not read primitive file '" + fileName + "' " + ex.Message);
            }
        }
    }
}
