using System;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Channels.Extensions.Tests
{
    public class UnitTest1
    {
        readonly ITestOutputHelper testOutputHelper;

        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Test1()
        {
            const int channelSize = 10;
            var channel = Channel.CreateBounded<string>(new BoundedChannelOptions(channelSize));
            
            var asyncSeq = channel.Reader.AsAsyncEnumerator();
            var input = AsyncEnumerable.Repeat("hello", 50);

            input.PostToChannel(channel.Writer);

            await asyncSeq.ForEachAsync(x => testOutputHelper.WriteLine(x));
        }
    }
}