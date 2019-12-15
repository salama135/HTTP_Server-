using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();

            //Start server
            // 1) Make server object on port 1000
            // 2) Start Server
            Server mainServer = new Server(1000, Configuration.RootPath);
            mainServer.StartServer();
        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            // each line in the file specify a redirection rule
            // example: "aboutus.html, aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2

            string fileName = "C:\\inetpub\\wwwroot\\fcis1\\redirectionRules.txt";
            FileInfo fi = new FileInfo(fileName);

            try
            {
                // Check if file already exists. If yes, delete it.     
                if (fi.Exists)
                {
                    fi.Delete();
                }

                // Create a new file     
                using (StreamWriter sw = fi.CreateText())
                {
                    sw.WriteLine("aboutus.html,aboutus2.html");
                    sw.WriteLine("aboutus0.html,aboutus2.html");
                    sw.WriteLine("aboutus1.html,aboutus2.html");
                    sw.WriteLine("aboutus3.html,aboutus2.html");
                    sw.WriteLine("aboutus4.html,aboutus2.html");
                    sw.WriteLine("aboutus5.html,aboutus2.html");
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }         
    }
}
