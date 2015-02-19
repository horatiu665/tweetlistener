using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace WPFTwitter
{
	public class ConsoleHelper
	{
		// answer here: http://stackoverflow.com/questions/6408588/how-to-tell-if-there-is-a-console
		public static bool ConsolePresent
		{
			get
			{
				var _consolePresent = true;
				try { 
					int window_height = Console.WindowHeight; 
				}
				catch { 
					_consolePresent = false; 
				}
				return _consolePresent;
			}
		}

		public static int Create()
		{
			if (AllocConsole())
				return 0;
			else
				return Marshal.GetLastWin32Error();
		}

		public static int Destroy()
		{
			if (FreeConsole())
				return 0;
			else
				return Marshal.GetLastWin32Error();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool AllocConsole();


		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool FreeConsole();
	}
}
