using System;
using NSubstituteAutoMocker;

namespace HWI.Integrations.Tests
{
    /// <summary>
    /// Base class that adds auto mocking/faking to NUnit.
    /// </summary>
    /// <typeparam name="TSubject">
    /// The subject of the specification. This is the type that is created by the specification for you.
    /// </typeparam>
    public abstract class WithSubject<TSubject> where TSubject : class
    {
        private readonly NSubstituteAutoMocker<TSubject> _mocker = new NSubstituteAutoMocker<TSubject>();

        private TSubject _subject;

        /// <summary>
        /// Gives access to the subject under specification. On first access
        /// the spec tries to create an instance of the subject type by itself.
        /// </summary>
        protected TSubject Subject => _subject ?? (_subject = _mocker.ClassUnderTest);

        /// <summary>
        /// Creates a fake of the type specified by <typeparamref name="TInterfaceType" />.
        /// This method reuses existing instances. If an instance of <typeparamref name="TInterfaceType" />
        /// was already requested it's returned here. (You can say this is kind of a singleton behavior)
        /// Besides that, you can obtain a reference to injected instances/fakes with this method.
        /// </summary>
        /// <typeparam name="TInterfaceType">The type to create a fake for. (Should be an interface or an abstract class)</typeparam>
        /// <returns>
        /// An instance implementing <typeparamref name="TInterfaceType" />.
        /// </returns>
        protected TInterfaceType The<TInterfaceType>() where TInterfaceType : class
        {
            return _mocker.Get<TInterfaceType>();
        }

        protected WithSubject()
        {
            // ReSharper disable VirtualMemberCallInConstructor
            Setup();
            Because();
        }

        protected virtual void Setup()
        {
        }

        protected abstract void Because();
    }
}