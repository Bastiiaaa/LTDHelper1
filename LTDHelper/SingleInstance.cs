using System;
using System.Threading;
using System.Windows;
using Microsoft.VisualBasic.CompilerServices;

namespace LTDHelper;

[StandardModule]
internal sealed class SingleInstance
{
	[STAThread]
	public static void Main()
	{
		bool createdNew;
		using (new Mutex(initiallyOwned: true, "LTDHelper for Geode", out createdNew))
		{
			if (!createdNew)
			{
				MessageBox.Show("Extension is already started!", "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			MainWindow window = new MainWindow();
			new Application().Run(window);
		}
	}
}
