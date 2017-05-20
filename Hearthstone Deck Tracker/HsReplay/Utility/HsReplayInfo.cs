using System.ComponentModel;
using System.Runtime.CompilerServices;
using Hearthstone_Deck_Tracker.Annotations;

namespace Hearthstone_Deck_Tracker.HsReplay.Utility
{
	public class HsReplayInfo : INotifyPropertyChanged
	{
		private string _uploadId;

		public HsReplayInfo()
		{
			
		}

		public HsReplayInfo(string uploadId)
		{
			UploadId = uploadId;
		}

		public string UploadId
		{
			get => _uploadId;
			set
			{
				_uploadId = value; 
				OnPropertyChanged();
				OnPropertyChanged(nameof(Uploaded));
			}
		}

		public int UploadTries { get; set; }

		public bool Unsupported { get; set; }

		public bool Uploaded => !string.IsNullOrEmpty(UploadId);

		public string Url => (string.IsNullOrEmpty(ReplayUrl) ? $"https://hsreplay.net/uploads/upload/{UploadId}/" : ReplayUrl) + "?utm_source=hdt&utm_medium=client&utm_campaign=replay";

		public string ReplayUrl { get; set; }

		public void UploadTry() => UploadTries++;

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
