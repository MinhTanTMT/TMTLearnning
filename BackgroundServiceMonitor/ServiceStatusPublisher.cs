using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundServiceMonitor
{
    public static class ServiceStatusPublisher
    {
        public static IObservable<string> TMTCreateMyObservable(NetworkStream myStream)
        {
            return new BackgroundServiceMonitor(myStream);
        }

        private class BackgroundServiceMonitor : IObservable<string>
        {
            private readonly NetworkStream _myStream;

            public BackgroundServiceMonitor(NetworkStream myStream)
            {
                _myStream = myStream;
            }

            public IDisposable Subscribe(IObserver<string> observer)
            {
                var cts = new CancellationTokenSource();
                var task = Task.Run(() => ReadLoopAsync(observer, cts.Token));
                // khởi tạo một luồng riêng cho function ReadLoopAsync biến nó thành hàm bất đồng bộ

                return new CancellationDisposable(cts);
            }

            public async Task ReadLoopAsync(IObserver<string> myObservable, CancellationToken cancellationToken)
            {
                var buffer = new byte[1024];
                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        int byteRead = await _myStream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
                        if (byteRead == 0)
                        {
                            myObservable.OnCompleted();
                            break;
                        }

                        string data = Encoding.UTF8.GetString(buffer, 0, byteRead);
                        myObservable.OnNext(data);
                    }
                }
                catch (Exception ex)
                {
                    myObservable.OnError(ex);
                }
            }

            private class CancellationDisposable : IDisposable
            {

                private readonly CancellationTokenSource _cts;

                public CancellationDisposable(CancellationTokenSource cts) {
                    _cts = cts; 
                }
                public void Dispose()
                {
                    _cts.Cancel();
                    _cts.Dispose();
                }
            }
        }
    }
}
