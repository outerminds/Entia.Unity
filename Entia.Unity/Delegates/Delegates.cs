using System;
using System.Collections;
using System.Collections.Generic;
using Entia.Core;
using Entia.Delegables;
using Entia.Delegates;

namespace Entia.Modules
{
    public sealed class Delegates : IModule, IEnumerable<IDelegate>
    {
        readonly TypeMap<UnityEngine.Object, IDelegate> _defaults = new TypeMap<UnityEngine.Object, IDelegate>();
        readonly TypeMap<UnityEngine.Object, IDelegate> _delegates = new TypeMap<UnityEngine.Object, IDelegate>();

        public IDelegate Default(Type type) =>
            _defaults.Default(type, typeof(IDelegable<>), null, state =>
            {
                if (state.Is<UnityEngine.Object>())
                {
                    try { return Activator.CreateInstance(typeof(Unity<>).MakeGenericType(state)) as IDelegate; }
                    catch { }
                }
                return new Default();
            });
        public IDelegate Get(Type type) => _delegates.TryGet(type, out var @delegate, true, false) ? @delegate : Default(type);
        public IDelegate Get<T>() where T : UnityEngine.Object => _delegates.TryGet<T>(out var @delegate, true, false) ? @delegate : Default(typeof(T));
        public bool Has<T>() where T : UnityEngine.Object => _delegates.Has<T>(true, false);
        public bool Has(Type type) => _delegates.Has(type, true, false);
        public bool Set<T>(Delegate<T> @delegate) where T : UnityEngine.Object => _delegates.Set<T>(@delegate);
        public bool Set(Type type, IDelegate @delegate) => _delegates.Set(type, @delegate);
        public bool Remove<T>() where T : UnityEngine.Object => _delegates.Remove<T>(false, false);
        public bool Remove(Type type) => _delegates.Remove(type, false, false);
        public bool Clear() => _defaults.Clear() | _delegates.Clear();

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        public IEnumerator<IDelegate> GetEnumerator() => _delegates.Values.Concat(_defaults.Values).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}