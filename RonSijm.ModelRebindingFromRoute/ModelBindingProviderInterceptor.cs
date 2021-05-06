using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace RonSijm.ModelRebindingFromRoute
{
    public class ModelBindingProviderInterceptor : IModelBinderProvider
    {
        private readonly IEnumerable<IModelBinderProvider> _modelBinderProviders;

        public ModelBindingProviderInterceptor(IEnumerable<IModelBinderProvider> modelBinderProviders)
        {
            _modelBinderProviders = modelBinderProviders;
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            var defaultModelBinderProvider = _modelBinderProviders.Where(modelBinderProvider => !(modelBinderProvider is ModelBindingProviderInterceptor))
                                                                  .Select(modelBinderProvider => modelBinderProvider.GetBinder(context))
                                                                  .FirstOrDefault(binder => binder != null);

            var hasIdentity = ModelHasFromRouteAttribute(context);

            if(!hasIdentity)
            {
                return defaultModelBinderProvider;
            }

            var interceptedModelBinderProvider = new FromRouteRebindingProvider(defaultModelBinderProvider);
            return interceptedModelBinderProvider;
        }

        /// <summary>
        /// Method that validates whether or not any of the properties as an <see cref="FromRouteAttribute"/> attribute.
        /// If the object does not have a <see cref="FromRouteAttribute"/>, the default IModelBinder is returned.
        /// <remarks>
        /// The overarching IModelBinderFactory caches modelbinders, so we don't want to create FromPathBinderProvider when we don't have to.
        /// This process is only evaluated the first time the IModelBinderFactory requests a model binder.
        /// </remarks>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool ModelHasFromRouteAttribute(ModelBinderProviderContext context)
        {
            return context.Metadata.Properties.OfType<DefaultModelMetadata>().Select(defaultData => defaultData.Attributes.PropertyAttributes).Select(attributes => attributes.Any(x => x is FromRouteAttribute)).Any(isIdentity => isIdentity);
        }
    }
}
