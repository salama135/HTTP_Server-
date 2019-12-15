using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
            headerLines = new Dictionary<string, string>();
        }

        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {            
            //TODO: parse the receivedRequest using the \r\n delimeter   
            requestLines = requestString.Split(new string[] {"\r\n"}, StringSplitOptions.None);

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length < 3)
                return false;

            // Parse Request line
            if (!ParseRequestLine())
                return false;

            // Validate blank line exists
            if (!ValidateBlankLine())
                return false;

            // Load header lines into HeaderLines dictionary
            if (!LoadHeaderLines())
                return false;

            return true;
        }

        private bool ParseRequestLine()
        {
            string[] requestLine = requestLines[0].Split(' ');

            if (requestLine[0] != "GET")
                return false;

            method = RequestMethod.GET;

            //relativeURI = "aboutus2.html";
            relativeURI = requestLine[1].Split('/')[1];
            
            if (!ValidateIsURI(relativeURI))
                return false;

            if (requestLine[2] != Configuration.ServerHTTPVersion)
                return false;

            httpVersion = HTTPVersion.HTTP11;

            return true;
        }

        private bool ValidateBlankLine()
        {            
            return requestLines[requestLines.Length - 1] == "";
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            for (int i = 1; i < requestLines.Length - 2; i++)
            {
                string[] headerLine = requestLines[i].Split(new string[] { ": " }, StringSplitOptions.None);

                if (headerLine.Length != 2)
                    return false;

                if (headerLine[0] == "" || headerLine[1] == "")
                    return false;

                headerLines.Add(headerLine[0], headerLine[1]);
            }

            return true;
        }
    }
}
