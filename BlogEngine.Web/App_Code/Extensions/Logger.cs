#region using

using System;
using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;
using System.IO;
using System.Text;

#endregion

/// <summary>
/// Subscribes to Log events and records the events in a file.
/// </summary>
[Extension("Subscribes to Log events and records the events in a file.", "1.0", "BlogEngine.NET")]
public class Logger
{
	static Logger()
	{
        Utils.OnLog += new EventHandler<EventArgs>(OnLog);
	}

	/// <summary>
	/// The event handler that is triggered every time there is a log notification.
	/// </summary>
    private static void OnLog(object sender, EventArgs e)
	{
        if (sender == null || !(sender is string))
            return;

        string logMsg = (string)sender;

        if (string.IsNullOrEmpty(logMsg))
            return;

        string file = GetFileName();

        StringBuilder sb = new StringBuilder();

        lock (_SyncRoot)
        {
            try
            {
                using (FileStream fs = new FileStream(file, FileMode.Append))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(@"*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*");
                        sw.WriteLine("Date: " + DateTime.Now.ToString());
                        sw.WriteLine("Contents Below");
                        sw.WriteLine(logMsg);

                        sw.Close();
                        fs.Close();
                    }
                }
            }
            catch
            {
                // Absorb the error.
            }
        }
	}

    private static string _FileName;
    private static object _SyncRoot = new object();

    private static string GetFileName()
    { 
        if (_FileName != null)
            return _FileName;

        _FileName = System.Web.Hosting.HostingEnvironment.MapPath(Path.Combine(BlogSettings.Instance.StorageLocation, "logger.txt"));
        return _FileName;
    }
}