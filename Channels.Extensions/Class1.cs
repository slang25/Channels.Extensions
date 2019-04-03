using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Channels.Extensions
{
    public static class ChannelReaderExtensions
    {
        public static async IAsyncEnumerable<T> AsAsyncEnumerator<T>(this ChannelReader<T> channelReader, CancellationToken cancellationToken = default)
        {
            while (await channelReader.WaitToReadAsync(cancellationToken))
            {
                while (channelReader.TryRead(out var item))
                {
                    yield return item;
                }
            }
        }
    }
    
    public static class AsyncEnumerableExtensions
    {
        public static void PostToChannel<T>(this IAsyncEnumerable<T> asyncEnumerable, ChannelWriter<T> channelWriter, CancellationToken cancellationToken = default)
        {
            async Task RunPostToChannel()
            {
                try
                {
                    await foreach (var item in asyncEnumerable.WithCancellation(cancellationToken))
                    {
                        if (!channelWriter.TryWrite(item))
                        {
                            await channelWriter.WriteAsync(item, cancellationToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    channelWriter.TryComplete(ex);
                }
                finally
                {
                    channelWriter.TryComplete();
                }
            }

            _ = RunPostToChannel();
        }
    }
}