namespace Take5Demo.Helper
{
    public class StringHelper
    {
        public static string ToBase64(byte[] bytes, bool addbase64Prefix = false)
        {
            var base64Str = Convert.ToBase64String(bytes, 0, bytes.Length);
            if (addbase64Prefix)
            {
                base64Str = $"data:image/png;base64,{base64Str}";
            }
            return base64Str;
        }
    }
}
