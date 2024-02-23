using System;

namespace AdaskoTheBeAsT.Owin.SecureExceptions;

public interface ITransformsCollection
{
    ITransform? FindTransform(Exception exception);
}
