using System;
using System.Linq;
using Qtee.Contracts.Processors;
using Qtee.Processors.QCounter;
using Xunit;

namespace Qtee.XUnitTests
{
    public class TestBasicCounter
    {
        [Fact]
        public void TestForGoodResult()
        {
            var goodResult = 989.20091m;
            var currentResult = 0m;
            using (var processorSwicher = new BasicCounter())
            {
                var counterTaskResult = new CounterTaskResult();
                counterTaskResult.SetValue(goodResult);
                processorSwicher.Add(counterTaskResult);
                ((IQProcessorSwicher) processorSwicher).StartProcessing();
                var waitForState = processorSwicher.WaitForState(QProcessorState.Wainting);
                waitForState.Wait(1000);
                if (waitForState.Result)
                {
                    ((IQProcessorSwicher) processorSwicher).StopProcessing();
                    currentResult = processorSwicher.GetLastOrNull();
                }
            }

            Assert.Equal(goodResult * goodResult, currentResult);
        }
    }
}