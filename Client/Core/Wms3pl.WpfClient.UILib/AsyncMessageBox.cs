using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace Wms3pl.WpfClient.UILib
{
    public class AsyncMessageBox
    {
        public EventWaitHandle eventWaitHandle = new ManualResetEvent(false);
        public MessageBoxResult result;
        public bool IsYesNo;
        public string content;
        public string title;

        public AsyncMessageBox(string content, string title, bool IsYesNo)
        {
            this.content = content;
            this.title = title;
            this.IsYesNo = IsYesNo;
        }

        public static void PerformUserWorkItem(Object asyncmsgboxObject)
        {
            AsyncMessageBox asyncmsgbox = asyncmsgboxObject as AsyncMessageBox;

            if (asyncmsgbox != null)
            {
                if (asyncmsgbox.IsYesNo == true)
                {
                    asyncmsgbox.result = MessageBox.Show(asyncmsgbox.content, asyncmsgbox.title, MessageBoxButton.YesNo,
                                                   MessageBoxImage.Asterisk, MessageBoxResult.No);
                }
                else
                {
                    MessageBox.Show(asyncmsgbox.content, asyncmsgbox.title);
                }
                asyncmsgbox.eventWaitHandle.Set(); // signal we're done
            }
        }

        public static MessageBoxResult ShowMessage(string content, string title, bool IsYesNo)
        {
            AsyncMessageBox asyncmsgbox = new AsyncMessageBox(content, title, IsYesNo);
            ThreadPool.QueueUserWorkItem(AsyncMessageBox.PerformUserWorkItem, asyncmsgbox);
            asyncmsgbox.eventWaitHandle.WaitOne();
            return asyncmsgbox.result;
        }
    }
}
