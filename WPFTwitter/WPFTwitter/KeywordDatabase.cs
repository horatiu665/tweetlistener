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
		Log log;

		public bool ContinuousUpdate { get; set; }

		public List<string> GetUsableKeywords()
		{
			return keywordList.Where(kd => kd.UseKeyword).Select(kd => kd.Keyword).ToList();
		}

		public KeywordDatabase(Log log)
		{
			this.log = log;

			//keywordList.CollectionChanged += (s, a) => {
			//	if (keywordList != null) {
			//		keywordList.ClearLanguageModels();
			//	}
			//};
		}

		/// <summary>
		/// stores a list of the current keywords being looked for
		/// </summary>
		private KeywordListClass keywordList = new KeywordListClass();
		public KeywordListClass KeywordList
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


		public class KeywordListClass : RangeObservableCollection<KeywordData>
		{
			public void Set(ObservableCollection<KeywordData> newList)
			{
				this.Clear();
				this.AddRange(newList);
			}

			public void Set(List<KeywordData> newList)
			{
				this.Clear();
				this.AddRange(newList);
			}

			public void Set(IEnumerable<KeywordData> newList)
			{
				this.Clear();
				this.AddRange(newList);
			}

			/// <summary>
			/// updates keywordlist based on list of tweets
			/// </summary>
			public void UpdateCount(IEnumerable<TweetDatabase.TweetData> tweets)
			{
				foreach (var kData in this) {
					try {
						// count appearances in tweet
						var c = tweets.Count(td => td.ContainsHashtag(kData.Keyword));

						kData.Count = c;
					}
					catch { }
				}
			}

			public void ClearLanguageModels()
			{
				foreach (var k in this) {
					k.LanguageModel = null;
				}
			}
		}


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

			/// <summary>
			/// if it has been calculated, will not be null => can be used without recalculation.
			/// any change to the tweet population will nullify the language model and must be recalculated.
			/// </summary>
			private LanguageModel languageModel = null;
			public LanguageModel LanguageModel
			{
				get
				{
					return languageModel;
				}
				set
				{
					languageModel = value;
					if (PropertyChanged != null) {
						PropertyChanged(this, new PropertyChangedEventArgs("HasLanguageModel"));
					}
				}
			}

			public bool HasLanguageModel
			{
				get
				{
					return LanguageModel != null;
				}
			}


			public event PropertyChangedEventHandler PropertyChanged;
		}
	}
}
