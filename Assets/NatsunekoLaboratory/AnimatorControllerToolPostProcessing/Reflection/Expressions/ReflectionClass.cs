// -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the License Zero Parity 7.0.0 (see LICENSE-PARITY file) and MIT (contributions, see LICENSE-MIT file) with exception License Zero Patron 1.0.0 (see LICENSE-PATRON file)
// -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Object = UnityEngine.Object;

namespace NatsunekoLaboratory.AnimatorControllerToolPostProcessing.Reflection.Expressions
{
    public class ReflectionClass
    {
        private static readonly Hashtable Caches = new Hashtable();
        private readonly object _instance;
        private readonly Type _type;

        protected Object RawInstance => _instance as Object;

        protected ReflectionClass(object instance, Type type)
        {
            _instance = instance;
            _type = type;

            if (Caches[_type] == null)
                Caches[_type] = new Cache();
        }

        public bool IsAlive()
        {
            return RawInstance != null && RawInstance;
        }

        protected TResult InvokeMethod<TResult>(string name, BindingFlags flags, params object[] parameters)
        {
            var methods = ((Cache) Caches[_type]).Methods;
            methods.TryGetValue(name, out var cache);

            if (cache != null)
                return (TResult) cache.Invoke(_instance, parameters);

            var mi = _instance.GetType().GetMethod(name, flags | BindingFlags.Instance);
            if (mi == null)
                throw new InvalidOperationException($"Method {name} is not found in this instance");

            methods.Add(name, CreateMethodAccessor(mi));
            return (TResult) methods[name].Invoke(_instance, parameters);
        }

        protected TResult InvokeField<TResult>(string name, BindingFlags flags)
        {
            var members = ((Cache) Caches[_type]).Members;
            members.TryGetValue(name, out var cache);

            if (cache != null)
                return (TResult) cache.Invoke(_instance);

            var fi = _instance.GetType().GetField(name, flags | BindingFlags.Instance);
            if (fi == null)
                throw new InvalidOperationException($"Field {name} is not found in this instance");

            members.Add(name, CreateFieldAccessor(fi));
            return (TResult) members[name].Invoke(_instance);
        }

        protected TResult InvokeProperty<TResult>(string name, BindingFlags flags)
        {
            var members = ((Cache) Caches[_type]).Members;
            members.TryGetValue(name, out var cache);

            if (cache != null)
                return (TResult) cache.Invoke(_instance);

            var fi = _instance.GetType().GetProperty(name, flags | BindingFlags.Instance);
            if (fi == null)
                throw new InvalidOperationException($"Property {name} is not found in this instance");

            members.Add(name, CreatePropertyAccessor(fi));
            return (TResult) members[name].Invoke(_instance);
        }

        private Func<object, object[], object> CreateMethodAccessor(MethodInfo mi)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var args = Expression.Parameter(typeof(object[]), "args");
            var body = mi.GetParameters().Length == 0
                ? Expression.Call(Expression.Convert(instance, _type), mi)
                : Expression.Call(Expression.Convert(instance, _type), mi, mi.GetParameters().Select((w, i) => Expression.Convert(Expression.ArrayIndex(args, Expression.Constant(i)), w.ParameterType)).Cast<Expression>().ToArray());

            return Expression.Lambda<Func<object, object[], object>>(Expression.Convert(body, typeof(object)), instance, args).Compile();
        }

        private Func<object, object> CreateFieldAccessor(FieldInfo fi)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var body = Expression.Field(Expression.Convert(instance, _type), fi);

            return Expression.Lambda<Func<object, object>>(Expression.Convert(body, typeof(object)), instance).Compile();
        }

        private Func<object, object> CreatePropertyAccessor(PropertyInfo pi)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var body = Expression.Property(Expression.Convert(instance, _type), pi);

            return Expression.Lambda<Func<object, object>>(Expression.Convert(body, typeof(object)), instance).Compile();
        }

        private class Cache
        {
            public readonly Dictionary<string, Func<object, object>> Members = new Dictionary<string, Func<object, object>>();
            public readonly Dictionary<string, Func<object, object[], object>> Methods = new Dictionary<string, Func<object, object[], object>>();
        }
    }
}