using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Core.Interfaces;
using System.Text.RegularExpressions;
using Tweetinvi.Core.Interfaces.Models.Entities;
using Tweetinvi.Core.Enum;

namespace TweetListener2
{
    /// <summary>
    /// hack used to import tweets from a file without saving all the data, but still incorporating them in all the systems used by the program - because the program uses ITweet
    /// </summary>
    public class CustomTweetFormat : ITweet
    {

        DateTime created_at;
        string id_str, in_reply_to_status_id_str, in_reply_to_user_id_str, user_screen_name, user_id_str, text;
        Language lang;
        int retweets = -1;
        long id;

        List<CustomHashtagFormat> hashtags = new List<CustomHashtagFormat>();

        CustomUserFormat user;

        /// <summary>
        /// generate fake tweet from data, only requires data that is used to display in tweet viewer. this can easily break, please consider it a quickfix.
        /// </summary>
        public CustomTweetFormat(DateTime created_at, string id_str, string in_reply_to_status_id_str, string in_reply_to_user_id_str, Language lang, int retweets, string user_screen_name, string user_id_str, string text)
        {
            this.created_at = created_at;
            this.id_str = id_str;
            this.id = long.Parse(id_str);
            this.in_reply_to_status_id_str = in_reply_to_status_id_str;
            this.in_reply_to_user_id_str = in_reply_to_user_id_str;
            this.lang = lang;
            this.retweets = retweets;
            this.user = new CustomUserFormat();
            this.user.ScreenName = user_screen_name;
            this.user.IdStr = user_id_str;
            this.text = text;

            // extract hashtags, based on http://stackoverflow.com/questions/1563844/best-hashtag-regex
            var matches = Regex.Matches(text, @"\B#\w\w+");
            hashtags.Clear();
            for (int i = 0; i < matches.Count; i++) {
                hashtags.Add(new CustomHashtagFormat(matches[i].Value.Replace("#", "")));
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

        public IUser CreatedBy
        {
            get { return user; }
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
            get { return lang; }
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

        public int PublishedTweetLength
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public long? QuotedStatusId
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string QuotedStatusIdStr
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ITweet QuotedTweet
        {
            get
            {
                throw new NotImplementedException();
            }
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

        public int CalculateLength(bool willBePublishedWithMedia)
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

    public class CustomUserFormat : IUser
    {

        public bool BlockUser()
        {
            throw new NotImplementedException();
        }

        public List<IUser> Contributees
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

        public List<IUser> Contributors
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

        public bool ContributorsEnabled
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime CreatedAt
        {
            get { throw new NotImplementedException(); }
        }

        public bool DefaultProfile
        {
            get { throw new NotImplementedException(); }
        }

        public bool DefaultProfileImage
        {
            get { throw new NotImplementedException(); }
        }

        public string Description
        {
            get { throw new NotImplementedException(); }
        }

        public IUserEntities Entities
        {
            get { throw new NotImplementedException(); }
        }

        public int FavouritesCount
        {
            get { throw new NotImplementedException(); }
        }

        public bool FollowRequestSent
        {
            get { throw new NotImplementedException(); }
        }

        public List<long> FollowerIds
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

        public List<IUser> Followers
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

        public int FollowersCount
        {
            get { throw new NotImplementedException(); }
        }

        public bool Following
        {
            get { throw new NotImplementedException(); }
        }

        public List<long> FriendIds
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

        public List<IUser> Friends
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

        public int FriendsCount
        {
            get { throw new NotImplementedException(); }
        }

        public List<ITweet> FriendsRetweets
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

        public bool GeoEnabled
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IUser> GetContributees(bool createContributeeList = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IUser> GetContributors(bool createContributorList = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITweet> GetFavorites(int maximumNumberOfTweets = 40)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<long> GetFollowerIds(int maxFriendsToRetrieve = 5000)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IUser> GetFollowers(int maxFriendsToRetrieve = 250)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<long> GetFriendIds(int maxFriendsToRetrieve = 5000)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IUser> GetFriends(int maxFriendsToRetrieve = 250)
        {
            throw new NotImplementedException();
        }

        public System.IO.Stream GetProfileImageStream(Tweetinvi.Core.Enum.ImageSize imageSize = Tweetinvi.Core.Enum.ImageSize.normal)
        {
            throw new NotImplementedException();
        }

        public IRelationshipDetails GetRelationshipWith(IUser user)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITweet> GetUserTimeline(Tweetinvi.Core.Interfaces.Parameters.IUserTimelineParameters timelineParameters)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITweet> GetUserTimeline(int maximumNumberOfTweets = 40)
        {
            throw new NotImplementedException();
        }

        public long Id
        {
            get { return long.Parse(idStr); }
        }

        private string idStr;
        public string IdStr
        {
            get { return idStr; }
            set { idStr = value; }
        }

        public bool IsTranslator
        {
            get { throw new NotImplementedException(); }
        }

        public Tweetinvi.Core.Enum.Language Language
        {
            get { throw new NotImplementedException(); }
        }

        public int ListedCount
        {
            get { throw new NotImplementedException(); }
        }

        public string Location
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public bool Notifications
        {
            get { throw new NotImplementedException(); }
        }

        public string ProfileBackgroundColor
        {
            get { throw new NotImplementedException(); }
        }

        public string ProfileBackgroundImageUrl
        {
            get { throw new NotImplementedException(); }
        }

        public string ProfileBackgroundImageUrlHttps
        {
            get { throw new NotImplementedException(); }
        }

        public bool ProfileBackgroundTile
        {
            get { throw new NotImplementedException(); }
        }

        public string ProfileBannerURL
        {
            get { throw new NotImplementedException(); }
        }

        public string ProfileImageFullSizeUrl
        {
            get { throw new NotImplementedException(); }
        }

        public string ProfileImageUrl
        {
            get { throw new NotImplementedException(); }
        }

        public string ProfileImageUrlHttps
        {
            get { throw new NotImplementedException(); }
        }

        public string ProfileLinkColor
        {
            get { throw new NotImplementedException(); }
        }

        public string ProfileSidebarBorderColor
        {
            get { throw new NotImplementedException(); }
        }

        public string ProfileSidebarFillColor
        {
            get { throw new NotImplementedException(); }
        }

        public string ProfileTextColor
        {
            get { throw new NotImplementedException(); }
        }

        public bool ProfileUseBackgroundImage
        {
            get { throw new NotImplementedException(); }
        }

        public bool Protected
        {
            get { throw new NotImplementedException(); }
        }

        public bool ReportUserForSpam()
        {
            throw new NotImplementedException();
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

        private string screen_name;
        public string ScreenName
        {
            get { return screen_name; }
            set { screen_name = value; }
        }

        public bool ShowAllInlineMedia
        {
            get { throw new NotImplementedException(); }
        }

        public Tweetinvi.Core.Interfaces.DTO.ITweetDTO Status
        {
            get { throw new NotImplementedException(); }
        }

        public int StatusesCount
        {
            get { throw new NotImplementedException(); }
        }

        public string TimeZone
        {
            get { throw new NotImplementedException(); }
        }

        public List<ITweet> Timeline
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

        public List<ITweet> TweetsRetweetedByFollowers
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

        public bool UnBlockUser()
        {
            throw new NotImplementedException();
        }

        public string Url
        {
            get { throw new NotImplementedException(); }
        }

        public Tweetinvi.Core.Interfaces.DTO.IUserDTO UserDTO
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

        public Tweetinvi.Core.Interfaces.Models.IUserIdentifier UserIdentifier
        {
            get { throw new NotImplementedException(); }
        }

        public int? UtcOffset
        {
            get { throw new NotImplementedException(); }
        }

        public bool Verified
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

        long Tweetinvi.Core.Interfaces.Models.IUserIdentifier.Id
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

        string Tweetinvi.Core.Interfaces.Models.IUserIdentifier.IdStr
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

        string Tweetinvi.Core.Interfaces.Models.IUserIdentifier.ScreenName
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

        public string ProfileImageUrl400x400
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Task<bool> BlockAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IUser>> GetContributeesAsync(bool createContributeeList = false)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IUser>> GetContributorsAsync(bool createContributorList = false)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ITweet>> GetFavoritesAsync(int maximumTweets = 40)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<long>> GetFollowerIdsAsync(int maxFriendsToRetrieve = 5000)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IUser>> GetFollowersAsync(int maxFriendsToRetrieve = 250)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<long>> GetFriendIdsAsync(int maxFriendsToRetrieve = 5000)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IUser>> GetFriendsAsync(int maxFriendsToRetrieve = 250)
        {
            throw new NotImplementedException();
        }

        public Task<System.IO.Stream> GetProfileImageStreamAsync(Tweetinvi.Core.Enum.ImageSize imageSize = Tweetinvi.Core.Enum.ImageSize.normal)
        {
            throw new NotImplementedException();
        }

        public Task<IRelationshipDetails> GetRelationshipWithAsync(IUser user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ITweet>> GetUserTimelineAsync(Tweetinvi.Core.Interfaces.Parameters.IUserTimelineParameters timelineRequestParameters)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ITweet>> GetUserTimelineAsync(int maximumTweet = 40)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UnBlockAsync()
        {
            throw new NotImplementedException();
        }

        public bool Equals(IUser other)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITwitterList> GetOwnedLists(int maximumNumberOfListsToRetrieve)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITwitterList> GetSubscribedLists(int maximumNumberOfListsToRetrieve = 1000)
        {
            throw new NotImplementedException();
        }
    }

}
