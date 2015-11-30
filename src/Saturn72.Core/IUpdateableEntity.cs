using System;

namespace Saturn72.Core
{
    public interface IUpdateableEntity : ICreateableEntity
    {
        DateTime UpdatedOnUtc { get; set; }
    }
}