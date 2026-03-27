using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using Geode.Extension;
using Geode.Habbo.Packages;
using Geode.Network;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace LTDHelper;

[DesignerGenerated]
public partial class MainWindow : Window, IComponentConnector
{
	public int CurrentLanguageInt;

	[CompilerGenerated]
	[AccessedThroughProperty("Extension")]
	private GeodeExtension _Extension;

	[CompilerGenerated]
	[AccessedThroughProperty("ConsoleBot")]
	private ConsoleBot _ConsoleBot;

	public bool TaskStarted;

	public bool TaskBlocked;

	public bool TestMode;

	public string[] CatalogCategory;

	public bool TaskCanBeStopped;

	public virtual GeodeExtension Extension
	{
		[CompilerGenerated]
		get
		{
			return _Extension;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			Action<DataInterceptedEventArgs> value2 = Extension_OnDataInterceptEvent;
			Action<string> value3 = Extension_OnCriticalErrorEvent;
			GeodeExtension geodeExtension = _Extension;
			if (geodeExtension != null)
			{
				geodeExtension.OnDataInterceptEvent -= value2;
				geodeExtension.OnCriticalErrorEvent -= value3;
			}
			_Extension = value;
			geodeExtension = _Extension;
			if (geodeExtension != null)
			{
				geodeExtension.OnDataInterceptEvent += value2;
				geodeExtension.OnCriticalErrorEvent += value3;
			}
		}
	}

	public virtual ConsoleBot ConsoleBot
	{
		[CompilerGenerated]
		get
		{
			return _ConsoleBot;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			Action<string> value2 = ConsoleBot_OnBotLoaded;
			Action<string> value3 = ConsoleBot_OnMessageReceived;
			ConsoleBot consoleBot = _ConsoleBot;
			if (consoleBot != null)
			{
				consoleBot.OnBotLoaded -= value2;
				consoleBot.OnMessageReceived -= value3;
			}
			_ConsoleBot = value;
			consoleBot = _ConsoleBot;
			if (consoleBot != null)
			{
				consoleBot.OnBotLoaded += value2;
				consoleBot.OnMessageReceived += value3;
			}
		}
	}

	public MainWindow()
	{
		base.Loaded += MainWindow_Loaded;
		CurrentLanguageInt = 0;
		TaskStarted = false;
		TaskBlocked = false;
		TestMode = false;
		CatalogCategory = new string[2] { "ler", "set_mode" };
		TaskCanBeStopped = true;
		InitializeComponent();
	}

	private void MainWindow_Loaded(object sender, RoutedEventArgs e)
	{
		base.Visibility = Visibility.Hidden;
		if (CultureInfo.CurrentCulture.Name.ToLower().StartsWith("es"))
		{
			CurrentLanguageInt = 1;
		}
		if (CultureInfo.CurrentCulture.Name.ToLower().StartsWith("pt"))
		{
			CurrentLanguageInt = 2;
		}
		Extension = new GeodeExtension("LTDHelper", "Geode examples.", "Lilith");
		Extension.Start();
		ConsoleBot = new ConsoleBot(Extension, "LTDHelper");
		ConsoleBot.ShowBot();
	}

	public void BotWelcome()
	{
		ConsoleBot.BotSendMessage(AppTranslator.WelcomeMessage[CurrentLanguageInt]);
		ConsoleBot.BotSendMessage(AppTranslator.BuyAdvice[CurrentLanguageInt]);
		ConsoleBot.BotSendMessage(AppTranslator.RiskAdvice[CurrentLanguageInt]);
		ConsoleBot.BotSendMessage(AppTranslator.FullCommandsList[CurrentLanguageInt]);
	}

	public async Task TryToBuyLTD()
	{
		TaskCanBeStopped = false;
		if (TaskStarted)
		{
			try
			{
				await Task.Delay(new Random().Next(500, 1000));
				Extension.SendToServerAsync(Extension.Out.GetCatalogIndex, "NORMAL");
				DataInterceptedEventArgs dataInterceptedEventArgs = await Extension.WaitForPacketAsync(Extension.In.CatalogIndex, 4000);
				ConsoleBot.BotSendMessage(AppTranslator.CatalogIndexLoaded[CurrentLanguageInt]);
				HCatalogNode hCatalogNode = new HCatalogNode(dataInterceptedEventArgs.Packet);
				HCatalogNode hCatalogNode2 = FindCatalogCategory(hCatalogNode.Children, CatalogCategory[Convert.ToInt32(TestMode)]);
				await Task.Delay(new Random().Next(500, 1000));
				ConsoleBot.BotSendMessage(AppTranslator.SimulatingPageClick[CurrentLanguageInt]);
				Extension.SendToServerAsync(Extension.Out.GetCatalogPage, hCatalogNode2.PageId, -1, "NORMAL");
				await Task.Delay(new Random().Next(500, 1000));
				ConsoleBot.BotSendMessage(AppTranslator.TryingToBuy[CurrentLanguageInt]);
				Extension.SendToServerAsync(Extension.Out.PurchaseFromCatalog, hCatalogNode2.PageId, hCatalogNode2.OfferIds[0], "", 1);
				if (await Extension.WaitForPacketAsync(Extension.In.PurchaseOK, 2000) == null)
				{
					throw new Exception("LTD not purchased!");
				}
				ConsoleBot.BotSendMessage(AppTranslator.PurchaseOK[CurrentLanguageInt]);
				TaskBlocked = true;
				TaskStarted = false;
				ConsoleBot.BotSendMessage(AppTranslator.ExitAdvice[CurrentLanguageInt]);
			}
			catch (Exception projectError)
			{
				ProjectData.SetProjectError(projectError);
				ConsoleBot.BotSendMessage(AppTranslator.PurchaseFailed[CurrentLanguageInt]);
				ProjectData.ClearProjectError();
			}
		}
		TaskCanBeStopped = true;
	}

