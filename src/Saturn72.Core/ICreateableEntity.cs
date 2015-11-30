using System;

namespace Saturn72.Core
{
    public interface ICreateableEntity
    {
        DateTime CreatedOnUtc { get; set; }
    }
}