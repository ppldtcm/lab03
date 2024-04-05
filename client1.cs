using System;
using System.Diagnostics.Metrics;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Client
{
    static void Main(string[] args)
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPAddress ip = IPAddress.Parse("127.0.0.1");
        IPEndPoint endPoint = new IPEndPoint(ip, 8888);
        Console.WriteLine("Connected to server.");
        socket.Connect(endPoint);

        bool flag = true;

        while(flag)
        { 
            Console.WriteLine("Enter action(1 - get a file, 2 - create a file, 3 - delete a file): ");
            string command = Console.ReadLine();

            string fileName = "";
            string fileContent = "";
            string toServer = "";


            switch (command)
            {
                case "1": //get
                    Console.WriteLine("Enter filename: ");
                    fileName = Console.ReadLine();
                    toServer = command + "//" + fileName;
                    break;

                case "2": //put
                    Console.WriteLine("Enter filename: ");
                    fileName = Console.ReadLine();

                    Console.WriteLine("Enter file content: ");
                    fileContent = Console.ReadLine();

                    toServer = command + "//" + fileName + "//" + fileContent;
                    break;

                case "3": //delete
                    Console.WriteLine("Enter filename: ");
                    fileName = Console.ReadLine();
                    toServer = command + "//" + fileName;
                    break;

                case "exit":
                    toServer = command + "//";
                    flag = false;
                    break;
            }

            // Отправка на сервер
            byte[] dataToSend = Encoding.ASCII.GetBytes(toServer);
            socket.Send(dataToSend);

            //Получение результат
            byte[] receivedData = new byte[256];
            int bytesRead = socket.Receive(receivedData);
            string result = Encoding.ASCII.GetString(receivedData, 0, bytesRead);
            Console.WriteLine(result);
        }




        // Закрытие соединения
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }
}

