namespace Pantrymony.Extensions;

public static class ThrowingExtensions
{
    public static T ThrowIf<T>(this T token, Predicate<T> isTrueFor, Exception ex) where T:class?
    {
        if (isTrueFor(token))
            throw ex;
        else
        {
            return token;
        }
    }
    public static T ThrowIfNull<T>(this T? token, Exception ex) where T:class
    {
        if (token is null)
            throw ex;
        else
        {
            return token;
        }
    }
}