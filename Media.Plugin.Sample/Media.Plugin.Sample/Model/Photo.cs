using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;


namespace Media.Plugin.Sample.Model
{
	// Auto Generated classes from VS Paste JSON as a class functionality
	// INotify property changed implemented in each class which is needed for displaying data on XAML page
	public class Caption : INotifyPropertyChanged
	{
		public string text { get; set; }
		public double confidence { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				if (value != text)
				{
					text = value;
					NotifyPropertyChanged();
				}
			}
		}

		public double Confidence
		{
			get
			{
				return confidence;
			}
			set
			{
				if (value != confidence)
				{
					confidence = value;
					NotifyPropertyChanged();
				}
			}
		}
	}

	public class Descriptions : INotifyPropertyChanged
	{
		public string[] tags { get; set; }
		public Caption[] captions { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public string[] Tags
		{
			get
			{
				return tags;
			}
			set
			{
				if (value != tags)
				{
					tags = value;
					NotifyPropertyChanged();
				}
			}
		}

		public Caption[] Captions
		{
			get
			{
				return captions;
			}
			set
			{
				if (value != captions)
				{
					captions = value;
					NotifyPropertyChanged();
				}
			}
		}
	}

	public class Metadata
	{
		public int width { get; set; }
		public int height { get; set; }
		public string format { get; set; }

	}

	public class Photo : INotifyPropertyChanged
	{
		public Descriptions description { get; set; }
		public string requestId { get; set; }
		public Metadata metadata { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public Photo() { }

		public Descriptions Description
		{
			get
			{
				return description;
			}
			set
			{
				if (value != description)
				{
					description = value;
					NotifyPropertyChanged();
				}
			}
		}
	}
}

