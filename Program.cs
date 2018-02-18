﻿/*
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
 
  Copyright (C) 2009-2013 Michael Möller <mmoeller@openhardwaremonitor.org>
	
*/

using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using OpenHardwareMonitor.GUI;

namespace OpenHardwareMonitor
{
    public static class Program
    {

        [STAThread]
        public static void Main(string[] args)
        {
#if !DEBUG
        Application.ThreadException += 
          new ThreadExceptionEventHandler(Application_ThreadException);
        Application.SetUnhandledExceptionMode(
          UnhandledExceptionMode.CatchException);

        AppDomain.CurrentDomain.UnhandledException += 
          new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
#endif

            if (!AllRequiredFilesAvailable())
                Environment.Exit(0);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (GUI.MainForm form = new GUI.MainForm())
            {
                ExApiClient.Start(args);
                form.FormClosed += delegate (Object sender, FormClosedEventArgs e)
                {
                    ExApiClient.Stop();
                    Application.Exit();
                };
                Application.Run();
            }
        }

        private static bool IsFileAvailable(string fileName)
        {
            string path = Path.GetDirectoryName(Application.ExecutablePath) +
              Path.DirectorySeparatorChar;

            if (!File.Exists(path + fileName))
            {
                MessageBox.Show("以下文件没有找到: " + fileName +
                  "\n请从压缩包中取出所有文件。", "错误",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private static bool AllRequiredFilesAvailable()
        {
            if (!IsFileAvailable("Aga.Controls.dll"))
                return false;
            if (!IsFileAvailable("OpenHardwareMonitorLib.dll"))
                return false;
            if (!IsFileAvailable("OxyPlot.dll"))
                return false;
            if (!IsFileAvailable("OxyPlot.WindowsForms.dll"))
                return false;

            return true;
        }

        private static void ReportException(Exception e)
        {
            CrashForm form = new CrashForm();
            form.Exception = e;
            form.ShowDialog();
        }

        public static void Application_ThreadException(object sender,
          ThreadExceptionEventArgs e)
        {
            try
            {
                ReportException(e.Exception);
            }
            catch
            {
            }
            finally
            {
                Application.Exit();
            }
        }

        public static void CurrentDomain_UnhandledException(object sender,
          UnhandledExceptionEventArgs args)
        {
            try
            {
                Exception e = args.ExceptionObject as Exception;
                if (e != null)
                    ReportException(e);
            }
            catch
            {
            }
            finally
            {
                Environment.Exit(0);
            }
        }
    }
}
