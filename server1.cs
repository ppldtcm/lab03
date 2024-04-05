using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    static void Main(string[] args)
    {
        const string basePath = @"C:\Users\user\Documents\";



        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 8888);

        serverSocket.Bind(endPoint); // связываем с локальной точкой ipPoint
        serverSocket.Listen(1);
        Console.WriteLine("Server started...");

        Console.WriteLine("Waiting for connection...");
        Socket clientSocket = serverSocket.Accept();
        Console.WriteLine("Client connected.");

        bool flag = true;

        while (flag)
        {
            // Чтение данных
            byte[] buffer = new byte[256];
            int bytesRead = clientSocket.Receive(buffer);
            string toServer = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            string[] parts = toServer.Split(new string[] { "//" }, StringSplitOptions.None);
            string command = parts[0];

            switch(command)
            {
                case "1": //get
                    string fileName = parts[1];
                    string fullPath = Path.Combine(basePath, fileName);
                    if (File.Exists(fullPath))
                    {
                        string fileContent = File.ReadAllText(fullPath);

                        byte[] resultData = Encoding.ASCII.GetBytes($"The content of the file is: {fileContent}");
                        clientSocket.Send(resultData);
                    }
                    else
                    {
                        byte[] resultData = Encoding.ASCII.GetBytes($"The response says that the file was not found!");
                        clientSocket.Send(resultData);
                    }
                    break;


                case "2": //put
                    string fileName2 = parts[1];
                    string fileContent2 = parts[2];
                    string fullPath2 = Path.Combine(basePath, fileName2);
                    if (File.Exists(fullPath2))
                    {
                        byte[] resultData = Encoding.ASCII.GetBytes($"The response says that the file was not found!");
                        clientSocket.Send(resultData);
                    }
                    else
                    {
                        File.WriteAllText(fullPath2, fileContent2);
                        byte[] resultData = Encoding.ASCII.GetBytes($"The response says that file was created!");
                        clientSocket.Send(resultData);
                    }
                    break;

                case "3": //delete
                    string fileName3 = parts[1];
                    string fullPath3 = Path.Combine(basePath, fileName3);
                    if (File.Exists(fullPath3))
                    {
                        File.Delete(fullPath3);
                        byte[] resultData = Encoding.ASCII.GetBytes($"The response says that the file was successfully deleted!");
                        clientSocket.Send(resultData);
                    }
                    else
                    {
                        byte[] resultData = Encoding.ASCII.GetBytes($"The response says that the file was not found!");
                        clientSocket.Send(resultData);
                    }
                    break;
            
                case "exit":
                    flag = false;
                    break;
            }
        
        }




        clientSocket.Close();
        Console.WriteLine("Server stopped.");
    }
}

