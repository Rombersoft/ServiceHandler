using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServiceHelper
{
    public delegate string CallBackDelegate(string command);

    public class ServiceSocket : IDisposable
    {
        public CallBackDelegate CallBack;

        private bool _debug;
        //use it if you want to output in console
        private Task _task;
        private int _port;
        //listen port
        private Thread _thread;
        //need for stop listen thread

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceHelper.ServiceSocket"/> class.
        /// </summary>
        /// <param name="port">Port for TcpListener</param>
        /// <param name="debug">If set to <c>true</c> output will be in console</param>
        public ServiceSocket(int port, CallBackDelegate callBack, bool debug)
        {
            _port = port;
            _debug = debug;
            CallBack = callBack;
            _task = new Task(Run);
        }

        /// <summary>
        /// Starts the port listen for localhost
        /// </summary>
        public void StartListen()
        {
            _task.Start();
            Thread.Sleep(100);
        }

        /// <summary>
        /// Stops the listen for localhost
        /// </summary>
        public void StopListen()
        {
            _thread.Abort();
        }

        private void Run()
        {
            _thread = Thread.CurrentThread;
            TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), _port);
            try
            {
                listener.Start();
            }
            catch (Exception ex)
            {
                if (_debug)
                    Console.WriteLine("ServiceSocket -> Listen of port {0} was failed. Error {1}", _port, ex.Message);
                return;
            }
            while (true)
            {
                TcpClient server = null;
                NetworkStream stream = null;
                try
                {
                    server = listener.AcceptTcpClient();
                    byte[] buffer = new byte[4096];
                    server.SendTimeout = 5000;
                    stream = server.GetStream();
                    stream.ReadTimeout = 5000;
                    stream.WriteTimeout = 30000;
                    int redBytes = stream.Read(buffer, 0, buffer.Length);
                    StringBuilder builder = new StringBuilder();
                    builder.Append(Encoding.ASCII.GetChars(buffer, 0, redBytes));
                    string command = builder.ToString().Replace("\n", "").Replace("\r", "");
                    if (_debug)
                        Console.WriteLine("ServiceSocket -> It was gotten command <{0}>", command);
                    byte[] answer;
                    if (CallBack != null)
                        answer = Encoding.ASCII.GetBytes(CallBack(command));
                    else
                        answer = Encoding.ASCII.GetBytes("Unrecognized command");
                    stream.Write(answer, 0, answer.Length);
                }
                catch (ThreadAbortException)
                {
                    if (_debug)
                        Console.WriteLine("ServiceSocket -> Exit");
                    break;
                }
                catch (Exception ex)
                {
                    if (stream != null)
                    {
                        byte[] answer = Encoding.ASCII.GetBytes(ex.Message);
                        stream.Write(answer, 0, answer.Length);
                    }
                }
                finally
                {
                    if (stream != null)
                        stream.Dispose();
                    if (server != null)
                        server.Close();
                }
            }
        }

        public void Dispose()
        {
            Thread.Sleep(1000);
            _task.Dispose();
        }
    }
}

