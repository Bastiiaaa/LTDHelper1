using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;

namespace LTDHelper;

public class Application : System.Windows.Application
{
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "5.0.8.0")]
	public void InitializeComponent()
	{
		base.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
	}

	[DebuggerNonUserCode]
	[STAThread]
	[GeneratedCode("PresentationBuildTasks", "5.0.8.0")]
	public static void Main()
	{
		Application application = new Application();
		application.InitializeComponent();
		application.Run();
	}
}
