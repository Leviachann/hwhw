using System;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace hww1
{
    interface IAdapter
    {
        void Add(string filename, string name, string surname, string phone, string address);
        void Update(string filename, string name, string surname, string phone, string address);
        string Get(string filename);
    }

    class JsonAdapter : IAdapter
    {
        public void Add(string filename, string name, string surname, string phone, string address)
        {
            var data = new { Name = name, Surname = surname, Phone = phone, Address = address };
            string json = JsonSerializer.Serialize(data);
            File.WriteAllText(filename, json);
        }

        public string Get(string filename)
        {
            if (!File.Exists(filename))
            {
                return null;
            }

            string json = File.ReadAllText(filename);
            var data = JsonSerializer.Deserialize<dynamic>(json);
            return $"{data.Name} {data.Surname}\nPhone: {data.Phone}\nAddress: {data.Address}";
        }

        public void Update(string filename, string name, string surname, string phone, string address)
        {
            if (!File.Exists(filename))
            {
                return;
            }

            string json = File.ReadAllText(filename);
            var data = JsonSerializer.Deserialize<dynamic>(json);
            data.Name = name;
            data.Surname = surname;
            data.Phone = phone;
            data.Address = address;
            json = JsonSerializer.Serialize(data);
            File.WriteAllText(filename, json);
        }
    }

    class XmlAdapter : IAdapter
    {
        public void Add(string filename, string name, string surname, string phone, string address)
        {
            var data = new { Name = name, Surname = surname, Phone = phone, Address = address };
            var serializer = new XmlSerializer(typeof(object));
            using (var stream = new StreamWriter(filename))
            {
                serializer.Serialize(stream, data);
            }
        }

        public string Get(string filename)
        {
            if (!File.Exists(filename))
            {
                return null;
            }

            var serializer = new XmlSerializer(typeof(object));
            using (var stream = new StreamReader(filename))
            {
                var data = serializer.Deserialize(stream);
                dynamic obj = data;
                return $"{obj.Name} {obj.Surname}\nPhone: {obj.Phone}\nAddress: {obj.Address}";
            }
        }

        public void Update(string filename, string name, string surname, string phone, string address)
        {
            if (!File.Exists(filename))
            {
                return;
            }

            var serializer = new XmlSerializer(typeof(object));
            using (var stream = new StreamReader(filename))
            {
                var data = serializer.Deserialize(stream);
                dynamic obj = data;
                obj.Name = name;
                obj.Surname = surname;
                obj.Phone = phone;
                obj.Address = address;
                using (var writer = new StreamWriter(filename))
                {
                    serializer.Serialize(writer, data);
                }
            }
        }
    }

    public partial class MainWindow : Window
    {
        private IAdapter adapter;

        public MainWindow()
        {
            InitializeComponent();

            JSONBox.Checked += OnAdapterSelected;
            XMLBox.Checked += OnAdapterSelected;

            OnAdapterSelected(null, null);
        }

        private void OnAdapterSelected(object sender, RoutedEventArgs e)
        {
            if (JSONBox.IsChecked == true)
            {
                adapter = new JsonAdapter();
            }
            else if (XMLBox.IsChecked == true)
            {
                if (JSONBox.IsChecked == true)
                {
                    var jsonData = new
                    {
                        Name = nameInput.Text,
                        Surname = surnameInput.Text,
                        Phone = phoneInput.Text,
                        Address = addressInput.Text
                    };
                    var json = JsonSerializer.Serialize(jsonData);
                    File.WriteAllText($"{filename.Text}.json", json);
                }
                else if (XMLBox.IsChecked == true)
                {
                    var xmlData = new XElement("Data",
                    new XElement("Name", nameInput.Text),
                    new XElement("Surname", surnameInput.Text),
                    new XElement("Phone", phoneInput.Text),
                    new XElement("Address", addressInput.Text));
                    xmlData.Save($"{filename.Text}.xml");
                }
                if (JSONBox.IsChecked == true && File.Exists($"{filename.Text}.json"))
                {
                    var json = File.ReadAllText($"{filename.Text}.json");
                    var jsonData = JsonSerializer.Deserialize<dynamic>(json);
                    nameInput.Text = jsonData.Name;
                    surnameInput.Text = jsonData.Surname;
                    phoneInput.Text = jsonData.Phone;
                    addressInput.Text = jsonData.Address;
                }
                else if (XMLBox.IsChecked == true && File.Exists($"{filename.Text}.xml"))
                {
                    var xml = XDocument.Load($"{filename.Text}.xml");
                    var xmlData = xml.Element("Data");
                    nameInput.Text = xmlData.Element("Name").Value;
                    surnameInput.Text = xmlData.Element("Surname").Value;
                    phoneInput.Text = xmlData.Element("Phone").Value;
                    addressInput.Text = xmlData.Element("Address").Value;
                }
                if (JSONBox.IsChecked == true && File.Exists($"{filename.Text}.json"))
                {
                    var jsonData = new
                    {
                        Name = nameInput.Text,
                        Surname = surnameInput.Text,
                        Phone = phoneInput.Text,
                        Address = addressInput.Text
                    };
                    var json = JsonSerializer.Serialize(jsonData);
                    File.WriteAllText($"{filename.Text}.json", json);
                }
                else if (XMLBox.IsChecked == true && File.Exists($"{filename.Text}.xml"))
                {
                    var xmlData = new XElement("Data",
                    new XElement("Name", nameInput.Text),
                    new XElement("Surname", surnameInput.Text),
                    new XElement("Phone", phoneInput.Text),
                    new XElement("Address", addressInput.Text));
                    xmlData.Save($"{filename.Text}.xml");
                }

            }
        }
    }
}