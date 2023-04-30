// See https://aka.ms/new-console-template for more information

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Uskov_cipher_Vigenere_Hill;

class Program
{
    public static List<User_> Users = new List<User_>();
    static void Main(string[] args)
    {
        SQLite.Read();
        var client = new TelegramBotClient("typeAPI");
        client.StartReceiving(Update, Error);
        int num = 0;
        TimerCallback tm = new TimerCallback(SQLite.Write);
        System.Threading.Timer timer = new System.Threading.Timer(tm, num, 0, 600000);
        Console.ReadLine();
    }
    async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        var message = update.Message;
        var answer = update.CallbackQuery;
        if (message != null)
        {
            if (message.Text != null)
            {
                if (HelpFunction.CheckErrorSymbol(message.Text)) { await botClient.SendTextMessageAsync(message.Chat.Id, "Увы я не знаю некоторые символы, повторите ввод."); }
                else if (message.Text == "/start")
                {
                    User_.GetIndex(message.Chat.Id);
                    InlineKeyboardMarkup inlineKeyboard = new(new[] {
                        InlineKeyboardButton.WithCallbackData(text: "Виженера", callbackData: "cipherVigenere"),
                        InlineKeyboardButton.WithCallbackData(text: "Хилла", callbackData: "cipherHill") });
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: $"Здравствуйте {message.Chat.FirstName ?? "человек без имени"}. Я могу шифровать и расшифровывать сообщения.\nВыберите метод шифрования.",
                        replyMarkup: inlineKeyboard);
                }
                else if (message.Text == "/cipher_vigenere")
                {
                    InlineKeyboardMarkup inlineKeyboard = new(new[] {
                        new []{ InlineKeyboardButton.WithCallbackData(text: "Ввести ключ", callbackData: "setKeyVigenere") },
                        new []{ InlineKeyboardButton.WithCallbackData(text: "Зашифровать", callbackData: "encVigenere") },
                        new []{ InlineKeyboardButton.WithCallbackData(text: "Расшифровать", callbackData: "decVigenere") } });
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Вы выбрали шифрование методом Виженера.",
                        replyMarkup: inlineKeyboard);
                }
                else if (message.Text == "/cipher_hill")
                {
                    InlineKeyboardMarkup inlineKeyboard = new(new[] {
                       new []{ InlineKeyboardButton.WithCallbackData(text: "Ввести ключ", callbackData: "setKeyHill") },
                       new []{ InlineKeyboardButton.WithCallbackData(text: "Зашифровать", callbackData: "encHill") },
                       new []{ InlineKeyboardButton.WithCallbackData(text: "Расшифровать", callbackData: "decHill") } });
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Вы выбрали шифрование методом Хилла.",
                        replyMarkup: inlineKeyboard);
                }
                else if (message.Text == "/help")
                {
                    string str = "";
                    switch (Users[User_.GetIndex(message.Chat.Id)].mode)
                    {
                        case 0:
                            str = "Я жду, чтобы вы выбрали команду или нажали на кнопку.";
                            break;
                        case 1:
                            str = "Я жду ввода ключа для метода Виженера.";
                            break;
                        case 2:
                            str = "Я жду ввода ключа для метода Хилла.";
                            break;
                        case 3:
                            str = "Я жду ввода открытого текста для Виженера.";
                            break;
                        case 4:
                            str = "Я жду ввода закрытого текста для Виженера.";
                            break;
                        case 5:
                            str = "Я жду ввода открытого текста для Хилла.";
                            break;
                        case 6:
                            str = "Я жду ввода закрытого текста для Хилла.";
                            break;
                    }
                    InlineKeyboardMarkup inlineKeyboard = new(new[] {
                       InlineKeyboardButton.WithCallbackData(text: "Виженера", callbackData: "getKeyVigenere"),
                       InlineKeyboardButton.WithCallbackData(text: "Хилла", callbackData: "getKeyHill")});
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: str + "\nВы можете посмотреть введённые ключи.",
                        replyMarkup: inlineKeyboard);
                }
                else
                {
                    var mode = Users[User_.GetIndex(message.Chat.Id)].mode;
                    if (mode == 0) { await botClient.SendTextMessageAsync(message.Chat.Id, "Простите, но я не знаю что делать, выберите команду или нажми на кнопки."); }
                    else if (mode == 1)
                    {
                        Users[User_.GetIndex(message.Chat.Id)].keyVigenere = message.Text;
                        InlineKeyboardMarkup inlineKeyboard = new(new[] {
                        new []{ InlineKeyboardButton.WithCallbackData(text: "Зашифровать", callbackData: "encVigenere") },
                        new []{ InlineKeyboardButton.WithCallbackData(text: "Расшифровать", callbackData: "decVigenere") } });
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Спасибо, ключ  для метода Виженера записал. Выберите действие.",
                            replyMarkup: inlineKeyboard);

                    }
                    else if (mode == 2)
                    {
                        InlineKeyboardMarkup inlineKeyboard = new(new[] {
                       new []{ InlineKeyboardButton.WithCallbackData(text: "Зашифровать", callbackData: "encHill") },
                       new []{ InlineKeyboardButton.WithCallbackData(text: "Расшифровать", callbackData: "decHill") } });
                        string check = CipherHill.SetKey(message.Chat.Id, message.Text);
                        if (!check.Contains("Увы"))
                        {
                            Users[User_.GetIndex(message.Chat.Id)].mode = 0;
                            check += Users[User_.GetIndex(message.Chat.Id)].keyHill + "\nВыберите действие для шифра Хилла.";
                        }
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: check,
                            replyMarkup: inlineKeyboard);
                    }
                    else if (mode == 3)
                    {
                        string encText = CipherVigenere.EncText(message.Chat.Id, message.Text);
                        { await botClient.SendTextMessageAsync(message.Chat.Id, $"Ваш зашифрованный текст по методу Виженера:\n{encText}\nМожете ввести ещё текст."); }
                    }
                    else if (mode == 4)
                    {
                        string decText = CipherVigenere.DecText(message.Chat.Id, message.Text);
                        { await botClient.SendTextMessageAsync(message.Chat.Id, $"Ваш расшифрованный текст по методу Виженера:\n{decText}\nМожете ввести ещё текст."); }
                    }
                    else if (mode == 5)
                    {
                        string encText = CipherHill.EncText(message.Chat.Id, message.Text);
                        { await botClient.SendTextMessageAsync(message.Chat.Id, $"Ваш зашифрованный текст по методу Хилла:\n{encText}\nМожете ввести ещё текст."); }
                    }
                    else if (mode == 6)
                    {
                        string decText = CipherHill.DecText(message.Chat.Id, message.Text);
                        { await botClient.SendTextMessageAsync(message.Chat.Id, $"Ваш расшифрованный текст по методу Хилла:\n{decText}\nМожете ввести ещё текст."); }
                    }
                    else { await botClient.SendTextMessageAsync(message.Chat.Id, "Похоже я сломался."); }
                }
            }
            else { await botClient.SendTextMessageAsync(message.Chat.Id, "Прости, но я не знаю что делать, выбери команду или нажми на кнопки."); }
        }
        else if (answer != null)
        {
            if (answer.Data == "setKeyVigenere")
            {
                Users[User_.GetIndex(answer.Message.Chat.Id)].mode = 1;
                await botClient.SendTextMessageAsync(answer.Message.Chat.Id, "Пожалуйста, введите ключ для метода Виженера.");
            }
            else if (answer.Data == "setKeyHill")
            {
                Users[User_.GetIndex(answer.Message.Chat.Id)].mode = 2;
                await botClient.SendTextMessageAsync(answer.Message.Chat.Id, "Пожалуйста, введите ключ для метода Хилла.");
            }
            else if (answer.Data == "encVigenere")
            {
                var keyVigenere = Users[User_.GetIndex(Convert.ToInt32(answer.Message.Chat.Id))].keyVigenere;
                if (keyVigenere.Length == 0)
                {
                    Users[User_.GetIndex(Convert.ToInt32(answer.Message.Chat.Id))].mode = 1;
                    await botClient.SendTextMessageAsync(answer.Message.Chat.Id, "Пожалуйста, введите ключ для метода Виженера.");
                }
                else
                {
                    Users[User_.GetIndex(Convert.ToInt32(answer.Message.Chat.Id))].mode = 3;
                    await botClient.SendTextMessageAsync(answer.Message.Chat.Id, "Пожалуйста, введите открытый текст для шифрования методом Виженера.");
                }
            }
            else if (answer.Data == "decVigenere")
            {
                var keyVigenere = Users[User_.GetIndex(Convert.ToInt32(answer.Message.Chat.Id))].keyVigenere;
                if (keyVigenere.Length == 0)
                {
                    Users[User_.GetIndex(Convert.ToInt32(answer.Message.Chat.Id))].mode = 1;
                    await botClient.SendTextMessageAsync(answer.Message.Chat.Id, "Пожалуйста, введите ключ для метода Виженера.");
                }
                else
                {
                    Users[User_.GetIndex(Convert.ToInt32(answer.Message.Chat.Id))].mode = 4;
                    await botClient.SendTextMessageAsync(answer.Message.Chat.Id, "Пожалуйста, введите закрытый текст для расшифровки методом Виженера.");
                }
            }
            else if (answer.Data == "encHill")
            {
                var keyVigenere = Users[User_.GetIndex(Convert.ToInt32(answer.Message.Chat.Id))].keyHill;
                if (keyVigenere.Length == 0)
                {
                    Users[User_.GetIndex(Convert.ToInt32(answer.Message.Chat.Id))].mode = 2;
                    await botClient.SendTextMessageAsync(answer.Message.Chat.Id, "Пожалуйста, введите ключ для метода Хилла.");
                }
                else
                {
                    Users[User_.GetIndex(Convert.ToInt32(answer.Message.Chat.Id))].mode = 5;
                    await botClient.SendTextMessageAsync(answer.Message.Chat.Id, "Пожалуйста, введите открытый текст для шифрования методов Хилла.");
                }
            }
            else if (answer.Data == "decHill")
            {
                var keyVigenere = Users[User_.GetIndex(Convert.ToInt32(answer.Message.Chat.Id))].keyHill;
                if (keyVigenere.Length == 0)
                {
                    Users[User_.GetIndex(Convert.ToInt32(answer.Message.Chat.Id))].mode = 2;
                    await botClient.SendTextMessageAsync(answer.Message.Chat.Id, "Пожалуйста, введите ключ для метода Хилла.");
                }
                else
                {
                    Users[User_.GetIndex(Convert.ToInt32(answer.Message.Chat.Id))].mode = 6;
                    await botClient.SendTextMessageAsync(answer.Message.Chat.Id, "Пожалуйста, введите закрытый текст для расшифровки методом Хилла.");
                }
            }
            else if (answer.Data == "cipherVigenere")
            {
                InlineKeyboardMarkup inlineKeyboard = new(new[] {
                        new []{ InlineKeyboardButton.WithCallbackData(text: "Ввести ключ", callbackData: "setKeyVigenere") },
                        new []{ InlineKeyboardButton.WithCallbackData(text: "Зашифровать", callbackData: "encVigenere") },
                        new []{ InlineKeyboardButton.WithCallbackData(text: "Расшифровать", callbackData: "decVigenere") } });
                await botClient.SendTextMessageAsync(
                    chatId: answer.Message.Chat.Id,
                    text: "Вы выбрали шифрование методом Виженера.",
                    replyMarkup: inlineKeyboard);
            }
            else if (answer.Data == "cipherHill")
            {
                InlineKeyboardMarkup inlineKeyboard = new(new[] {
                       new []{ InlineKeyboardButton.WithCallbackData(text: "Ввести ключ", callbackData: "setKeyHill") },
                       new []{ InlineKeyboardButton.WithCallbackData(text: "Зашифровать", callbackData: "encHill") },
                       new []{ InlineKeyboardButton.WithCallbackData(text: "Расшифровать", callbackData: "decHill") } });
                await botClient.SendTextMessageAsync(
                    chatId: answer.Message.Chat.Id,
                    text: "Вы выбрали шифрование методом Хилла.",
                    replyMarkup: inlineKeyboard);
            }
            else if (answer.Data == "getKeyVigenere")
            {
                string keyVig = Users[User_.GetIndex(Convert.ToInt32(answer.Message.Chat.Id))].keyVigenere;
                if (keyVig.Length == 0) { keyVig = "Ключ для метода Виженера ещё не записан."; }
                else { keyVig = $"Ключ для метода Виженера:\n" + keyVig; }
                await botClient.SendTextMessageAsync(answer.Message.Chat.Id, keyVig);
            }
            else if (answer.Data == "getKeyHill")
            {
                string keyHill = Users[User_.GetIndex(Convert.ToInt32(answer.Message.Chat.Id))].keyHill;
                if (keyHill.Length == 0) { keyHill = "Ключ для метода Хилла ещё не записан."; }
                else { keyHill = $"Ключ для метода Хилла:\n" + keyHill; }
                await botClient.SendTextMessageAsync(answer.Message.Chat.Id, keyHill);
            }
            else { await botClient.SendTextMessageAsync(answer.Message.Chat.Id, "Похоже я сломался."); }
        }
        else { Console.WriteLine("Неизвестный запрос\n"); }
    }
    static Task Error(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }
}