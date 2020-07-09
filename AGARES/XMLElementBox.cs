using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Utils
{
    public partial class XMLElementBox : Form
    {
        private readonly string XMLFilename;
        private readonly bool IsSaveOnDispose;
        private readonly List<Tuple<XName,string>> Elements;
        private readonly string ElementTag;
        public XMLElementBox(string xmlFilename,string elementTag)
        {
            InitializeComponent();
            IsSaveOnDispose = true;
            XMLFilename = xmlFilename;
            ElementTag = elementTag;
            Elements = new XMLManager(xmlFilename).DeserializeToTuple();
            var xElements = new XMLManager(xmlFilename).Deserialize();
            if (xElements is null) { return; }
            foreach(var item in xElements)
            {
                ElementBox.Items.Add(item.Value);
            }
        }

        public XMLElementBox(string xmlFilename,string elementTag,List<string> SelectTags)
        {
            InitializeComponent();
            XMLFilename = xmlFilename;
            ElementTag = elementTag;
            Elements = new XMLManager(xmlFilename).DeserializeToTuple();
            foreach (var selecttag in SelectTags)
            {
                var xElements = new XMLManager(xmlFilename).Deserialize((XName)selecttag);
                if (xElements is null) { return; }
                foreach (var item in xElements)
                {
                    ElementBox.Items.Add(item);
                }
            }
        }
        private void XMLElementBox_Load(object sender, EventArgs e)
        {
            MaximumSize = this.Size;
            MinimumSize = this.Size;
            this.Location = new Point(
                this.Owner.Location.X + (this.Owner.Width - this.Width) / 2,
                this.Owner.Location.Y + (this.Owner.Height - this.Height) / 2);
            Delete.Visible = false;
            if (!IsSaveOnDispose)
            {
                ItemforAdd.Visible = false;
                Add.Visible = false;
                Delete.Visible = false;
            }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            Elements.Add(new Tuple<XName, string>(ElementTag, ItemforAdd.Text));
            ElementBox.Items.Add(ItemforAdd.Text);
            ItemforAdd.Text = "";
        }
        private void Delete_Click(object sender, EventArgs e)
        {
            var x = ElementBox.Items[selectingIndex].ToString();
            Elements.Remove(new Tuple<XName, string>(ElementTag, x));
            ElementBox.Items.RemoveAt(selectingIndex);
            Delete.Visible = false;
        }

        private void XMLElementBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            using (var xmlManager = new XMLManager(XMLFilename))
            {
                xmlManager.Clear();
                foreach (var element in Elements)
                {
                    xmlManager.Add(new XElement(element.Item1,element.Item2));
                }
                xmlManager.Save();
            }
        }

        int selectingIndex;
        private void ElementBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(((ListBox)sender).SelectedIndex == -1) { return; }
            selectingIndex = ((ListBox)sender).SelectedIndex;
            Delete.Visible = true;
        }

    }
}
