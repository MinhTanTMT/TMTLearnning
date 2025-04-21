using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks; // (optional)

namespace TMTLearn_Tcp_Observable
{
    public static class TcpObservableStream
    {
        public static IObservable<string> CreateObservable(NetworkStream stream)
        {
            return Observable.Create<string>(async (observer, cancellationToken) =>
            {
                var buffer = new byte[1024];
                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        int bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
                        if (bytesRead == 0)
                        {
                            observer.OnCompleted();
                            break;
                        }

                        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        observer.OnNext(data);
                    }
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }
            });
        }
    }
}
