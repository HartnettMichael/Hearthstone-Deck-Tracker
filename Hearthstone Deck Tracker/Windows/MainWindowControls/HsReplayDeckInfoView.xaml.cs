using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.HsReplay;
using Hearthstone_Deck_Tracker.Utility;
using Hearthstone_Deck_Tracker.Utility.MVVM;
using LiveCharts;
using LiveCharts.Defaults;

namespace Hearthstone_Deck_Tracker.Windows.MainWindowControls
{
	/// <summary>
	/// Interaction logic for HsReplayDeckInfoView.xaml
	/// </summary>
	public partial class HsReplayDeckInfoView : UserControl
	{
		public HsReplayDeckInfoView()
		{
			InitializeComponent();
		}
		public void SetDeck(Deck deck) => ((HsReplayDeckInfoViewModel)DataContext).Deck = deck;
	}

	public class HsReplayDeckInfoViewModel : ViewModel
	{
		private Deck _deck;
		private double _winrate;
		private bool _hasHsReplayData;

		public HsReplayDeckInfoViewModel()
		{
			HsReplayDecks.OnLoaded += () =>
			{
				HasHsReplayData = !string.IsNullOrEmpty(ShortId) && HsReplayDecks.AvailableDecks.Contains(ShortId);
			};
		}

		public Deck Deck
		{
			get => _deck;
			set
			{
				_deck = value;
				ShortId = _deck?.GetSelectedDeckVersion().ShortId;
				OnPropertyChanged();
				UpdateWinrate();
				HasHsReplayData = !string.IsNullOrEmpty(ShortId) && HsReplayDecks.AvailableDecks.Contains(ShortId);
			}
		}

		public string ShortId { get; set; }

		public bool HasHsReplayData
		{
			get => _hasHsReplayData;
			set
			{
				_hasHsReplayData = value; 
				OnPropertyChanged();
			}
		}

		public void UpdateWinrate()
		{
			if(Deck == null)
				return;

			var games = Deck.DeckStats.Games.Count;
			var wins = Deck.DeckStats.Games.Count(x => x.Result == GameResult.Win);

			Winrate = games > 0 ? Math.Round(100.0 * wins / games) : 0;
		}

		public double Winrate
		{
			get => _winrate;
			set
			{
				_winrate = value; 
				OnPropertyChanged();
			}
		}

		public ICommand OpenDeckPageCommand	=> new Command(() =>
		{
			if(Deck?.ShortId != null)
				Helper.TryOpenUrl($"https://hsreplay.net/decks/{ShortId}/?utm_source=hdt&utm_medium=client&utm_campaign=mulliganguide");
		});

		public Func<double, string> EmptyFormatter { get; } = val => string.Empty;
	}
}
