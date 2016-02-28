using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Streams;
using Tweetinvi;
using Tweetinvi.Core;
using Tweetinvi.Controllers;
using Tweetinvi.Core.Credentials;
using System.IO;
using System.Threading;

namespace TwitterAnalyzer
{
    class TwitterStreamControl
    {
        /// <summary>
        /// Get yourself logged in. 
        /// Please go to Twitter Website https://apps.twitter.com for more information.
        /// </summary>
        /// <param name="usrKey"></param>
        /// <param name="usrSecret"></param>
        /// <param name="accessToken"></param>
        /// <param name="tokenSecret"></param>
        public static void Login(string usrKey, string usrSecret, string accessToken, string tokenSecret)
        {
            Auth.ApplicationCredentials = new TwitterCredentials(usrKey, usrSecret, accessToken, tokenSecret);
            var user = User.GetLoggedUser();
            Console.WriteLine("======CURRENT USER INFO======");
            Console.Write("Username: ");
            Console.WriteLine(user.Name + ", " + user.ScreenName);
            Console.Write("ID: ");
            Console.WriteLine(user.Id.ToString());
            Console.WriteLine("=============================");
            Console.WriteLine();
        }

        /// <summary>
        /// Start a query stream
        /// </summary>
        /// <param name="tfSettingPath"></param>
        /// <param name="extraSettingPath"></param>
        public static void StartStream(string tfSettingPath, string resultPath)
        {
            StreamWriter writer = new StreamWriter(resultPath);
            var stream = Tweetinvi.Stream.CreateFilteredStream();
            for(int i = 0; i < File.ReadLines(tfSettingPath).Count(); i++)
            {
                stream.AddTrack(File.ReadLines(tfSettingPath)
                    .Skip(i)
                    .Take(1)
                    .First()
                );
            }

            // When stream found one matching 
            stream.MatchingTweetReceived += (sender, args) =>
            {
                Console.WriteLine("Thread " + Thread.CurrentThread.ManagedThreadId.ToString() 
                    + "has found 1 tweet meets the requirement:" + args.MatchingTracks);

                /* 
                    Record these following information:
                      - Matching Track, which kind queried word matches?
                      - Is it retweeted? True or False?
                      - Retweet count
                      - Length of this tweet
                      - The user name who created it
                      - The tweet's country the user sent the tweet 
                      - The tweet's full name of the place where the user sent the tweet
                */
                string resultTweet = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                    args.MatchingTracks,
                    args.Tweet.Text,
                    args.Tweet.IsRetweet.ToString(),
                    args.Tweet.RetweetCount,
                    args.Tweet.PublishedTweetLength,
                    args.Tweet.CreatedBy.Name,
                    args.Tweet.Place.Country,
                    args.Tweet.Place.FullName
                 );
                    
                writer.WriteLine(resultTweet);
            };

            stream.StartStreamMatchingAllConditions();
            
        }

        public static void StopStream()
        {

        }

    }
}
