using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Core.Interfaces;
using System.Text.RegularExpressions;
using Tweetinvi.Core.Interfaces.Models.Entities;

namespace WPFTwitter
{
	public class CustomTweetFormat : ITweet
	{

		DateTime created_at;
		string id_str, in_reply_to_status_id_str, in_reply_to_user_id_str, lang, user_screen_name, user_id_str, text;
		int retweets = -1;
		long id;

		List<CustomHashtagFormat> hashtags = new List<CustomHashtagFormat>();

		/// <summary>
		/// generate fake tweet from data, only requires data that is used to display in tweet viewer. this can easily break, please consider it a quickfix.
		/// </summary>
		public CustomTweetFormat(DateTime created_at, string id_str, string in_reply_to_status_id_str, string in_reply_to_user_id_str, string lang, int retweets, string user_screen_name, string user_id_str, string text)
		{
			this.created_at = created_at;
			this.id_str = id_str;
			this.id = long.Parse(id_str);
			this.in_reply_to_status_id_str = in_reply_to_status_id_str;
			this.in_reply_to_user_id_str = in_reply_to_user_id_str;
			this.lang = lang;
			this.retweets = retweets;
			this.user_screen_name = user_screen_name;
			this.user_id_str = user_id_str;
			this.text = text;

			// extract hashtags, based on http://stackoverflow.com/questions/1563844/best-hashtag-regex
			var matches = Regex.Matches(text, @"\B#\w\w+");
			hashtags.Clear();
			for (int i = 0; i < matches.Count; i++) {
				hashtags.Add(new CustomHashtagFormat(matches[i].Value.Replace("#","")));
			}
		}


		public Tweetinvi.Core.Interfaces.DTO.IMedia AddMedia(byte[] data, string name = null)
		{
			throw new NotImplementedException();
		}

		public Tweetinvi.Core.Interfaces.DTO.IMedia AddMediaAsAClone(Tweetinvi.Core.Interfaces.DTO.IMedia media)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<long> Contributors
		{
			get { throw new NotImplementedException(); }
		}

		public int[] ContributorsIds
		{
			get { throw new NotImplementedException(); }
		}

		public Tweetinvi.Core.Interfaces.Models.ICoordinates Coordinates
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public DateTime CreatedAt
		{
			get { return created_at; }
		}

		public IUser Creator
		{
			get { throw new NotImplementedException(); }
		}

		public Tweetinvi.Core.Interfaces.Models.ITweetIdentifier CurrentUserRetweetIdentifier
		{
			get { throw new NotImplementedException(); }
		}

		public bool Destroy()
		{
			throw new NotImplementedException();
		}

		public Tweetinvi.Core.Interfaces.Models.Entities.ITweetEntities Entities
		{
			get { throw new NotImplementedException(); }
		}

		public void Favourite()
		{
			throw new NotImplementedException();
		}

		public int FavouriteCount
		{
			get { throw new NotImplementedException(); }
		}

		public bool Favourited
		{
			get { throw new NotImplementedException(); }
		}

		public string FilterLevel
		{
			get { throw new NotImplementedException(); }
		}

		public Tweetinvi.Core.Interfaces.Models.IOEmbedTweet GenerateOEmbedTweet()
		{
			throw new NotImplementedException();
		}

		public List<ITweet> GetRetweets()
		{
			throw new NotImplementedException();
		}

		public List<Tweetinvi.Core.Interfaces.Models.Entities.IHashtagEntity> Hashtags
		{
			get
			{
				return hashtags.ToList<IHashtagEntity>();
			}
		}

		public string InReplyToScreenName
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public long? InReplyToStatusId
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string InReplyToStatusIdStr
		{
			get
			{
				return in_reply_to_status_id_str;
			}
			set
			{
				in_reply_to_status_id_str = value;
			}
		}

		public long? InReplyToUserId
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string InReplyToUserIdStr
		{
			get
			{
				return in_reply_to_user_id_str;
			}
			set
			{
				in_reply_to_user_id_str = value;
			}
		}

		public bool IsRetweet
		{
			get { return false; }
		}

		public bool IsTweetDestroyed
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsTweetPublished
		{
			get { throw new NotImplementedException(); }
		}

		public Tweetinvi.Core.Enum.Language Language
		{
			get { throw new NotImplementedException(); }
		}

		public int Length
		{
			get { throw new NotImplementedException(); }
		}

		public List<Tweetinvi.Core.Interfaces.Models.Entities.IMediaEntity> Media
		{
			get { throw new NotImplementedException(); }
		}

		public Tweetinvi.Core.Interfaces.Models.IPlace Place
		{
			get { throw new NotImplementedException(); }
		}

		public bool PossiblySensitive
		{
			get { throw new NotImplementedException(); }
		}

		public bool Publish()
		{
			throw new NotImplementedException();
		}

		public bool PublishInReplyTo(ITweet replyToTweet)
		{
			throw new NotImplementedException();
		}

