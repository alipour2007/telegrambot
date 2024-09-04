using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MyTelegramBot
{
    public class MyBot
    {
        TelegramBotClient bot;

        Dictionary<long, UserState> userStates = new Dictionary<long, UserState>();


        class UserState
        {
            public Step CurrentStep { get; set; }
            public string Year { get; set; }
            public string Month { get; set; }
            public string Day { get; set; }
        }

        enum Step
        {
            None,
            WaitingToEnterYear,
            WaitingToEnterMonth,
            WaitingToEnterDay,
            Completed
        }

        public async Task StartListeningAsync()
        {
            bot = new TelegramBotClient("7326036746:AAEU2V6-Smbv5F933BjeZKTDydADBP7DsHc");
            var me = await bot.GetMeAsync();

            await bot.DropPendingUpdatesAsync();

            bot.OnMessage += Bot_OnMessage;
            bot.OnUpdate += Bot_OnUpdate;
        }

        private async Task Bot_OnUpdate(Telegram.Bot.Types.Update update)
        {
            userStates[update.CallbackQuery.Message.Chat.Id].Month = update.CallbackQuery.Data;
            userStates[update.CallbackQuery.Message.Chat.Id].CurrentStep = Step.WaitingToEnterDay;
            await bot.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "لطفا روز تولد خود را وارد کنید");
        }

        private async Task Bot_OnMessage(Telegram.Bot.Types.Message message, Telegram.Bot.Types.Enums.UpdateType type)
        {

            if (userStates.ContainsKey(message.Chat.Id) == false)
            {
                userStates.Add(message.Chat.Id, new UserState { CurrentStep = Step.None });
            }

            if (message.Text == "/start")
            {
                await bot.SendTextMessageAsync(message.Chat.Id, "لطفا سال تولد خود را وارد کنید");
                userStates[message.Chat.Id] = new UserState { CurrentStep = Step.WaitingToEnterYear };
            }
            else
            {
                var state = userStates[message.Chat.Id];
                switch (state.CurrentStep)
                {
                    case Step.WaitingToEnterYear:
                        {
                            userStates[message.Chat.Id].Year = message.Text;
                            userStates[message.Chat.Id].CurrentStep = Step.WaitingToEnterMonth;

                            var months = new InlineKeyboardMarkup();
                            months.AddNewRow("فروردین", "اردیبهشت", "خرداد");
                            months.AddNewRow("تیر", "مرداد", "شهریور");
                            months.AddNewRow("مهر", "آبان", "آذر");
                            months.AddNewRow("دی", "بهمن", "اسفند");

                            await bot.SendTextMessageAsync(message.Chat.Id, "لطفا ماه تولد خود را وارد کنید", replyMarkup: months);

                        }
                        break;

                    case Step.WaitingToEnterMonth:
                        {
                            userStates[message.Chat.Id].Month = message.Text;
                            userStates[message.Chat.Id].CurrentStep = Step.WaitingToEnterDay;
                            await bot.SendTextMessageAsync(message.Chat.Id, "لطفا روز تولد خود را وارد کنید");

                        }
                        break;

                    case Step.WaitingToEnterDay:
                        {
                            userStates[message.Chat.Id].Day = message.Text;
                            userStates[message.Chat.Id].CurrentStep = Step.Completed;


                            var year = userStates[message.Chat.Id].Year;
                            var month = userStates[message.Chat.Id].Month;
                            var day = userStates[message.Chat.Id].Day;

                            await bot.SendTextMessageAsync(message.Chat.Id, "سن شما : 37  سال می باشد");

                        }
                        break;
                }
            }
        }
    }
}
