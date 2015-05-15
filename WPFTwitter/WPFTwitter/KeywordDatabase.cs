using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Core.Interfaces;
using System.Collections.ObjectModel;

namespace WPFTwitter
{
	public class KeywordDatabase
	{



		// keywords are added by expanding the original keywords, and each _expansion is expanding the next _expansion etc. until maxExpansionCount
		public class KeywordData
		{
			/// <summary>
			/// the actual hashtag/_keyword string.
			/// </summary>
			private string _keyword;

			public string Keyword
			{
				get { return _keyword; }
				set { _keyword = value; }
			}

			/// <summary>
			/// how many times since the original tweet were tweets with similar keywords pulled?
			/// </summary>
			private int _expansion;

			public int Expansion
			{
				get { return _expansion; }
				set { _expansion = value; }
			}

			private int _count = 1;

			public int Count
			{
				get { return _count; }
				set { _count = value; }
			}

			public KeywordData(string keyword, int generation)
			{
				this._keyword = keyword;
				this._expansion = generation;

			}

		}
	}
}