	private HCatalogNode FindCatalogCategory(HCatalogNode[] NodeChildrens, string CategoryName)
	{
		foreach (HCatalogNode hCatalogNode in NodeChildrens)
		{
			if (Operators.CompareString(hCatalogNode.PageName, CategoryName, TextCompare: false) == 0)
			{
				return hCatalogNode;
			}
			HCatalogNode hCatalogNode2 = FindCatalogCategory(hCatalogNode.Children, CategoryName);
			if (hCatalogNode2 != null)
			{
				return hCatalogNode2;
			}
		}
		return null;
	}

	private void ConsoleBot_OnBotLoaded(string e)
	{
		BotWelcome();
	}

	private void ConsoleBot_OnMessageReceived(string e)
	{
		if (!TaskBlocked)
		{
			switch (e.ToLower())
			{
			case "/test":
			case "/probar":
			case "/testar":
				if (!TaskStarted)
				{
					ConsoleBot.BotSendMessage(AppTranslator.StartedMessage[CurrentLanguageInt]);
					TestMode = true;
					TaskStarted = true;
					TryToBuyLTD();
				}
				else
				{
					ConsoleBot.BotSendMessage(AppTranslator.ReducedCommandsList[CurrentLanguageInt]);
				}
				break;
			case "/force":
			case "/forzar":
			case "/forçar":
				if (!TaskBlocked & TaskCanBeStopped)
				{
					ConsoleBot.BotSendMessage(AppTranslator.StartedMessage[CurrentLanguageInt]);
					TestMode = false;
					TaskStarted = true;
					TryToBuyLTD();
				}
				else
				{
					ConsoleBot.BotSendMessage(AppTranslator.ReducedCommandsList[CurrentLanguageInt]);
				}
				break;
			case "/start":
			case "/iniciar":
			case "/começar":
				if (!TaskStarted)
				{
					ConsoleBot.BotSendMessage(AppTranslator.StartedMessage[CurrentLanguageInt]);
					TestMode = false;
					TaskStarted = true;
				}
				else
				{
					ConsoleBot.BotSendMessage(AppTranslator.ReducedCommandsList[CurrentLanguageInt]);
				}
				break;
			case "/stop":
			case "/detener":
			case "/parar":
				if (TaskStarted)
				{
					if (TaskCanBeStopped)
					{
						ConsoleBot.BotSendMessage(AppTranslator.StoppedMessage[CurrentLanguageInt]);
						TaskStarted = false;
					}
					else
					{
						ConsoleBot.BotSendMessage(AppTranslator.StopFailed[CurrentLanguageInt]);
					}
				}
				else
				{
					ConsoleBot.BotSendMessage(AppTranslator.FullCommandsList[CurrentLanguageInt]);
				}
				break;
			case "/salir":
			case "/sair":
				ConsoleBot.CustomExitCommand = e;
				break;
			default:
				if (!TaskStarted)
				{
					ConsoleBot.BotSendMessage(AppTranslator.FullCommandsList[CurrentLanguageInt]);
				}
				else
				{
					ConsoleBot.BotSendMessage(AppTranslator.ReducedCommandsList[CurrentLanguageInt]);
				}
				break;
			}
		}
		else
		{
			ConsoleBot.BotSendMessage(AppTranslator.ExitAdvice[CurrentLanguageInt]);
		}
	}

	private void Extension_OnDataInterceptEvent(DataInterceptedEventArgs e)
	{
		if ((Extension.In.ErrorReport.Match(e) | Extension.In.PurchaseError.Match(e) | Extension.In.PurchaseNotAllowed.Match(e) | Extension.In.NotEnoughBalance.Match(e)) && TaskStarted)
		{
			e.IsBlocked = true;
		}
		if (Extension.In.CatalogPublished.Match(e) && (TaskStarted & TaskCanBeStopped))
		{
			ConsoleBot.BotSendMessage(AppTranslator.CatalogUpdateReceived[CurrentLanguageInt]);
			TestMode = false;
			TryToBuyLTD();
		}
	}

	private void Extension_OnCriticalErrorEvent(string e)
	{
		base.Visibility = Visibility.Visible;
		base.ShowInTaskbar = true;
		Activate();
		Interaction.MsgBox(e + ".", MsgBoxStyle.Critical, "Critical error");
		Environment.Exit(0);
	}
}
