using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;

namespace Wms3pl.ECCommon
{
    /// <summary>
    /// A <see cref="TraceListener"/> that writes an email message, formatting the output with an <see cref="ILogFormatter"/>.
    /// </summary>
    [ConfigurationElementType(typeof(CustomTraceListenerData))]
    public class Wms3plEmailTraceListener : CustomTraceListener
    {
        private string toAddress = String.Empty;
        private string fromAddress = String.Empty;
        private string subjectLineStarter = String.Empty;
        private string subjectLineEnder = String.Empty;
        private string smtpServer = String.Empty;
        private int smtpPort = 25;
        private EmailAuthenticationMode authenticationMode = EmailAuthenticationMode.None;
        private string userName = string.Empty;
        private string password = string.Empty;
        private bool useSSL = false;
        private bool initialized = false;

        public Wms3plEmailTraceListener(): base()
        {
        }

        private void Initialize()
        {
            if (!initialized) {
            toAddress = this.Attributes["toAddress"];
            fromAddress = this.Attributes["fromAddress"];
            subjectLineStarter = this.Attributes["subjectLineStarter"];
            subjectLineEnder = this.Attributes["subjectLineEnder"];
            smtpServer = this.Attributes["smtpServer"];
            toAddress = this.Attributes["toAddress"];
            if (!string.IsNullOrWhiteSpace(this.Attributes["smtpPort"]))
                smtpPort = int.Parse(this.Attributes["smtpPort"]);
            if (!string.IsNullOrWhiteSpace(this.Attributes["useSSL"]))
                useSSL = bool.Parse(this.Attributes["useSSL"]);
            }
            initialized = true;
        }

        /// <summary>
        /// Sends an email message given a predefined string
        /// </summary>
        /// <param name="message">The string to write as the email message</param>
        public override void Write(string message)
        {
            Initialize();
            var mailMessage =
                new Wms3plEmailMessage(toAddress, fromAddress, subjectLineStarter, subjectLineEnder, smtpServer, smtpPort,
                                 message, this.Formatter, authenticationMode, userName, password, useSSL);
            mailMessage.Send();
        }

        /// <summary>
        /// Sends an email message given a predefined string
        /// </summary>
        /// <param name="message">The string to write as the email message</param>
        public override void WriteLine(string message)
        {
            Write(message);
        }

        /// <summary>
        /// Delivers the trace data as an email message.
        /// </summary>
        /// <param name="eventCache">The context information provided by <see cref="System.Diagnostics"/>.</param>
        /// <param name="source">The name of the trace source that delivered the trace data.</param>
        /// <param name="eventType">The type of event.</param>
        /// <param name="id">The id of the event.</param>
        /// <param name="data">The data to trace.</param>
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id,
                                       object data)
        {
            Initialize();
            if ((this.Filter == null) ||
                this.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null))
            {
                if (data is LogEntry)
                {
                    var message = new Wms3plEmailMessage(toAddress, fromAddress, subjectLineStarter, subjectLineEnder,
                                                            smtpServer, smtpPort, data as LogEntry, this.Formatter,
                                                            authenticationMode, userName, password, useSSL);
                    message.Send();
                }
                else if (data is string)
                {
                    Write(data);
                }
                else
                {
                    base.TraceData(eventCache, source, eventType, id, data);
                }
            }
        }

        /// <summary>
        /// Declare the supported attributes for <see cref="EmailTraceListener"/>
        /// </summary>
        protected override string[] GetSupportedAttributes()
        {
            return new string[7]
                {
                    "formatter", "toAddress", "fromAddress", "subjectLineStarter", "subjectLineEnder", "smtpServer",
                    "smtpPort"
                };
        }
    }
}
