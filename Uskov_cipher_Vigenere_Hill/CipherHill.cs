namespace Uskov_cipher_Vigenere_Hill
{
    internal class CipherHill
    { 
        public static string SetKey(long ID, string key)
        {
            string answer = "Спасибо, ключ для метода Хилла записал:\n";
            if (key.Length > 49)
            {
                key = key.Substring(0, 49);
                answer = "Ключ оказался слишком длинным (больше 49 символов), я записал:\n";
            }
            else if (HelpFunction.GetSqrt(key.Length) > 10)
            {
                int start = HelpFunction.alphavit.IndexOf(key.ToCharArray()[key.Length - 1]) + 1;
                int tmp1 = key.Length;
                int tmp2 = HelpFunction.GetSqrt(tmp1) - 10;
                for (int i = 0; i < tmp2*tmp2 - tmp1; i++)
                {
                    if (start > HelpFunction.alphavit.Count) { start = 0; }
                    key += HelpFunction.alphavit[start].ToString();
                    start++;
                }
                answer = "Ключ оказался неправильной длины (должна быть 4, 9, ...), я записал:\n";
            }
            int poryadok = Convert.ToInt32(Math.Sqrt(key.Length));
            int[,] matrixKey = new int[poryadok, poryadok];
            int step = 0;
            while (true)
            {
                string newKey = key.Substring(key.Length - step, step) + key.Substring(0, key.Length - step);
                for (int i = 0, help = 0, index = 0; i < poryadok; i++)
                {
                    for (int j = 0; j < poryadok; j++)
                    {
                        matrixKey[i, help * (poryadok - 1) + Convert.ToInt32(Math.Pow(-1, help)) * j] = HelpFunction.ConvertSymbolToCode(newKey[index]);
                        index++;
                    }
                    if (help == 0) { help = 1; }
                    else { help = 1; }
                }
                if (HelpFunction.CheckDetMatrix(matrixKey, HelpFunction.alphavit.Count))
                {
                    step++;
                    if (step == poryadok * poryadok)
                    {
                        answer = "Увы ключ не подходит. Я не смогу расшифровать методом Хилла.\nВведите другой ключ.";
                        break;
                    }
                    else { continue; }
                }
                else { Program.Users[User_.GetIndex(ID)].keyHill = newKey; break; }
            }
            return answer;
        }
        public static string EncText(long ID,  string text)
        {
            string key = Program.Users[User_.GetIndex(ID)].keyHill;
            int poryadok = Convert.ToInt32(Math.Sqrt(key.Length));
            int[,] matrixKey = new int[poryadok, poryadok];
            for (int i = 0, help = 0, index = 0; i < poryadok; i++)
            {
                for (int j = 0; j < poryadok; j++)
                {
                    matrixKey[i, help * (poryadok - 1) + Convert.ToInt32(Math.Pow(-1, help)) * j] = HelpFunction.ConvertSymbolToCode(key[index]);
                    index++;
                }
                if (help == 0) { help = 1; }
                else { help = 1; }
            }
            if (text.Length % poryadok != 0)
            {
                text += new string(' ', poryadok - (text.Length % poryadok));
            }
            int[,] matrixText = new int[text.Length / poryadok, poryadok];
            for (int i = 0, index = 0; i < text.Length / poryadok; i++)
            {
                for (int j = 0; j < poryadok; j++)
                {
                    matrixText[i, j] = HelpFunction.ConvertSymbolToCode(text[index]);
                    index++;
                }
            }
            int[,] matrixNewText = HelpFunction.MultiplyMatrixMod(matrixText, matrixKey, HelpFunction.alphavit.Count);
            string newText = "";
            for (int i = 0; i < matrixNewText.GetLength(0); i++)
            {
                for (int j = 0; j < matrixNewText.GetLength(1); j++)
                {
                    newText += HelpFunction.ConvertCodeToSymbol(matrixNewText[i, j]);
                }
            }
            return newText;
        }
        public static string DecText(long ID, string text)
        {
            string key = Program.Users[User_.GetIndex(ID)].keyHill;
            int poryadok = Convert.ToInt32(Math.Sqrt(key.Length));
            int[,] matrixKey = new int[poryadok, poryadok];
            for (int i = 0, help = 0, index = 0; i < poryadok; i++)
            {
                for (int j = 0; j < poryadok; j++)
                {
                    matrixKey[i, help * (poryadok - 1) + Convert.ToInt32(Math.Pow(-1, help)) * j] = HelpFunction.ConvertSymbolToCode(key[index]);
                    index++;
                }
                if (help == 0) { help = 1; }
                else { help = 1; }
            }
            matrixKey = HelpFunction.GetRevMatrix(matrixKey, HelpFunction.alphavit.Count);
            if (text.Length % poryadok != 0)
            {
                text += new string(' ', poryadok - (text.Length % poryadok));
            }
            int[,] matrixText = new int[text.Length / poryadok, poryadok];
            for (int i = 0, index = 0; i < text.Length / poryadok; i++)
            {
                for (int j = 0; j < poryadok; j++)
                {
                    matrixText[i, j] = HelpFunction.ConvertSymbolToCode(text[index]);
                    index++;
                }
            }
            int[,] matrixNewText = HelpFunction.MultiplyMatrixMod(matrixText, matrixKey, HelpFunction.alphavit.Count);
            string newText = "";
            for (int i = 0; i < matrixNewText.GetLength(0); i++)
            {
                for (int j = 0; j < matrixNewText.GetLength(1); j++)
                {
                    newText += HelpFunction.ConvertCodeToSymbol(matrixNewText[i, j]);
                }
            }
            return newText;
        }
    }
}
