
using Encrypt.Enums;

namespace TokenGeneration.Entidades
{
    public class CtorTokenGeneratorRequest
    {
        public required string SecretKey { get; set; }
        public required EncryptAlgorithm Algorithm { get; set; }
        public required EncryptedMode CypherMode { get; set; }
        public string Rol { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}
