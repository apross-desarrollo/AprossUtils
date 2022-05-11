using System;
using System.Text;

namespace AprossUtils
{
    public class DateTimeUtils
    {
        public static int GetAge(DateTime birth)
        {
            DateTime today = DateTime.Now;
            int age = today.Year - birth.Year;
            if (birth.Date > today.AddYears(-age)) age--;
            return age;
        }

        public static DateTime? ToLocalDate(DateTime? dt)
        {
            if (dt == null) return null;
            return DateTime.SpecifyKind((DateTime)dt, DateTimeKind.Local);
        }
        public static DateTime ToLocalDate(DateTime dt)
        {
            return DateTime.SpecifyKind((DateTime)dt, DateTimeKind.Local);
        }
    }

    public class Encrypt
    {
        public static string Sha1(string input)
        {
            var sha1 = new System.Security.Cryptography.SHA1Managed();
            var plaintextBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = sha1.ComputeHash(plaintextBytes);

            var sb = new StringBuilder();
            foreach (var hashByte in hashBytes)
            {
                sb.AppendFormat("{0:x2}", hashByte);
            }

            return sb.ToString();
        }
    }
    public class Validators
    {
        public static bool CuiltValidate(string cuit)
        {
            if (string.IsNullOrEmpty(cuit)) return false;
            if (cuit.Length != 13 && cuit.Length != 11) return false;
            bool rv = false;
            int verificador;
            int resultado = 0;
            string cuit_nro = cuit.Replace("-", string.Empty);
            string codes = "6789456789";
            long cuit_long = 0;
            if (long.TryParse(cuit_nro, out cuit_long))
            {
                verificador = int.Parse(cuit_nro[cuit_nro.Length - 1].ToString());
                int x = 0;
                while (x < 10)
                {
                    int digitoValidador = int.Parse(codes.Substring((x), 1));
                    int digito = int.Parse(cuit_nro.Substring((x), 1));
                    int digitoValidacion = digitoValidador * digito;
                    resultado += digitoValidacion;
                    x++;
                }
                resultado = resultado % 11;
                rv = (resultado == verificador);
            }
            return rv;
        }
    }
}