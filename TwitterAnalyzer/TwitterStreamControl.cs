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
using System.Text.RegularExpressions;
using Tweetinvi.Core.Enum;

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
        private static Tweetinvi.Core.Interfaces.Streaminvi.ISampleStream sampleStream;
        private static Tweetinvi.Core.Interfaces.Streaminvi.IFilteredStream filteredStream;
        private static ITwitterCredentials twitterCert;
        public static UInt64 FilteredTweetCount = new UInt64();
        public static UInt64 SampleTweetCount = new UInt64();
        public static void Login(string usrKey, string usrSecret, string accessToken, string tokenSecret)
        {
            FilteredTweetCount = SampleTweetCount = 0;
            Auth.ApplicationCredentials = new TwitterCredentials(usrKey, usrSecret, accessToken, tokenSecret);
            twitterCert = new TwitterCredentials(usrKey, usrSecret, accessToken, tokenSecret);
            var user = User.GetAuthenticatedUser();
            Console.WriteLine("======LOGGED IN, CURRENT USER INFO======");
            Console.Write("Username: ");
            Console.WriteLine(user.Name + ", " + user.ScreenName);
            Console.Write("ID: ");
            Console.WriteLine(user.Id.ToString());
            Console.WriteLine("=======================================");
            Console.WriteLine();
        }

        /// <summary>
        /// Start a query stream
        /// </summary>
        /// <param name="tfSettingPath"></param>
        /// <param name="extraSettingPath"></param>
        public static async Task StartFilteredStream(string tfSettingPath, string resultPath)
        {
            filteredStream = Tweetinvi.Stream.CreateFilteredStream(twitterCert);
            StreamWriter writer = new StreamWriter(resultPath);
            for(int i = 0; i < File.ReadLines(tfSettingPath).Count(); i++)
            {
                filteredStream.AddTrack(File.ReadLines(tfSettingPath)
                    .Skip(i)
                    .Take(1)
                    .First()
                );
            }

            // When stream found one matching...
            filteredStream.MatchingTweetReceived += (sender, args) =>
            {
                FilteredTweetCount++;
                Console.WriteLine("[ "+DateTime.Now.ToLongDateString() + "  "+ DateTime.Now.ToLongTimeString()+" ]" 
                    +"  Found {0} filtered tweet(s).", FilteredTweetCount.ToString());

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
                    resultTweet = @""""+ DateTime.Now.ToShortDateString() + "  " + DateTime.Now.ToLongTimeString()+
                    @""",""" + args.Tweet.Text + @"""," +
                    args.Tweet.IsRetweet.ToString() + "," + args.Tweet.RetweetCount.ToString() + "," +
                    args.Tweet.PublishedTweetLength.ToString() + "," + args.Tweet.FavoriteCount.ToString()+ ",,," +
                    args.Tweet.Place.Country.ToString() + "," +args.Tweet.Place.FullName.ToString().Replace(","," ");
                }
                else // ...what a pity, no location information! So, just record something else available only!
                {
                    resultTweet = @"""" + DateTime.Now.ToShortDateString() + "  " + DateTime.Now.ToLongTimeString() +
                    @""",""" + args.Tweet.Text + @"""," + args.Tweet.IsRetweet.ToString() + "," + args.Tweet.RetweetCount.ToString() + "," +
                    args.Tweet.PublishedTweetLength.ToString() + "," + args.Tweet.FavoriteCount.ToString();
                }
                
                    
                writer.WriteLine(resultTweet);
            };

            filteredStream.StreamStopped += (sender, args) =>
            {
                writer.Flush();
                writer.Dispose();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("====================STOPPED====================");
                Console.WriteLine("TwitterAnalyzer Filter Stream stopped.");
                Console.WriteLine();

                if (args.Exception != null)
                {
                    Console.WriteLine("Exceptions: ");
                    Console.WriteLine(args.Exception.Message);
                    Console.WriteLine();
                    if (args.Exception.InnerException != null)
                    {
                        Console.WriteLine(args.Exception.InnerException.Message);
                        Console.WriteLine();
                    }
                }

                if(args.DisconnectMessage != null)
                {
                    Console.WriteLine("Reason: ");
                    Console.WriteLine(args.DisconnectMessage.Reason);
                    Console.WriteLine();
                }


                Console.WriteLine("====================STOPPED====================");

            };

            filteredStream.StreamPaused += (sender, args) =>
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("====================PAUSED====================");
                Console.WriteLine("TwitterAnalyzer Filter Stream paused.");
                Console.WriteLine("====================PAUSED====================");
            };

           await filteredStream.StartStreamMatchingAllConditionsAsync();
            
        }


        public async static Task StartSampleStream(string tfSettingPath, string resultPath)
        {

            sampleStream = Tweetinvi.Stream.CreateSampleStream(twitterCert);
            sampleStream.AddTweetLanguageFilter(Language.English);

            StreamWriter writer = new StreamWriter(resultPath);

            sampleStream.TweetReceived += (sender, args) =>
            {
                SampleTweetCount++;
                Console.WriteLine("[ " + DateTime.Now.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString() + " ]"
                    + "  Found {0} sample tweets.", SampleTweetCount.ToString());

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
                    resultTweet = @"""" + DateTime.Now.ToShortDateString() + "  " + DateTime.Now.ToLongTimeString() +
                    @""",""" + args.Tweet.Text + @"""," +
                    args.Tweet.IsRetweet.ToString() + "," + args.Tweet.RetweetCount.ToString() + "," +
                    args.Tweet.PublishedTweetLength.ToString() + "," + args.Tweet.FavoriteCount.ToString() + ",,," +
                    args.Tweet.Place.Country.ToString() + "," + args.Tweet.Place.FullName.ToString().Replace(",", " ");
                }
                else // ...what a pity, no location information! So, just record something else available only!
                {
                    resultTweet = @"""" + DateTime.Now.ToShortDateString() + "  " + DateTime.Now.ToLongTimeString() +
                    @""",""" + args.Tweet.Text + @"""," + args.Tweet.IsRetweet.ToString() + "," + args.Tweet.RetweetCount.ToString() + "," +
                    args.Tweet.PublishedTweetLength.ToString() + "," + args.Tweet.FavoriteCount.ToString();
                }

                
                writer.WriteLine(resultTweet);
            };


            sampleStream.StreamStopped += (sender, args) =>
            {
                writer.Flush();
                writer.Dispose();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("====================STOPPED====================");
                Console.WriteLine("TwitterAnalyzer Filter Stream stopped.");
                Console.WriteLine();

                if (args.Exception != null)
                {
                    Console.WriteLine("Exceptions: ");
                    Console.WriteLine(args.Exception.Message);
                    Console.WriteLine();
                    if (args.Exception.InnerException != null)
                    {
                        Console.WriteLine(args.Exception.InnerException.Message);
                        Console.WriteLine();
                    }
                }

                if (args.DisconnectMessage != null)
                {
                    Console.WriteLine("Reason: ");
                    Console.WriteLine(args.DisconnectMessage.Reason);
                    Console.WriteLine();
                }

                TwitterStatistic.GetStatisticOpt(tfSettingPath, resultPath, SampleTweetCount.ToString());
                Console.WriteLine("====================STOPPED====================");

            };

            await sampleStream.StartStreamAsync();

        }


        public static void StopAllStreams()
        {
            sampleStream.StopStream();
            filteredStream.StopStream();
        }


        

    }
}
