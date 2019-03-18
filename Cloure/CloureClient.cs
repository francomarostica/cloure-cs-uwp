using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Cloure
{
    public class CloureClient
    {
        private int requestId = 0;
        private Socket clientSocket;

        private string host = "cloure.com";
        private int port = 2083;
        private string appToken = string.Empty;
        private string userToken = string.Empty;
        private string lang = "en";
        public int BuildVersion = 36;
        public string SubscriptionType { get; private set; } = "test_free";
        public JsonArray modulesGroupsArr { get; private set; }

        public string Name { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string GroupId { get; private set; }
        public string Group { get; private set; }
        public string PrimaryDomain { get; private set; }

        public delegate void OnConnectHandler();
        public delegate void OnDataReceivedHandler(string data);
        public delegate void OnLoginSuccessHandler();
        public delegate void OnLoginErrorHandler(string error, string errorType);
        public delegate void OnBroadcastMessageReceivedHandler(string message);

        public event OnConnectHandler OnConnect;
        public event OnDataReceivedHandler OnDataReceived;
        public event OnLoginSuccessHandler OnLoginSuccess;
        public event OnLoginErrorHandler OnLoginError;
        public event OnBroadcastMessageReceivedHandler OnBroadcastMessageReceived;

        public bool Connected
        {
            get
            {
                return clientSocket.Connected;
            }
        }

        //Don't remove the async! make problems on server
        public async void Connect()
        {
            string result = string.Empty;
            string responseString = string.Empty;

            logToFile("Trying to connect to CloureServer (" + host + ":" + port.ToString() + ")");
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(host, port);
                ReceiveData();

                var _onConnect = OnConnect;
                _onConnect?.Invoke();
            }
            catch (Exception ex)
            {
                logToFile("Trying to connect to CloureServer (" + host + ":" + port.ToString() + ")");
            }
        }

        private async void ReceiveData()
        {
            await Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        NetworkStream stream = new NetworkStream(clientSocket);
                        StringBuilder stringBuilder = new StringBuilder();
                        byte[] dataBuff = new byte[16384]; //8kb
                        int bytesRead = 0;
                        do
                        {
                            bytesRead = stream.Read(dataBuff, 0, dataBuff.Length);
                            stringBuilder.AppendFormat("{0}", Encoding.ASCII.GetString(dataBuff, 0, bytesRead));

                        } while (bytesRead > 0 && stream.DataAvailable);

                        logToFile("Received data: " + stringBuilder.ToString());

                        parseData(stringBuilder.ToString());
                    }
                    catch (Exception ex)
                    {
                        //se produce un error de desconexion
                    }
                }
            });
        }

        private void parseData(string data)
        {
            try
            {
                JsonObject jsonResponse = JsonObject.Parse(data);
                string topic = jsonResponse.GetNamedString("Topic");
                if (topic == "login_response")
                {
                    string error = jsonResponse.GetNamedString("Error");
                    if (error.Length > 0)
                    {
                        var evento = OnLoginError;
                        evento?.Invoke(error, "");
                    }
                    else
                    {
                        JsonObject jsonResponseObject = jsonResponse.GetNamedObject("Response");
                        appToken = jsonResponseObject.GetNamedString("app_token");
                        userToken = jsonResponseObject.GetNamedString("user_token");
                        SubscriptionType = jsonResponseObject.GetNamedString("account_type");
                        modulesGroupsArr = jsonResponseObject.GetNamedArray("modules_groups");
                        Name = jsonResponseObject.GetNamedString("name");
                        LastName = jsonResponseObject.GetNamedString("last_name");
                        Group = jsonResponseObject.GetNamedString("group");
                        Email = jsonResponseObject.GetNamedString("email");
                        PrimaryDomain = jsonResponseObject.GetNamedString("primary_domain");
                        /*
                        message = responseObject.GetNamedString("message");
                        */

                        CloureManager.SetAppToken(appToken);
                        CloureManager.SetUserToken(userToken);

                        var evento = OnLoginSuccess;
                        evento?.Invoke();
                    }
                }
                else if(topic == "broadcast_message")
                {
                    JsonObject jsonResponseObject = jsonResponse.GetNamedObject("Response");
                    string message = jsonResponseObject.GetNamedString("message");
                    var evento = OnBroadcastMessageReceived;
                    evento?.Invoke(message);
                }
                else
                {
                    var evento = OnDataReceived;
                    evento?.Invoke(data);
                }
            }
            catch (Exception ex)
            {
                //CloureManager.ShowDialog(ex.Message);
            }
        }


        public void SendData(string topic, string module = "", List<CloureParam> cloureParams = null)
        {
            string JsonRequest = string.Empty;
            requestId++;

            CloureParams cParams = new CloureParams();
            cParams.Add("topic", topic);
            cParams.Add("module", module);
            cParams.Add("request_id", requestId);
            cParams.Add("referer", "uwp");
            cParams.Add("referer_version", BuildVersion);
            cParams.Add("language", CloureManager.lang);

            if(cloureParams!=null)
            {
                foreach (CloureParam cloureParam in cloureParams)
                {
                    cParams.Add(cloureParam.name, cloureParam.value);
                }
            }

            JsonRequest = cParams.ToString();

            clientSocket.Send(Encoding.UTF8.GetBytes(JsonRequest));
        }

        protected async void logToFile(string message)
        {
            try
            {
                string logsFileName = "logs.txt";
                string content = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + message + "\n";

                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                if (await storageFolder.TryGetItemAsync(logsFileName) == null) await storageFolder.CreateFileAsync(logsFileName);

                StorageFile logFile = await storageFolder.GetFileAsync(logsFileName);

                await FileIO.AppendTextAsync(logFile, content, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
