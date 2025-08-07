namespace Encrypt.Interface
{
    public interface IHash 
    {
        byte[] HashKey(string key);
    }
}
