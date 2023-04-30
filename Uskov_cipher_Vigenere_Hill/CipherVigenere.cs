namespace Uskov_cipher_Vigenere_Hill
{
    internal class CipherVigenere
    {
        public static string SetKey(long ID, string key)
        {
            string answer = "Ключ для метода Виженера записал.";
            //if (key.Length > ) в телеграме длина сообщения не более 2^12 символов, БД поддреживает 2^16
            Program.Users[User_.GetIndex(ID)].keyVigenere = key;
            return answer;
        }
        static int GetCodeFirstRow(char ch)
        {
            int number = HelpFunction.ConvertSymbolToCode(ch);
            if (number + 1 + HelpFunction.alphavit.Count % 10 > HelpFunction.alphavit.Count) { return HelpFunction.alphavit.Count - 1 - (number % 10); }
            return (number / 10 + 1) * 10 - 1 - number % 10;
        }
        static string GetSymbolFirstRow(int number)
        {
            int index;
            if (number + 1 + HelpFunction.alphavit.Count % 10 > HelpFunction.alphavit.Count) { index = HelpFunction.alphavit.Count - 1 - (number % 10); }
            else { index = (number / 10 + 1) * 10 - 1 - number % 10; }
            return HelpFunction.ConvertCodeToSymbol(index);
        }
        public static string EncText(long ID, string text)
        {
            string key = Program.Users[User_.GetIndex(ID)].keyVigenere;
            string newText = "";
            for (int i = 0; i < text.Length; i++)
            {
                newText += HelpFunction.ConvertCodeToSymbol((GetCodeFirstRow(text[i]) + HelpFunction.ConvertSymbolToCode(key[i % key.Length])) % HelpFunction.alphavit.Count);
            }
            return newText;
        }
        public static string DecText(long ID, string text)
        {
            string key = Program.Users[User_.GetIndex(ID)].keyVigenere;
            string newText = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (i == 168)
                {
                    int t = 0;
                }
                newText += GetSymbolFirstRow(HelpFunction.GetOstat(HelpFunction.ConvertSymbolToCode(text[i]) - HelpFunction.ConvertSymbolToCode(key[i % key.Length]), HelpFunction.alphavit.Count));
            }
            return newText;
        }
    }
}
