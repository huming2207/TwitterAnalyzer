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
        private static ITwitterCredentials twitterCert;
        public static void Login(string usrKey, string usrSecret, string accessToken, string tokenSecret)
        {
            Auth.ApplicationCredentials = new TwitterCredentials(usrKey, usrSecret, accessToken, tokenSecret);
            twitterCert = new TwitterCredentials(usrKey, usrSecret, accessToken, tokenSecret);
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
        
        //private static Tweetinvi.Core.Interfaces.Streaminvi.IFilteredStream stream = Tweetinvi.Stream.CreateFilteredStream(twitterCert);
        public static void StartStream(string tfSettingPath, string resultPath)
        {
            var stream = Tweetinvi.Stream.CreateFilteredStream(twitterCert);
            StreamWriter writer = new StreamWriter(resultPath);
            for(int i = 0; i < File.ReadLines(tfSettingPath).Count(); i++)
            {
                stream.AddTrack(File.ReadLines(tfSettingPath)
                    .Skip(i)
                    .Take(1)
                    .First()
                );
            }

            // When stream found one matching...
            stream.MatchingTweetReceived += (sender, args) =>
            {
                Console.WriteLine("Found 1 tweet meets the requirement.");

                /* 
                    Record these following information:
                      - Is it retweeted? True or False?
                      - Retweet count
                      - Length of this tweet
                      - The user name who created it
                      - The tweet's country the user sent the tweet 
                      - The tweet's full name of the place where the user sent the tweet
                */

                string resultTweet = string.Empty;
                if (args.Tweet.Place != null) // Try recording all location info.
                {
                    resultTweet = @"""" + args.Tweet.Text.ToString().Replace("\r\n", "") + @"""," +
                    args.Tweet.IsRetweet.ToString() + "," + args.Tweet.RetweetCount.ToString() + "," +
                    args.Tweet.PublishedTweetLength.ToString() + "," + args.Tweet.CreatedBy.Name.ToString() + "," +
                    args.Tweet.Place.Country.ToString() + "," +args.Tweet.Place.FullName.ToString();
                }
                
                else //...what a pity, no location information! So, just record something else available only!
                {
                    resultTweet = @"""" + args.Tweet.Text.ToString().Replace("\r\n", "") + @"""," +
                    args.Tweet.IsRetweet.ToString() + "," + args.Tweet.RetweetCount.ToString() + "," +
                    args.Tweet.PublishedTweetLength.ToString() + "," + args.Tweet.CreatedBy.Name.ToString();
                }
                
                /* sultTweet = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                    args.Tweet.Text.ToString(),
                    args.Tweet.IsRetweet.ToString(),
                    args.Tweet.RetweetCount.ToString(),
                    args.Tweet.PublishedTweetLength.ToString(),
                    args.Tweet.CreatedBy.Name.ToString(),
                    args.Tweet.Place.Country.ToString(),
                    args.Tweet.Place.FullName.ToString()
                 ); */
                    
                writer.WriteLine(resultTweet);
            };

            stream.StreamStopped += (sender, args) =>
            {
                writer.Flush();
                writer.Dispose();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("==========STOPPED==========");
                Console.WriteLine("TwitterAnalyzer stopped.");
                Console.WriteLine("Reason: ");
                Console.WriteLine(args.DisconnectMessage.Reason);
                Console.WriteLine();
                Console.WriteLine("Exceptions: ");
                Console.WriteLine(args.Exception.Message);
                Console.WriteLine();
                Console.WriteLine(args.Exception.InnerException.Message);
                Console.WriteLine("==========STOPPED==========");

            };

            stream.StreamPaused += (sender, args) =>
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("==========PAUSED==========");
                Console.WriteLine("TwitterAnalyzer paused.");
                Console.WriteLine("==========PAUSED==========");
            };

            stream.StartStreamMatchingAllConditions();
            
        }


    }
}
