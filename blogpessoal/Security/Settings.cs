namespace blogpessoal.Security
{
    public class Settings
    {
        private static string secret = "f11427f4dc07c067a9288bcc6b2c01ccd2110c3714bd6cafecce88f9ed637fe8";

        public static string Secret { get => secret; set => secret = value; } 


    }
}
