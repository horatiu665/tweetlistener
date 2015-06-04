# Misc
This section contains what does not belong anywhere else: outcast documentation. Also stuff that is not proper enough to deserve its proper section. 

As soon as relevant stuff starts forming here, please move it to a proper section linked from somewhere.

## Optimizing tweet data for storage and analysis

*“Since in our cases we’re going to be re-broadcasting this out at an extremely high rate to all the streaming servers, we want to trim this down to conserve bandwidth. Thus we create a new JSON blob from a hash containing just the bare minimum to construct a tweet: tweet ID, text, and author info (permalink URLs are predictable and can be recreated with this info). This reduces the size by 10-20x.”* - [Emojitracker post on Medium.com](https://medium.com/@mroth/how-i-built-emojitracker-179cfd8238ac)
