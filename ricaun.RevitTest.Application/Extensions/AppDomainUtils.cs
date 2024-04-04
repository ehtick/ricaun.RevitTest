using System;
using System.Reflection;

namespace ricaun.RevitTest.Application.Extensions
{
    internal static class AppDomainUtils
    {
        /// <summary>
        /// AssemblyResolveDisposable
        /// Remove all the AssemblyResolve until Dispose, the current AssemblyResolve is not Removed and stays with the priority.
        /// </summary>
        /// <returns></returns>
        public static AppDomainExtension.DelegatesDisposable AssemblyResolveDisposable()
        {
#if DEBUG
            var resolveEventHandler = AppDomain.CurrentDomain.GetResolveEventHandler();
            Console.WriteLine($"Debug Domain >> {AppDomain.CurrentDomain.FriendlyName}");
            Console.WriteLine($"Debug Assembly >> {typeof(AppDomainUtils).Assembly.FullName}");
            Console.WriteLine($"Debug Location >> {typeof(AppDomainUtils).Assembly.Location}");
            Console.WriteLine($"Debug GetResolveEventHandler >> {resolveEventHandler}");
            if (resolveEventHandler is not null)
            {
                Console.WriteLine($"Debug GetInvocationList >> {resolveEventHandler.GetInvocationList().Length}");
            }
#endif
            return AppDomain.CurrentDomain.GetAssemblyResolveDisposable().NotRemoveDelegatesAfterDispose();
        }
    }

    internal static class AppDomainExtension
    {
        /// <summary>
        /// Get private <see cref="AppDomain.AssemblyResolve"/> EventHandler
        /// </summary>
        /// <param name="appDomain"></param>
        /// <returns></returns>
        public static ResolveEventHandler GetResolveEventHandler(this AppDomain appDomain)
        {
#if NETFRAMEWORK
            var fieldName = "_AssemblyResolve";

            var fieldInfo = appDomain.GetType()
                .GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            return fieldInfo?.GetValue(appDomain) as ResolveEventHandler;
#elif NET
            var fieldName = "AssemblyResolve";

            var fieldInfo = typeof(System.Runtime.Loader.AssemblyLoadContext)
                .GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);

            return fieldInfo?.GetValue(null) as ResolveEventHandler;
#endif
        }

        /// <summary>
        /// Get private <see cref="AppDomain.AssemblyResolve"/> Delegate[]
        /// </summary>
        /// <param name="appDomain"></param>
        /// <returns></returns>
        public static Delegate[] GetResolveEventHandlerList(this AppDomain appDomain)
        {
            return appDomain.GetResolveEventHandler()?.GetInvocationList() ?? new Delegate[] { };
        }

        /// <summary>
        /// Create new <see cref="DelegatesDisposable"/> for <see cref="AppDomain.AssemblyResolve"/>
        /// </summary>
        /// <param name="appDomain"></param>
        /// <returns></returns>
        public static DelegatesDisposable GetAssemblyResolveDisposable(this AppDomain appDomain)
        {
            var resolveEventHandler = appDomain.GetResolveEventHandler();
            return new DelegatesDisposable(
                value => appDomain.AssemblyResolve += (ResolveEventHandler)value,
                value => appDomain.AssemblyResolve -= (ResolveEventHandler)value,
                () => appDomain.GetResolveEventHandlerList()
                );
        }

        /// <summary>
        /// DelegatesDisposable
        /// </summary>
        internal class DelegatesDisposable : IDisposable
        {
            private Delegate[] Delegates;
            private readonly Action<Delegate> add;
            private readonly Action<Delegate> remove;
            private readonly Func<Delegate[]> get;

            private bool _AddCurrentDelegates = false;
            private bool _RemoveCurrentDelegates = true;

            /// <summary>
            /// AddDelegatesAfterDispose 
            /// </summary>
            /// <returns></returns>
            public DelegatesDisposable AddDelegatesAfterDispose()
            {
                _AddCurrentDelegates = true;
                return this;
            }

            /// <summary>
            /// NotRemoveDelegatesAfterDispose 
            /// </summary>
            /// <returns></returns>
            public DelegatesDisposable NotRemoveDelegatesAfterDispose()
            {
                _RemoveCurrentDelegates = false;
                return this;
            }

            /// <summary>
            /// DelegatesDisposable
            /// </summary>
            /// <param name="add"></param>
            /// <param name="remove"></param>
            /// <param name="get"></param>
            public DelegatesDisposable(Action<Delegate> add, Action<Delegate> remove, Func<Delegate[]> get)
            {
                this.add = add;
                this.remove = remove;
                this.get = get;
                Delegates = get();

                Initialize();
            }

            private void Initialize()
            {
                foreach (var d in Delegates)
                {
                    remove(d);
                }
            }

            private Delegate[] RemoveCurrentDelegates()
            {
                var delegates = get();
                foreach (var d in delegates)
                {
                    remove(d);
                }
                return delegates;
            }

            /// <summary>
            /// Dispose
            /// </summary>
            public void Dispose()
            {
                var delegates = new Delegate[] { };

                if (_RemoveCurrentDelegates)
                    delegates = RemoveCurrentDelegates();

                foreach (var d in Delegates)
                {
                    add(d);
                }

                if (_AddCurrentDelegates)
                {
                    foreach (var d in delegates) add(d);
                }
            }
        }
    }
}