		public bool PublishInReplyTo(long replyToTweetId)
		{
			throw new NotImplementedException();
		}

		public bool PublishReply(ITweet replyTweet)
		{
			throw new NotImplementedException();
		}

		public ITweet PublishReply(string text)
		{
			throw new NotImplementedException();
		}

		public ITweet PublishRetweet()
		{
			throw new NotImplementedException();
		}

		public bool PublishWithGeo(double longitude, double latitude)
		{
			throw new NotImplementedException();
		}

		public bool PublishWithGeo(Tweetinvi.Core.Interfaces.Models.ICoordinates coordinates)
		{
			throw new NotImplementedException();
		}

		public bool PublishWithGeoInReplyTo(double longitude, double latitude, ITweet replyToTweet)
		{
			throw new NotImplementedException();
		}

		public bool PublishWithGeoInReplyTo(double longitude, double latitude, long replyToTweetId)
		{
			throw new NotImplementedException();
		}

		public bool PublishWithGeoInReplyTo(Tweetinvi.Core.Interfaces.Models.ICoordinates coordinates, ITweet replyToTweet)
		{
			throw new NotImplementedException();
		}

		public int RetweetCount
		{
			get { return retweets; }
		}

		public bool Retweeted
		{
			get { throw new NotImplementedException(); }
		}

		public ITweet RetweetedTweet
		{
			get { throw new NotImplementedException(); }
		}

		public List<ITweet> Retweets
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Dictionary<string, object> Scopes
		{
			get { throw new NotImplementedException(); }
		}

		public string Source
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool Truncated
		{
			get { throw new NotImplementedException(); }
		}

		public Tweetinvi.Core.Interfaces.DTO.ITweetDTO TweetDTO
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public DateTime TweetLocalCreationDate
		{
			get { throw new NotImplementedException(); }
		}

		public int TweetRemainingCharacters()
		{
			throw new NotImplementedException();
		}

		public void UnFavourite()
		{
			throw new NotImplementedException();
		}

		public List<Tweetinvi.Core.Interfaces.Models.Entities.IUrlEntity> Urls
		{
			get { throw new NotImplementedException(); }
		}

		public List<Tweetinvi.Core.Interfaces.Models.Entities.IUserMentionEntity> UserMentions
		{
			get { throw new NotImplementedException(); }
		}

		public bool WithheldCopyright
		{
			get { throw new NotImplementedException(); }
		}

		public IEnumerable<string> WithheldInCountries
		{
			get { throw new NotImplementedException(); }
		}

		public string WithheldScope
		{
			get { throw new NotImplementedException(); }
		}

		public long Id
		{
			get { return id; }
		}

		public string IdStr
		{
			get { return id_str; }
		}

		public Task<bool> DestroyAsync()
		{
			throw new NotImplementedException();
		}

		public Task FavouriteAsync()
		{
			throw new NotImplementedException();
		}

		public Task<Tweetinvi.Core.Interfaces.Models.IOEmbedTweet> GenerateOEmbedTweetAsync()
		{
			throw new NotImplementedException();
		}

		public Task<List<ITweet>> GetRetweetsAsync()
		{
			throw new NotImplementedException();
		}

		public Task<bool> PublishAsync()
		{
			throw new NotImplementedException();
		}

		public Task<bool> PublishInReplyToAsync(ITweet replyToTweet)
		{
			throw new NotImplementedException();
		}

		public Task<bool> PublishInReplyToAsync(long replyToTweetId)
		{
			throw new NotImplementedException();
		}

		public Task<ITweet> PublishReplyAsync(string text)
		{
			throw new NotImplementedException();
		}

		public Task<ITweet> PublishRetweetAsync()
		{
			throw new NotImplementedException();
		}

		public Task<bool> PublishWithGeoAsync(double longitude, double latitude)
		{
			throw new NotImplementedException();
		}

		public Task<bool> PublishWithGeoAsync(Tweetinvi.Core.Interfaces.Models.ICoordinates coordinates)
		{
			throw new NotImplementedException();
		}

		public Task<bool> PublishWithGeoInReplyToAsync(double latitude, double longitude, ITweet replyToTweet)
		{
			throw new NotImplementedException();
		}

		public Task<bool> PublishWithGeoInReplyToAsync(double latitude, double longitude, long replyToTweetId)
		{
			throw new NotImplementedException();
		}

		public Task UnFavouriteAsync()
		{
			throw new NotImplementedException();
		}

		public bool Equals(ITweet other)
		{
			throw new NotImplementedException();
		}
	}

	public class CustomHashtagFormat : Tweetinvi.Core.Interfaces.Models.Entities.IHashtagEntity
	{
		string text;

		public CustomHashtagFormat(string hashtag)
		{
			this.text = hashtag;
		}

		public int[] Indices
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
			}
		}

		public bool Equals(Tweetinvi.Core.Interfaces.Models.Entities.IHashtagEntity other)
		{
			throw new NotImplementedException();
		}
	}

}
