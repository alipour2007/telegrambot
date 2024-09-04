// See https://aka.ms/new-console-template for more information
using MyTelegramBot;
using Telegram.Bot;
using Telegram.Bot.Types;

internal class Program
{
    private static async Task Main(string[] args)
    {
     
        MyBot bot = new MyBot();
        bot.StartListeningAsync();
        Console.ReadLine();
    }

 
}