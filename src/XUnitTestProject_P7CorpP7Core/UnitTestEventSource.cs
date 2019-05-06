using System.Collections.Generic;
using System.Threading.Tasks;
using P7Core.Utils;
using Shouldly;
using Xunit;

namespace XUnitTestProject_P7CorpP7Core
{
    public class MyEventSink
    {

    }
    public class UnitTestEventSource: EventSource<MyEventSink>
    {

      
        [Fact]
        public async Task Test_Utils_ArgumentNotNull()
        {
            var sink = new MyEventSink();
            EventSinks.Count.ShouldBe(0);
            RegisterEventSink(sink);
            EventSinks.Count.ShouldBe(1);
            UnregisterEventSink(sink);
            EventSinks.Count.ShouldBe(0);
            RegisterEventSink(sink);
            RegisterEventSink(new MyEventSink());
            EventSinks.Count.ShouldBe(2);
            UnregisterAll();
            EventSinks.Count.ShouldBe(0);
            RegisterEventSink(sink);
            EventSinks.Count.ShouldBe(1);
            EventSinks = new List<MyEventSink>();
            EventSinks.Count.ShouldBe(0);
        }
       
    }
}