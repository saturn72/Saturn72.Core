using System;
using System.Collections.Generic;
using Automation.Core.Validation;

namespace Automation.Core.Activity
{
    /// <summary>
    ///     Represents system request
    /// </summary>
    public class ActivityRequest : RequestBase<object>
    {
    }


    /// <summary>
    ///     Represents system request
    /// </summary>
    public class VoidActivityRequest : RequestBase<VoidResult>
    {
    }

    /// <summary>
    ///     Represents System Request object
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class RequestBase<TResult> : RequestBase
    {
        public override Type ReturnType
        {
            get { return typeof (TResult); }
        }

        public override Func<object> Do
        {
            get { return () => (TResult) base.Do(); }
            set { base.Do = value; }
        }
    }

    public abstract class RequestBase
    {
        public abstract Type ReturnType { get; }
        public IEnumerable<string> ValidationKeys { get; set; }
        public RequestBase ParentRequest { get; set; }

        public IDictionary<object, object> Parameters
        {
            get { return _parameters ?? (_parameters = new Dictionary<object, object>()); }
            protected set { _parameters = value; }
        }

        public virtual Func<object> Do
        {
            get
            {
                return () =>
                {
                    if (!WasDoInvoked)
                    {
                        _doResult = _do();
                        WasDoInvoked = true;
                    }
                    return _doResult;
                };
            }
            set { _do = value; }
        }

        public virtual object ExpectedValue { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }

        public IEnumerable<Action> PreDoActions
        {
            get { return _preDoActions ?? (_preDoActions = new List<Action>()); }
            set { _preDoActions = value; }
        }

        public IEnumerable<Action> PostDoActions
        {
            get { return _postDoActions ?? (_postDoActions = new List<Action>()); }
            set { _postDoActions = value; }
        }

        public IEnumerable<Func<ValidationPoint>> ValidationPoints { get; set; }
        public dynamic ValidationData { get; set; }

        public virtual RequestBase Setup { get; set; }
        public virtual RequestBase TearDown { get; set; }

        #region Fields

        private IDictionary<object, object> _parameters;
        private IEnumerable<Action> _postDoActions;
        private IEnumerable<Action> _preDoActions;
        protected bool WasDoInvoked { get; private set; }
        

        private object _doResult;
        private Func<object> _do;

        #endregion
    }
}