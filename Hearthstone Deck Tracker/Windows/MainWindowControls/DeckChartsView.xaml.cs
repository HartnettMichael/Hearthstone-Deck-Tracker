using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Stats;
using Hearthstone_Deck_Tracker.Utility.MVVM;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace Hearthstone_Deck_Tracker.Windows.MainWindowControls
{
	/// <summary>
	/// Interaction logic for DeckChartsView.xaml
	/// </summary>
	public partial class DeckChartsView : UserControl
	{
		public DeckChartsView()
		{
			InitializeComponent();
		}

		public void SetDeck(Deck deck) => ((DeckChartsViewModel)DataContext).Deck = deck;
	}

	public class DeckChartsViewModel : ViewModel
	{
		private Deck _deck;

		public Deck Deck
		{
			get => _deck;
			set
			{
				_deck = value;
				OnPropertyChanged();
				Games = _deck?.DeckStats.Games.Where(x => x.BelongsToDeckVerion(_deck.GetSelectedDeckVersion()))
					.OrderByDescending(x => x.StartTime).ToList();
				HasData = Games?.Any() ?? false;
				UpdateWinrate();
			}
		}

		public List<GameStats> Games
		{
			get => _games;
			set
			{
				_games = value;
				OnPropertyChanged();
			}
		}

		public int Wins
		{
			get => _wins;
			set
			{
				_wins = value; 
				OnPropertyChanged();
			}
		}

		public int Losses
		{
			get => _losses;
			set
			{
				_losses = value; 
				OnPropertyChanged();
			}
		}

		public bool HasData
		{
			get => _hasData;
			set
			{
				_hasData = value; 
				OnPropertyChanged();
			}
		}

		public DeckChartsViewModel()
		{
			OpponentCollection = new SeriesCollection();
			SeriesCollection = new SeriesCollection();
			PointLabel = p => p.Participation == 0 ? "" : $"{Math.Round(p.Participation * 100, 0)}%";
			var series = _playerClasses.Select(p => new PieSeries
			{
				Title = p,
				Values = new ChartValues<ObservableValue> { new ObservableValue(0) },
				Fill = new SolidColorBrush(Helper.GetClassColor(p, true)),
				Foreground = Brushes.Black,
				LabelPoint = PointLabel,
				DataLabels = true
			});
			OpponentCollection.AddRange(series);
			var foo = _playerClasses.Select(p => new StackedRowSeries
			{
				Values = new ChartValues<ObservableValue> {new ObservableValue(0)},
				StackMode = StackMode.Percentage,
				DataLabels = true,
				Fill = new SolidColorBrush(Helper.GetClassColor(p, true)),
				LabelPoint = x => $"{Math.Round(x.Participation * 100)}%",
				Title = p,
				Foreground = p == "Priest" || p == "Rogue" ? Brushes.Black : Brushes.White,
			});
			SeriesCollection.AddRange(foo);
		}

		public void UpdateWinrate()
		{
			if(Deck == null)
				return;

			var wins = 0;
			var losses = 0;
			var opponents = _playerClasses.ToDictionary(x => x, x => 0);

			foreach(var game in Deck.DeckStats.Games)
			{
				if(opponents.ContainsKey(game.OpponentHero))
					opponents[game.OpponentHero]++;
				if(game.Result == GameResult.Win)
					wins++;
				else if(game.Result == GameResult.Loss)
					losses++;
			}

			foreach(var series in OpponentCollection)
				((ObservableValue)series.Values[0]).Value = opponents[series.Title];

			foreach(var series in SeriesCollection)
				((ObservableValue)series.Values[0]).Value = opponents[series.Title];

			var total = wins + losses;
			Wins = wins;
			Losses = losses;
			WinrateTotal = total > 0 ? Math.Round(100.0 * wins/total) : 0;
		}

		public Func<ChartPoint, string> PointLabel { get; }

		public double WinrateTotal
		{
			get => _winrateTotal;
			set
			{
				_winrateTotal = value;
				OnPropertyChanged();
			}
		}

		public Func<double, string> EmptyFormatter { get; } = val => string.Empty;

		private readonly string[] _playerClasses =
			{ "Druid", "Hunter", "Mage", "Paladin", "Priest", "Shaman", "Rogue", "Warlock", "Warrior" };

		private double _winrateFirst;
		private double _winrateSecond;

		public SeriesCollection OpponentCollection { get; }

		public SeriesCollection SeriesCollection { get; }

		private string[] _axisLabels;
		private double _winrateTotal;
		private bool _hasData;
		private List<GameStats> _games;
		private int _wins;
		private int _losses;

		public string[] AxisLabels
		{
			get => _axisLabels;
			set
			{
				_axisLabels = value; 
				OnPropertyChanged();
			}
		}
	}
}
