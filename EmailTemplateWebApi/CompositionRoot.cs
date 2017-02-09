using LightInject;
using XServices.Common.Contracts.Services;


namespace XServices.EmailTemplateWebApi
{
    public class CompositionRoot : ICompositionRoot
    {
        void ICompositionRoot.Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IEmailTemplateService,EmailTemplateService>();
        }
    }
}