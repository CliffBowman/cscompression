namespace System.Text;

public static class StringBuilderExtensionMethods
{
    public static void TrimTail(this StringBuilder buffer, int maxSize)
    {
        if (buffer.Length > maxSize)
            buffer.Remove(0, buffer.Length - maxSize);
    }
}
