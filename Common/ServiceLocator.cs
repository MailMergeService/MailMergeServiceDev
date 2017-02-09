using LightInject;
using System;

namespace XServices.Common
{
    public static class XServiceLocator
    {
        private static ServiceContainer currentContainer;

        public static ServiceContainer Current
        {
            get
            {
                if (!IsLocationProviderSet)
                    throw new InvalidOperationException("Service location provider not set");

                return currentContainer;
            }
        }

        public static void SetLocatorProvider(ServiceContainer newContainer)
        {
            currentContainer = newContainer;
        }

        public static bool IsLocationProviderSet
        {
            get { return currentContainer != null; }
        }
    }
}