using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            
            //TODO: initialize this.serverSocket
            IPEndPoint hostEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            serverSocket.Bind(hostEndPoint);

        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(5);

            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket ClientSocket = serverSocket.Accept();
                Thread ClientThread = new Thread(new ParameterizedThreadStart(HandleConnection));
                ClientThread.Start(ClientSocket);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            Socket ClientSocket = (Socket)obj;

            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] requestBytes = new byte[1024];
                    int length = ClientSocket.Receive(requestBytes);

                    // TODO: break the while loop if receivedLen==0
                    if (length == 0)
                        break;

                    // TODO: Create a Request object using received request string
                    string requestString = Encoding.ASCII.GetString(requestBytes, 0, length);
                    Request request = new Request(requestString);

                    // TODO: Call HandleRequest Method that returns the response
                    Response resopnse = HandleRequest(request);

                    // TODO: Send Response back to client
                    byte[] responseBytes = Encoding.ASCII.GetBytes(resopnse.ResponseString);
                    ClientSocket.Send(responseBytes);
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                    Console.WriteLine(ex.Message);
                }
            }

            // TODO: close client socket
            ClientSocket.Close();
        }

        Response HandleRequest(Request request)
        {
            string content, ResponsePath ,RootPath = Configuration.RootPath + "\\";
            StatusCode RStatusCode;
            Response ResponseToSend;
            try
            {
                //TODO: check for bad request 
                if (request.ParseRequest())
                {
                    RStatusCode = StatusCode.OK;
                    ResponsePath =  RootPath + request.relativeURI;

                    //TODO: check for redirect
                    if (Configuration.RedirectionRules.ContainsKey(request.relativeURI.TrimStart('/')))
                    {
                        RStatusCode = StatusCode.Redirect;
                        ResponsePath = GetRedirectionPagePathIFExist(request.relativeURI);
                    }
                    else if (request.relativeURI == "/")
                    {
                        ResponsePath = LoadDefaultPage("main.html");
                    }

                }
                else
                {
                    RStatusCode = StatusCode.BadRequest;
                    ResponsePath = RootPath + Configuration.BadRequestDefaultPageName;
                }
                
               

                //TODO: map the relativeURI in request to get the physical path of the resource.
                

                //TODO: check file exists
                if (!File.Exists(ResponsePath))
                {
                    RStatusCode = StatusCode.NotFound;
                    ResponsePath = RootPath + Configuration.NotFoundDefaultPageName;
                }

                ResponsePath = ResponsePath.Trim();

                //TODO: read the physical file
                content = File.ReadAllText(ResponsePath);
                 
                // Create OK response
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                // TODO: in case of exception, ret   urn Internal Server Error. 
                Logger.LogException(ex);

                RStatusCode = StatusCode.InternalServerError;
                ResponsePath = RootPath + Configuration.InternalErrorDefaultPageName;

                //TODO: read the physical file
                ResponsePath = ResponsePath.Trim();
                content = File.ReadAllText(ResponsePath);
                Console.WriteLine(ex.Message);
            }
            
            ResponseToSend = new Response(RStatusCode, "text/html", content, ResponsePath);
            return ResponseToSend;
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty

            return Configuration.RootPath + "\\" +  Configuration.RedirectionRules[relativePath.TrimStart('/')];
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            
            // else read file and return its content
            return filePath;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file                
                filePath += "\\redirectionRules.txt";
                string text = File.ReadAllText(filePath);
                string[] lines = text.Split('\n');
               
                // then fill Configuration.RedirectionRules dictionary 
                Configuration.RedirectionRules = new Dictionary<string, string>();

                for (int i =0;i< lines.Length; i++)
                {
                    string[] from_to = lines[i].Split(',');
                    if(from_to.Length == 2 && !Configuration.RedirectionRules.ContainsKey(from_to[0]))
                        Configuration.RedirectionRules.Add(from_to[0], from_to[1]);
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
        }
    }
}
