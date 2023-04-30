namespace Uskov_cipher_Vigenere_Hill
{
    internal class User_
    {
        public long ID { get; set; }
        public int mode { get; set; }
        public string keyVigenere { get; set; }
        public string keyHill { get; set; }
        public static void RegisterUser(long ID)
        {
            Program.Users.Add(new User_
            {
                ID = ID,
                mode = 0,
                keyVigenere = "",
                keyHill = ""
            });
        }
        public static int GetIndex(long ID)
        {
            if (Program.Users.FindIndex(user => user.ID == ID) == -1)
            {
                RegisterUser(ID);
            }
            return Program.Users.FindIndex(user => user.ID == ID);
        }
    }
}
