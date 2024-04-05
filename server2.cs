using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Diagnostics.Metrics;


var tcpListener = new TcpListener(IPAddress.Any, 8888);

try
{
    tcpListener.Start();    // Запускаем сервер
    Console.WriteLine("Server started");

    while (true)
    {
        try
        {
            // Получаем подключение в виде TcpClient
            var tcpClient = await tcpListener.AcceptTcpClientAsync();

            // Создаем новую задачу для обслуживания нового клиента
            Task.Run(async () => await ProcessClientAsync(tcpClient));
        }
        catch
        {
            break;
        }
        
    }
}
finally
{
    tcpListener.Stop();
}

async Task ProcessClientAsync(TcpClient tcpClient)
{
            var stream = tcpClient.GetStream();


            using (stream)
            using (BinaryWriter binaryWriter = new BinaryWriter(stream))
            using (BinaryReader binaryReader = new BinaryReader(stream))
            {
                bool flag = true;
                while (flag)
                {
                    string toServer = binaryReader.ReadString();

                    const string clientPath = @"C:\Users\user\Documents\c#\client";
                    const string serverPath = @"C:\Users\user\Documents\c#\server";
                    const string archivePath = @"C:\Users\user\Documents\c#\archive.txt";

                    //var toServer = Encoding.UTF8.GetString(response.ToArray());
                    //Console.WriteLine(toServer);
                    string[] parts = toServer.Split(new string[] { "//" }, StringSplitOptions.None);
                    string command = parts[0];
                    string result = "";


                    switch (command)
                    {
                        case "1":
                            if (parts[1].Trim() == "1") //name
                            {
                                string fileName1 = parts[2].Trim();
                                if (File.Exists(Path.Combine(serverPath, fileName1).Trim())) 
                                {
                                    string startPath1 = Path.Combine(serverPath, fileName1).Trim();

                                    byte[] fileBytes = File.ReadAllBytes(startPath1);
                                    binaryWriter.Write(fileBytes.Length); // Сначала отправляем размер файла
                                    binaryWriter.Write(fileBytes); // Затем сам файл

                                    File.Delete(startPath1);

                                    result = "200";

                                    result += '\n';
                                    binaryWriter.Write(result);
                                    binaryWriter.Flush();
                                }
                                else
                                {
                                    result = "404";

                                    result += '\n';
                                    binaryWriter.Write(result);
                                    binaryWriter.Flush();
                                }
                            }
                            else //id
                            {
                                string fileId = parts[2].Trim();
                        
                                using (StreamReader reader = new StreamReader(archivePath))
                                {
                                    string currentLine;
                                    while ((currentLine = reader.ReadLine()) != null)
                                    {
                                        string[] parts1 = currentLine.Split(' ');
                                        if (parts1[0].Trim() == fileId)
                                        {
                                            string fileName1 = parts1[1];
                                            if (File.Exists(Path.Combine(serverPath, fileName1).Trim()))
                                            {
                                                string startPath1 = Path.Combine(serverPath, fileName1).Trim();

                                                byte[] fileBytes = File.ReadAllBytes(startPath1);
                                                binaryWriter.Write(fileBytes.Length); // Сначала отправляем размер файла
                                                binaryWriter.Write(fileBytes); // Затем сам файл

                                                File.Delete(startPath1);

                                                result = "200";

                                                result += '\n';
                                                binaryWriter.Write(result);
                                                binaryWriter.Flush();
                                            }
                                            else
                                            {
                                                result = "404";

                                                result += '\n';
                                                binaryWriter.Write(result);
                                                binaryWriter.Flush();
                                            }
                                        }

                                    }
                                }
                            }
                            break;
                    
                        case "2":

                        string fileName = parts[1];
                        string fileNameServer = parts[2];
                        string startPath2 = Path.Combine(clientPath, fileName).Trim();

                        string finishPath2;
                        if (fileNameServer != null)
                        {  
                            finishPath2 = Path.Combine(serverPath, fileNameServer);
                            finishPath2.Trim();
                        }
                        else
                        {  
                            finishPath2 = Path.Combine(serverPath, fileName);
                            finishPath2.Trim();
                        }

                        if((File.Exists(finishPath2)) || (!File.Exists(startPath2)))
                        {
                            result = "404";
                            result += '\n';
                            binaryWriter.Write(result);
                            binaryWriter.Flush();
                        }
                        else
                        {
                        int fileSize = binaryReader.ReadInt32();
                        byte[] fileData = binaryReader.ReadBytes(fileSize);
                        await File.WriteAllBytesAsync(finishPath2, fileData);



                        string previousLine = null;
                            int number = 0;
                            using (StreamReader reader = new StreamReader(archivePath))
                            {
                                string currentLine;
                                // Читаем файл построчно до его конца
                                while ((currentLine = reader.ReadLine()) != null)
                                {
                                    previousLine = currentLine;
                                }
                            }
                            if (previousLine != null)
                            {
                                // Извлекаем число из предыдущей строки
                                string[] parts1 = previousLine.Split(' ');
                                if (parts1.Length > 0 && int.TryParse(parts1[0], out int nums))
                                {
                                    number = nums;
                                }
                            }
                            using (StreamWriter writer = new StreamWriter(archivePath, true))
                            {
                                writer.WriteLine($"{number+1} {fileNameServer}");
                            }

                            string numberOfFile = (number+1).ToString();
                            result = "200"  + "//" + numberOfFile;
                            result += '\n';
                            binaryWriter.Write(result);
                            binaryWriter.Flush();
                        }

                    

                        break;

                        case "3":
                    
                            if (parts[1].Trim() == "1") //name
                            {
                                string fileName3 = parts[2];

                                if (File.Exists(Path.Combine(serverPath, fileName3).Trim()))
                                {
                                    File.Delete((Path.Combine(serverPath, fileName3).Trim()));
                                    result = "200";
                                    result += '\n';
                                    binaryWriter.Write(result);
                                    binaryWriter.Flush();
                                }
                                else
                                {
                                    result = "404";
                                    result += '\n';
                                    binaryWriter.Write(result);
                                    binaryWriter.Flush();
                                }
                            }
                            else //id
                            {
                                string lineToDelete = "";
                                string fileId = parts[2];

                                using (StreamReader reader = new StreamReader(archivePath))
                                {
                                    string currentLine;
                                    while((currentLine = reader.ReadLine()) != null)
                                    {
                                        string[] parts3 = currentLine.Split(' ');
                                        if (parts3[0].Trim() == fileId.Trim())
                                        {
                                            string fileName3 = parts3[1];
                                            string serverPath3 = Path.Combine(serverPath, fileName3).Trim();
                                            lineToDelete = currentLine;
                                            
                                            if (File.Exists(serverPath3))
                                            {
                                                File.Delete(serverPath3);
                                                result ="200";
                                                result += '\n';
                                                binaryWriter.Write(result);
                                                binaryWriter.Flush();
                                            }
                                            else
                                            {
                                                result = "400";
                                                result += '\n';
                                                binaryWriter.Write(result);
                                                binaryWriter.Flush();
                                            }
                                            
                                        }

                                    }
                                }
                            }
                            result += '\n';
                            binaryWriter.Write(result);
                            binaryWriter.Flush();


                            break;

                        case "exit":
                            flag = false;
                            break;
                    }
                    if (flag == false)
                    {
                        break;
                    }
                }   
            }
            tcpListener.Stop();
            tcpClient.Close();
    
}
