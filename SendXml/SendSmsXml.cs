using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SendXml
{
    public class SendSmsXml
    {
        /// <summary>
        /// Логин к серверу
        /// </summary>
        private string _LOGIN;

        /// <summary>
        /// Пароль к серверу
        /// </summary>
        private string _PASSWORD;

        /// <summary>
        /// Параметры сообщения
        /// </summary>
        public List<DataMessage> _Messages;

        /// <summary>
        /// Адерс сервера (IP)
        /// </summary>
        public string _ServerIP { get; set; }

        /// <summary>
        /// Номер порта
        /// </summary>
        public int _NumPort { get; set; }

        public SendSmsXml(string LOGIN, string PASSWORD)
        {
            _LOGIN = LOGIN;
            _PASSWORD = PASSWORD;
            if (_Messages == null) _Messages = new List<DataMessage>();
        }

        /// <summary>
        /// Отправка сообщений
        /// </summary>
        public void Send()
        {
            HttpWebResponse res=null;
            StreamReader sr=null;
            try
            {
                string xmlDoc = CreateXML();
                string url = $"{_ServerIP}:{_NumPort}";
                HttpWebRequest req = (HttpWebRequest) WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "text/xml;charset=utf-8";
                byte[] requestBytes = System.Text.Encoding.ASCII.GetBytes(xmlDoc);
                req.ContentLength = requestBytes.Length;
                Stream requestStream = req.GetRequestStream();
                requestStream.Write(requestBytes, 0, requestBytes.Length);
                requestStream.Close();

                res = (HttpWebResponse) req.GetResponse();
                sr = new StreamReader(res.GetResponseStream(), System.Text.Encoding.Default);
                string backstr = sr.ReadToEnd();
                GetSend(backstr);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                sr?.Close();
                res?.Close();
            }
        }

        /// <summary>
        /// Получение ответа от сервера после отправки сообщения
        /// </summary>
        /// <param name="backstr"></param>
        public string GetSend(string backstr)
        {
            if(string.IsNullOrEmpty(backstr)) return String.Empty;

            XmlDocument xDoc =new XmlDocument();
            xDoc.LoadXml(backstr);
            XmlElement xRoot = xDoc.DocumentElement;
            return "Все остальное :-)";
        }

        /// <summary>
        /// Создание XML и вывод его в формате string
        /// </summary>
        /// <returns></returns>
        private string CreateXML()
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            XmlElement rootElement = doc.CreateElement(string.Empty, "package", string.Empty);
            rootElement.SetAttribute("login", _LOGIN);
            rootElement.SetAttribute("password", _PASSWORD);
            doc.AppendChild(rootElement);

            XmlElement bodyElement = doc.CreateElement(string.Empty, "send", string.Empty);
            rootElement.AppendChild(bodyElement);

            if (_Messages.Count > 0)
            {
                int id = 1;
                foreach (var dataMessage in _Messages)
                {
                    XmlElement message = doc.CreateElement(string.Empty, "message", string.Empty);
                    message.SetAttribute("id", id.ToString());
                    message.SetAttribute("receiver", dataMessage._receiverPHONE);
                    message.SetAttribute("sender", dataMessage._sender);
                    XmlText nameText = doc.CreateTextNode(dataMessage._text);
                    message.AppendChild(nameText);
                    bodyElement.AppendChild(message);
                    id++;
                }
            }

            StringWriter sw = new StringWriter();
            XmlTextWriter tx = new XmlTextWriter(sw);
            doc.WriteTo(tx);
            string str = sw.ToString();
            sw.Close();
            tx.Close();

            return str;
        }

        /// <summary>
        /// Создание сообщения
        /// </summary>
        public void CreateMessage(string receiverPhone, string sender, string text)
        {
            _Messages?.Add(new DataMessage { _receiverPHONE = receiverPhone, _sender = sender, _text = text });
        }
    }

    /// <summary>
    /// Параметры сообщения
    /// </summary>
    public class DataMessage
    {
        /// <summary>
        /// номер телефона, на который отправляется сообщение
        /// </summary>
        public string _receiverPHONE { get; set; }

        /// <summary>
        /// подпись отправителя, высвечивающаяся вместо номера телефона
        /// </summary>
        public string _sender { get; set; }

        /// <summary>
        /// текст сообщения
        /// </summary>
        public string _text { get; set; }
    }
}
