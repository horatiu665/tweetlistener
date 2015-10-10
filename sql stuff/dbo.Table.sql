CREATE TABLE [dbo].[Table]
(
	[id] INT NOT NULL PRIMARY KEY, 
    [tweet_id_str] NVARCHAR(64) NULL, 
    [tweet] NVARCHAR(200) NULL,
	[created_at] datetime NULL,
	[user_id_str] NVARCHAR(32) NULL, 
    [user_name] NVARCHAR(16) NULL, 
	[in_reply_to_status_id_str] NVARCHAR(64) NULL, 
    [in_reply_to_user_id_str] NVARCHAR(32) NULL, 
    [lang] NVARCHAR(2) NULL, 
    [retweet_count] INT NULL, 
    
)
