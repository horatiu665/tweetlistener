using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Core.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WPFTwitter
{
	public class KeywordDatabase
	{
		public List<string> GetUsableKeywords()
		{
			return keywordList.Where(kd => kd.UseKeyword).Select(kd => kd.Keyword).ToList();
		}

		/// <summary>
		/// stores a list of the current keywords being looked for
		/// </summary>
		private KeywordDatabase.KeywordListClass keywordList;
		public KeywordDatabase.KeywordListClass KeywordList
		{
			get
			{
				return keywordList;
			}
			set
			{
				// should not set {} an observable collection because it breaks the binding.
				// instead should only mess around with private value. unoptimally: clear list and add each element.
				keywordList = value;
			}
		}


		public class KeywordListClass : ObservableCollection<KeywordData>
		{
			public void Set(ObservableCollection<KeywordData> newList)
			{
				this.Clear();
				foreach (var l in newList) {
					this.Add(l);
				}
			}

			public void Set(List<KeywordData> newList)
			{
				this.Clear();
				foreach (var l in newList) {
					this.Add(l);
				}
			}

			/// <summary>
			/// updates keyword list based on list of tweets
			/// </summary>
			public void Update(TweetDatabase.TweetList tweets)
			{
				foreach (var kData in this) {
					kData.Count = tweets.Count(td => td.ContainsHashtag(kData.Keyword));
				}
			}

		}


		// keywords are added by expanding the original keywords, and each _expansion is expanding the next _expansion etc. until maxExpansionCount
		public class KeywordData : INotifyPropertyChanged
		{
			/// <summary>
			/// the actual hashtag/_keyword string.
			/// </summary>
			private string _keyword;

			public string Keyword
			{
				get { return _keyword; }
				set
				{
					_keyword = value;
					if (PropertyChanged != null) {
						PropertyChanged(this, new PropertyChangedEventArgs("Keyword"));
					}
				}
			}

			/// <summary>
			/// how many times since the original tweet were tweets with similar keywords pulled?
			/// </summary>
			private int _expansion;

			public int Expansion
			{
				get { return _expansion; }
				set
				{
					_expansion = value;
					if (PropertyChanged != null) {
						PropertyChanged(this, new PropertyChangedEventArgs("Expansion"));
					}
				}
			}

			private int _count = 1;

			public int Count
			{
				get { return _count; }
				set
				{
					_count = value; 
					if (PropertyChanged != null) {
						PropertyChanged(this, new PropertyChangedEventArgs("Count"));
					}
				}
			}

			private bool useKeyword = true;

			public bool UseKeyword
			{
				get { return useKeyword; }
				set
				{
					useKeyword = value; 
					if (PropertyChanged != null) {
						PropertyChanged(this, new PropertyChangedEventArgs("UseKeyword"));
					}
				}
			}

			public KeywordData(string keyword, int generation)
			{
				this._keyword = keyword;
				this._expansion = generation;

			}


			public event PropertyChangedEventHandler PropertyChanged;
		}
	}
}
