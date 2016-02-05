using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetListener2.Systems
{

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

        private int _count = 0;

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
