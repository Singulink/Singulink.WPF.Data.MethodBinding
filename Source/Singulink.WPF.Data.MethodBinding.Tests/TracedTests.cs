using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Singulink.WPF.Data.Tests
{
    public class TracedTests : TraceListener
    {
        private readonly List<string> _traceMessages = new();
        private bool _lastLineFinished = true;

        public IReadOnlyList<string> TraceMessages => _traceMessages;

        public TracedTests()
        {
            Trace.Listeners.Add(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Trace.Listeners.Remove(this);

            base.Dispose(disposing);
        }

        public override void Write(string? message) => Write(message, false);

        public override void WriteLine(string? message) => Write(message, true);

        private void Write(string? message, bool finishLine)
        {
            if (message == null)
                return;

            if (_lastLineFinished) {
                _traceMessages.Add(message);
            }
            else {
                _traceMessages[^1] += message;
            }

            _lastLineFinished = finishLine;
        }
    }
}
