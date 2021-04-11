using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace SimpleRedisChat
{
    class Program
    {
        const string channel = "chat";
        const int port = 6379;
        static void Main(string[] args)
        {
            Console.Write("Enter your name: ");
            var userName = Console.ReadLine();

            var options = new ConfigurationOptions();
            options.EndPoints.Add("localhost", port);
            var connection = ConnectionMultiplexer.Connect(options);
            var pubsub= connection.GetSubscriber();
            var db = connection.GetDatabase();


            pubsub.Subscribe(channel, (channel,message) => Console.WriteLine(message));
            pubsub.Publish(channel,$"{userName}-брат, присоединился к чату");
            PrintOldMessages(db.ListRange("chat-messages"));
            while (true) 
            {
                var message = Console.ReadLine();
                db.ListRightPush("chat-messages",$"{DateTime.Now.ToString("g")} {userName}-брат: {message}");
                pubsub.Publish(channel, $"{DateTime.Now.ToString("g")} {userName}-брат: {message}");
            }
        }

        static void PrintOldMessages(RedisValue[] messeges) 
        {
            foreach (var m in messeges)
            {
                Console.WriteLine(m);
            }
        }
    }
}
