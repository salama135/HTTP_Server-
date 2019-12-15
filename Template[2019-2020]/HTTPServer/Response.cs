using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            //throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            this.code = code;
            headerLines.Add("Date: " + System.DateTime.Now.ToString());
            headerLines.Add("Server: " + Configuration.ServerType);
            headerLines.Add("Content-Type: " + contentType);
            headerLines.Add("Content-Length: " + content.Length.ToString());

            if (code==StatusCode.Redirect)
            {
                headerLines.Add("Location: " + redirectoinPath);
            }


            // TODO: Create the request string
            responseString = GetStatusLine(code) + "\r\n";
            for (int i = 0; i < headerLines.Count; i++)
            {
                responseString += headerLines[i] + "\r\n";
            }
            responseString += "\r\n";

            responseString += content;

        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string Message = "";

            if (code == StatusCode.OK)
            {
                Message = "OK";
            }
            else if (code == StatusCode.Redirect)
            {
                Message = "Redirection";
            }
            else if (code == StatusCode.NotFound)
            {
                Message = "Page not found :(";
            }
            else if(code == StatusCode.InternalServerError)
            {
                Message = "Internal server error";
            }

            string statusLine = Configuration.ServerHTTPVersion + " " + code + " " + Message;

            return statusLine;
        }
    }
}
