﻿using Sean.Core.DependencyInjection;
using System;

namespace Example.Topshelf.JobService
{
    public static class DIManager
    {
        //public static IDIResolve Container => _container;

        private static IDIContainer _container;

        public static void Register(Action<IDIRegister> action)
        {
            if (_container == null)
            {
                var builder = new ContainerBuilder();
                var container = builder.Build();
                _container = container;
            }

            action?.Invoke(_container);
        }

        public static TService Resolve<TService>()
        {
            return _container.Resolve<TService>();
        }
        public static TService Resolve<TService>(Type serviceType)
        {
            return (TService)_container.Resolve(serviceType);
        }
    }
}
