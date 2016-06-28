using System;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;

namespace FlyCallWS.Streaming
{
    internal class ClientStreaming
    {

        // This class stores the State of the request.
        const int BUFFER_SIZE = 1024;
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        private FileStream _fs = null;
        //bool ParsedError = false;

        public bool GetResponse(HttpWebRequest myHttpWebRequest, FileStream fs)
        {

            try
            {
                _fs = fs;

                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                // Gets the stream associated with the response.
                Stream responseStream = myHttpWebResponse.GetResponseStream();
                int totRead = 0;

                var result = new StringBuilder("");
                do
                {
                    byte[] buffer = new byte[BUFFER_SIZE];
                    totRead = responseStream.Read(buffer, 0, buffer.Length);
                    if (totRead == 0)
                    {
                        break;
                    }
                    _fs.Write(buffer, 0, totRead);
                    //result.Append(Encoding.UTF8.GetString(buffer, 0, read));
                }
                while (totRead <= BUFFER_SIZE);
                return true;

                //qui sotto la vecchia gestione che fà scattare le altre 2 funzioni
                //RequestState myRequestState = new RequestState();
                //myRequestState.request = myHttpWebRequest;
                //ParsedError = false;

                ////Start the asynchronous request.
                //IAsyncResult result =
                //  (IAsyncResult)myHttpWebRequest.BeginGetResponse(new AsyncCallback(RespCallback), myRequestState);

                //if (!ParsedError)
                //{
                //    allDone = new ManualResetEvent(false);
                //    allDone.WaitOne(-1);

                //    if (myRequestState.response != null)
                //    {
                //        // Release the HttpWebResponse resource.
                //        myRequestState.response.Close();
                //    }

                //    return true;
                //}
                //return false;

            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    Stream responseStream = e.Response.GetResponseStream();
                    int totRead = 0;

                    var result = new StringBuilder("");
                    do
                    {
                        byte[] buffer = new byte[BUFFER_SIZE];
                        totRead = responseStream.Read(buffer, 0, buffer.Length);
                        if (totRead == 0)
                        {
                            break;
                        }
                        _fs.Write(buffer, 0, totRead);
                        result.Append(Encoding.UTF8.GetString(buffer, 0, totRead));

                    }
                    while (totRead <= BUFFER_SIZE);
                    return false;
                }
                else
                    throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //private void RespCallback(IAsyncResult asynchronousResult)
        //{
        //    // State of request is asynchronous.
        //    RequestState myRequestState = (RequestState)asynchronousResult.AsyncState;

        //    try
        //    {
        //        HttpWebRequest myHttpWebRequest = myRequestState.request;
        //        myRequestState.response = (HttpWebResponse)myHttpWebRequest.EndGetResponse(asynchronousResult);

        //        // Read the response into a Stream object.
        //        Stream responseStream = myRequestState.response.GetResponseStream();
        //        myRequestState.streamResponse = responseStream;

        //        // Begin the Reading of the contents of the HTML page and print it to the console.
        //        IAsyncResult asynchronousInputRead = responseStream.BeginRead(myRequestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);
        //        return;
        //    }
        //    catch (WebException e)
        //    {
        //        if (e.Response != null)
        //        {
        //            Stream responseStream = e.Response.GetResponseStream();
        //            int totRead = 0;

        //            var result = new StringBuilder("");
        //            do
        //            {
        //                byte[] buffer = new byte[BUFFER_SIZE];
        //                var read = responseStream.Read(buffer, 0, buffer.Length);
        //                if (read == 0)
        //                {
        //                    break;
        //                }
        //                _fs.Write(buffer, 0, read);
        //                result.Append(Encoding.UTF8.GetString(buffer, 0, read));
        //                totRead += read;
        //            }
        //            while (totRead <= BUFFER_SIZE);
        //            ParsedError = true;
        //            return;
        //        }
        //        else
        //            throw e;
        //    }
        //    finally
        //    {
        //        allDone.Set();
        //    }

        //}

        //private void ReadCallBack(IAsyncResult asyncResult)
        //{
        //    try
        //    {

        //        RequestState myRequestState = (RequestState)asyncResult.AsyncState;
        //        Stream responseStream = myRequestState.streamResponse;
        //        int read = responseStream.EndRead(asyncResult);
        //        // Read the HTML page and then print it to the console.
        //        if (read > 0)
        //        {
        //            //checosa += Encoding.ASCII.GetString(myRequestState.BufferRead, 0, read);
        //            _fs.Write(myRequestState.BufferRead, 0, read);
        //            IAsyncResult asynchronousResult = responseStream.BeginRead(myRequestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);
        //            return;
        //        }
        //        else
        //        {
        //            _fs.Flush();
        //            _fs.Close();
        //            responseStream.Close();
        //            allDone.Set();
        //        }

        //    }
        //    catch (WebException e)
        //    {
        //        allDone.Set();
        //        throw e;
        //    }
        //}
    }


}

public class RequestState
{
    // This class stores the State of the request.
    const int BUFFER_SIZE = 1024;
    public StringBuilder requestData;
    public byte[] BufferRead;
    public HttpWebRequest request;
    public HttpWebResponse response;
    public Stream streamResponse;
    public RequestState()
    {
        BufferRead = new byte[BUFFER_SIZE];
        requestData = new StringBuilder("");
        request = null;
        streamResponse = null;


    }
}
