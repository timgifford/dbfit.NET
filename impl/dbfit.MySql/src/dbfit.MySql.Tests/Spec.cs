using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

namespace dbfit.test {
    
    [TestFixture]
    public abstract class Spec {
        private MockRepository mocks;

        [SetUp]
        public virtual void SetUp()
        {
            mocks = new MockRepository();
        }

        protected virtual TType Mock<TType>() {
            return mocks.DynamicMock<TType>();
        }

        protected virtual TType Stub<TType>() {
            return mocks.Stub<TType>();
        }

        protected virtual TType PartialMock<TType>(params object[] argumentsForConstructor) where TType : class {
            return mocks.PartialMock<TType>(argumentsForConstructor);
        }

        protected virtual IDisposable Record {
            get { return mocks.Record(); }
        }

        protected virtual IDisposable Playback {
            get { return mocks.Playback(); }
        }

        protected virtual void Verify(object mock) {
            mocks.Verify(mock);
        }

        protected virtual void VerifyAll() {
            mocks.VerifyAll();
        }
    }
}
