using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        using var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync("127.0.0.1", 8888);
        var stream = tcpClient.GetStream();

        using (stream)
        using (BinaryWriter binaryWriter = new BinaryWriter(stream))
        using (BinaryReader binaryReader = new BinaryReader(stream))
        {

            bool flag = true;
            while (flag)
            {
                Console.WriteLine("Enter action(1 - get a file, 2 - save a file, 3 - delete a file): ");
                string command = Console.ReadLine() ?? string.Empty;

                string fileName = "";
                string fileNameServer = "";
                string fileId = "";
                string toServer = "";
                const string clientPath = @"C:\Users\user\Documents\c#\client";

                switch (command)
                {
                    case "1":

                        Console.WriteLine("Do you want to get the file by name or by id (1 - name, 2 - id): ");
                        string command1 = Console.ReadLine();

                        if (command1 == "1")
                        {
                            Console.WriteLine("Enter name: ");
                            fileName = Console.ReadLine();
                            toServer = command + "//" + command1 + "//" + fileName;
                        }
                        else if (command1 == "2")
                        {
                            Console.WriteLine("Enter id: ");
                            fileId = Console.ReadLine();
                            toServer = command + "//" + command1 + "//" + fileId;
                        }

                        Console.WriteLine("Enter name of new file: ");
                        string fileNewName = Console.ReadLine();

                        string clientPath1 = Path.Combine(clientPath, fileNewName).Trim();
                        binaryWriter.Write(toServer);


                        int fileSize = binaryReader.ReadInt32();
                        byte[] fileData = binaryReader.ReadBytes(fileSize);

                        await File.WriteAllBytesAsync(clientPath1, fileData);

                        string answer1 = binaryReader.ReadString().Trim();
                        if (answer1 == "200")
                        {
                            Console.WriteLine($"The file was downloaded! Specify a name for it: {fileNewName}");
                            Console.WriteLine($"File saved on the hard drive!");
                        }
                        else
                        {
                            Console.WriteLine($"The response says that this file is not found!!");
                        }

                        break;

                    case "2":
                        Console.WriteLine("Enter filename: ");
                        fileName = Console.ReadLine() ?? string.Empty;

                        string userFile = Path.Combine(clientPath, fileName).Trim();


                        Console.WriteLine("Enter name of the file to be saved on server: ");
                        fileNameServer = Console.ReadLine();

                        toServer = command + "//" + fileName + "//" + fileNameServer + "//";

                        binaryWriter.Write(toServer); 
                        byte[] fileBytes = File.ReadAllBytes(userFile);
                        binaryWriter.Write(fileBytes.Length); // Сначала отправляем размер файла
                        binaryWriter.Write(fileBytes); // Затем сам файл

                        File.Delete(userFile);

                        string answer2 = binaryReader.ReadString().Trim();
                        if (answer2 == "404")
                        {
                            Console.WriteLine($"The response says that this file is not found!!");
                        }
                        else 
                        {
                            string[] parts = answer2.Split(new string[] { "//" }, StringSplitOptions.None);
                            Console.WriteLine($"Response says that file is saved! ID = {parts[1]}");
                            
                        }

                        break;

                    case "3":
                        Console.WriteLine("Do you want to delete the file by name or by id (1 - name, 2 - id): ");
                        string command3 = Console.ReadLine();

                        if (command3 == "1")
                        {
                            Console.WriteLine("Enter name: ");
                            fileName = Console.ReadLine();
                            toServer = command + "//" + command3 + "//" + fileName;
                        }
                        else if (command3 == "2")
                        {
                            Console.WriteLine("Enter id: ");
                            fileId = Console.ReadLine();
                            toServer = command + "//" + command3 + "//" + fileId;
                        }
                        binaryWriter.Write(toServer);

                        string answer3 = binaryReader.ReadString().Trim();
                        if (answer3 == "404")
                        {
                            Console.WriteLine($"The response says that this file is not found!!");
                        }
                        else if (answer3=="200")
                        {
                            Console.WriteLine($"The response says that this file was deleted successfully!");
                        }

                        break;

                    case "exit":
                        toServer = command + "//";
                        flag = false;
                        binaryWriter.Write(toServer);
                        break;
                }

                if (flag == false)
                {
                    break;
                }



                /*string answer = binaryReader.ReadString(); 
                Console.WriteLine($"{answer}");*/

                // Имитируем долговременную работу
                await Task.Delay(2000);

            }
            // Отправляем маркер завершения подключения - END
            binaryWriter.Write("END\n");
            binaryWriter.Flush();
            stream.Close();
            tcpClient.Close();
        }
    }
}
