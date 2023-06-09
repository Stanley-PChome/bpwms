using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.Common
{
  public class Wms3plFunction
  {
    public static object IfValue(object sourceValue, object ifValueIs, object convValue)
    {
      object obj;
      if (ifValueIs == DBNull.Value)
        obj = (sourceValue == DBNull.Value) ? convValue : sourceValue;
      else
        obj = (sourceValue == ifValueIs) ? convValue : sourceValue;

      return obj;
      //Public Function IfValue(SourceValue As Variant, IfValueIs As Variant, ConvValue As Variant) As Variant
      //    If IsNull(IfValueIs) Then
      //        IfValue = IIf(Not IsNull(SourceValue), SourceValue, ConvValue)
      //    Else
      //        IfValue = IIf(SourceValue <> IfValueIs, SourceValue, ConvValue)
      //    End If
      //End Function
    }

    /// <summary>
    /// 非同步執行
    /// </summary>
    /// <param name="asyncAction"></param>
    /// <param name="completed"></param>
    /// <param name="failed"></param>
    public static void AsyncAction(Action asyncAction, Action completed, Action<Exception> failed)
    {
      var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
      Task.Factory.StartNew(asyncAction)
        .ContinueWith(t =>
        {
          if (t.IsFaulted)
            failed(t.Exception.InnerExceptions.First());
          else
            completed();
        }, scheduler);
    }

    /// <summary>
    /// 非同步執行
    /// </summary>
    /// <param name="asyncAction"></param>
    /// <param name="completed"></param>
    /// <param name="failed"></param>
    /// <param name="cancellationToken"> </param>
    public static Task AsyncAction(Action asyncAction, Action completed, Action<Exception> failed,
      CancellationToken cancellationToken, Action cancelAction)
    {
      var ui = TaskScheduler.FromCurrentSynchronizationContext();
      var task = Task.Factory.StartNew(asyncAction, cancellationToken)
        .ContinueWith(t =>
        {
          completed();
        }, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, ui)
        .ContinueWith(r => cancelAction(),
        CancellationToken.None, TaskContinuationOptions.OnlyOnCanceled, ui).ContinueWith(
        t => failed(t.Exception.InnerExceptions.First()), CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, ui);

      return task;
    }
  }
}
