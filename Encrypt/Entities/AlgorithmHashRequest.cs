
using Encrypt.Enums;

namespace Encrypt.Entities
{
    public class AlgorithmHashRequest
    {
        public EncryptAlgorithm Algorithm { get; set; }
        public EncryptedMode CypherMode { get; set; }
    }
}
