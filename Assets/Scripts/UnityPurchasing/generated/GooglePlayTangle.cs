// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("1iFxv0pVaKinz9peGxp72SeFIAfIEFjHMNTx4bISdb2CXxN+59jtOFf5S65xNV0Iqp8Qcbs7NgvPHJzZfwmgay8fU0otzLfSq8XJEYu98vmaSwKHBBZQhbKZ9BO3dBvKwkUqQvBCweLwzcbJ6kaIRjfNwcHBxcDD6l+qjr8aNamAjaYDhwtCiBObJA6brj/yZTiYXUGjaAxNF/DiLeQ+coKfUuG1iZlI1k6cukAacM7bBJg5H8JPZAB6qSzsv4N9k+Q3uyDnEcxCwc/A8ELBysJCwcHARaT7BkWHxJzJhV3YwbimsAFHjCAhS6OWRjDSSTrqEYIVPwBUW68OcHIbGY5Wggwo27xQKsUDqf2/rhF/1VoJOXuG3qQZ/DAsyKdtq8LDwcDB");
        private static int[] order = new int[] { 5,13,6,9,9,5,11,8,8,10,13,13,13,13,14 };
        private static int key = 192;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
