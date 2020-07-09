using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Utils
{
    public class XMLManager : TextfileManager, IDisposable
    {
        private XDocument Doc;

        public string Filename { get; private set; }

        public XMLManager(string filename) : base(filename)
        {
            Filename = filename;
            if (IsNotExist(filename))
            {
                New(Filename);
            }
            Load();
        }
        public override void Create()
        {
            Create(Filename);
        }

        public new static void Create(string filename)
        {
            if (IsNotExist(filename))
            {
                New(filename);
            }
        }
        public XDocument Clear()
        {
            Clear(Filename);
            return Load();
        }
        public static void Clear(string filename)
        {
            New(filename);
        }

        private static void New(string filename)
        {
            var xml = new XDocument(new XDeclaration("1.0", "utf-8", "true"));
            var element = new XElement("Root");
            xml.Add(element);
            xml.Save(filename);
        }
        public bool Save()
        {
            try
            {
                Doc.Save(Filename);
                return true;
            }
            catch (Exception any)
            {
                // XDocment.Save may throw various exception.
                return false;
            }
        }
        public XDocument Load()
        {
            if (IsNotExist()) { throw new FileNotFoundException(Filename, Filename); }
            try
            {
                Doc = XDocument.Load(Filename);
            }
            catch (XmlException)
            {
                base.Delete();
                New(Filename);
                Load();
            }
            return Doc;
        }
        public static XElement NewElement(string tag, string value)
        {
            return new XElement(tag, value);
        }

        public void Add(XElement element)
        {
            Doc.Elements().Last().Add(element);
        }
        public void Update(XName itemName,string newValue)
        {
            var tmp = Deserialize();
            this.Clear();

            foreach (var x in tmp)
            {
                if (x.Name == itemName) { Add(x); }
                else { Add(new XElement(itemName, newValue)); }
            }
        }

        public Dictionary<XName, string> DeserializeToDictionary()
        {
            var dest = new Dictionary<XName, string>();
            foreach (var element in DeserializeToTuple())
            {
                dest.Add(element.Item1, element.Item2);
            }
            return dest;
        }

        public List<Tuple<XName, string>> DeserializeToTuple()
        {
            return (Deserialize().Select(element => new Tuple<XName, string>(element.Name, element.Value))).ToList();
        }
        public List<XElement> Deserialize()
        {
            return Doc.Descendants().Elements().ToList();
        }
        public List<string> Deserialize(XName selectTag)
        {
            if (selectTag is null) { throw new ArgumentNullException(nameof(selectTag)); }
            return (Deserialize().Where(element => element.Name == selectTag).Select(element => element.Value)).ToList();
        }

        public List<XName> Deserialize(string selectItem)
        {
            var XElements = Doc.Descendants().Elements();
            if (selectItem is null) { throw new ArgumentNullException(nameof(selectItem)); }
            return XElements.
                Where(element => element.Value == selectItem).
                Select(element => element.Name).ToList();
        }
        public List<XName> DeserializeTag()
        {
            return (Deserialize().Select(item => item.Name)).ToList();
        }
        public List<string> DeserializeValue()
        {
            return (Deserialize().Select(item => item.Value)).ToList();
        }
        public void Delete(XElement deleteItem)
        {
            if (deleteItem is null) { throw new ArgumentNullException(nameof(deleteItem)); }
            if (deleteItem.Name == "") { throw new ArgumentException(nameof(deleteItem)); }
            var doc = Doc;
            Clear();
            var cnt = 0;
            foreach (var item in doc.Descendants())
            {
                if (cnt == 0) { cnt++; continue; }
                if (item.Name == deleteItem.Name &&
                    item.Value == deleteItem.Value) { continue; }
                Add(item);
            }
        }
        public void Dispose()
        {
            Doc = null;
        }
    }
}
