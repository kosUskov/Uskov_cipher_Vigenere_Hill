namespace Uskov_cipher_Vigenere_Hill
{
    internal class HelpFunction
    {
        public static List<char> alphavit = "Ё∆АБВГ✓ДЕЖ№ЗИЙК√ЛМНОПР!С\"Т•™#У$Ф%Х&Ц'Ч(Ш)Щ*Ъ+Ы,Ь-Э.Ю/Я0а1 б2в3г4д5е6ж7з8и9й:к;л<м=н>о?п@рAсBтCуDфEхFцGчHшIщJъKыLьMэNюOяPQёRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~¢£¥©€®°¶π×÷".ToList();
        public static bool CheckErrorSymbol(string text)
        {
            foreach (char ch in text.ToCharArray())
            {
                if (alphavit.IndexOf(ch) == -1) { return true; }
            }
            return false;
        }
        public static int ConvertSymbolToCode(char ch)
        {
            return alphavit.IndexOf(ch);
        }
        public static string SetDataToSQL(string text)
        {
            string newText = "";
            foreach (char ch in text)
            {
                int number = ConvertSymbolToCode(ch);
                if (number < 10) { newText += $"00{number}"; }
                else if (number < 100) { newText += $"0{number}"; }
                else { newText += number.ToString(); }
            }
            return newText;
        }
        public static string ConvertCodeToSymbol(int code)
        {
            return alphavit[Convert.ToInt32(code)].ToString();
        }
        public static string GetDataFromSql(string text)
        {
            string newText = "";
            for (int i = 0; i < text.Length; i += 3)
            {
                newText += ConvertCodeToSymbol(Convert.ToInt32(text.Substring(i, 3)));
            }
            return newText;
        }
        static int[,] DelRowCol(int[,] m, int _i, int _j)
        {//удаляет строку и столбец для вычисления минора
            int k = m.Length;
            int[,] newM1 = new int[m.GetLength(0) - 1, m.GetLength(1)];
            int x = 0;
            for (int i = 0; i < m.GetLength(0); i++)
            {
                if (i == _i)
                {
                    _i = -1;
                    continue;
                }

                for (int j = 0; j < m.GetLength(1); j++)
                {
                    newM1[x, j] = m[i, j];
                }
                x++;
            }
            int[,] newM2 = new int[newM1.GetLength(0), newM1.GetLength(1) - 1];
            for (int i = 0; i < newM1.GetLength(0); i++)
            {
                x = 0;
                for (int j = 0; j < newM1.GetLength(1); j++)
                {
                    if (j == _j) continue;
                    newM2[i, x] = newM1[i, j];
                    x++;
                }
            }
            return newM2;
        }
        static decimal GetAlgDop(int[,] m, int _i, int _j)
        {//вычисление матр алг дополнений
            decimal algDop = 0;
            int k = m.GetLength(0) - 1;
            int[,] _m = DelRowCol(m, _i, _j);
            if (k == 1) { algDop = _m[0, 0]; }
            else
            {
                for (int j = 0; j < k; j++)
                {
                    algDop += _m[0, j] * GetAlgDop(_m, 0, j);
                }
            }
            int tmpI = (_i + _j) % 2;
            if (tmpI == 1) { return -algDop; }
            else { return algDop; }
        }
        static decimal[] GetDetMatrix(int[,] m)
        {//вычисление детерминанта
            decimal[] det_ = new decimal[m.GetLength(0)];
            //decimal det = 0;
            for (int i = 0; i < m.GetLength(0); i++)
            {
                det_[i] = m[0, i] * GetAlgDop(m, 0, i);
                //det += m[0, i] * GetAlgDop(m, 0, i);
            }
            return det_;
        }
        public static int GetOstat(decimal delim, int delit)
        {//правильный остаток
            if (delit != 0)
            {
                while ((delim < 0) || (delim >= delit))
                {
                    delim %= delit;
                    if (delim < 0) { delim += delit; }
                }
                return Convert.ToInt32(delim);
            }
            else { return -1; }
        }
        public static int GetOstat(int delim, int delit)
        {//правильный остаток
            if (delit != 0)
            {
                while ((delim < 0) || (delim >= delit))
                {
                    delim %= delit;
                    if (delim < 0) { delim += delit; }
                }
                return Convert.ToInt32(delim);
            }
            else { return -1; }
        }
        public static int[,] MultiplyMatrixMod(int[,] leftMatrix, int[,] rightMatrix, int mod)
        {//перемножение матриц
            int row = leftMatrix.GetLength(0);
            int med = rightMatrix.GetLength(0);
            int col = rightMatrix.GetLength(1);
            int[,] result = new int[row, col];
            //for (int i = 0; i < row; i++) { result[i].resize(col); }
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    long sum = 0;
                    for (int k = 0; k < med; k++)
                    {
                        sum += leftMatrix[i, k] * rightMatrix[k, j];
                    }
                    result[i, j] = GetOstat(sum, mod);
                }
            }
            return result;
        }
        static int GetRevDet(int[,] m, int mod)
        {//обратный дет в кольце
            decimal[] det_ = GetDetMatrix(m);
            for (int i = 1; i < mod; i++)
            {
                decimal det = 0;
                for (int j = 0; j < det_.GetLength(0); j++)
                {
                    det += GetOstat(i * det_[j], mod);
                }
                if (det % mod == 1) return i;
            }
                return -1;
        }
        public static int[,] GetRevMatrix(int[,] m, int mod)
        {//обратная матрица по модулю
            int revDet = GetRevDet(m, mod);
            int k = m.GetLength(0);
            int[,] result = new int[k, k];
            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < k; j++)
                {
                    decimal tmpResult = revDet * GetAlgDop(m, i, j);
                    result[j, i] = GetOstat(tmpResult, mod);
                }
            }
            return result;
        }
        public static bool CheckDetMatrix(int[,] matrix, int mod)
        {//истина, если обр дет нет
            int k = GetRevDet(matrix, mod);
            if (k == -1) { return true; }
            return false;
        }
        public static int GetSqrt(int x)
        {
            int i;
            for (i = 1; i * i <= x; i++)
            {
                if (i * i <= x) { if (i * i == x) { return i; } }
            }
            return 10 + i;
        }
    }
}
