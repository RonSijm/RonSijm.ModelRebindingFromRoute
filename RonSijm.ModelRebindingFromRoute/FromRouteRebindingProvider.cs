using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace RonSijm.ModelRebindingFromRoute
{
    /// <summary>
    /// From Path Binder Provider
    /// Rebinds path parameters back into the model, for the purpose of all other layers receiving all input data into 1 model, which is easier to work with.
    /// <example>
    /// Decorate controller action with:
    ///     [HttpPost("[controller]/{objectHash}/action")]
    ///     public async string Action([FromBody] ActionObject action)
    /// ///////
    /// /// Note that {objectHash} defined path is not actually used in the input parameters.
    /// ///////
    ///   public class ActionObject
    ///   {
    ///    [FromRoute]
    ///    public string Action { get; set; }
    /// }
    /// ///////
    /// /// Note that Action property should be named the same as the one defined in the path, so this binder knows how to bind it.
    /// ///////
    /// </example>
    /// </summary>
    public class FromRouteRebindingProvider : IModelBinder
    {
        private readonly IModelBinder _innerModelBinder;

        public FromRouteRebindingProvider(IModelBinder innerModelBinder)
        {
            _innerModelBinder = innerModelBinder;
        }


        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            await _innerModelBinder.BindModelAsync(bindingContext);

            // The FromPathBinderProvider expects another IModelBinder to initially bind it,
            // And then this binder will rebind path parameters in the correct place
            if (!bindingContext.Result.IsModelSet)
            {
                return;
            }

            foreach (var modelMetadataData in bindingContext.ModelMetadata.Properties)
            {
                if (!(modelMetadataData is DefaultModelMetadata defaultData))
                {
                    continue;
                }

                var attributes = defaultData.Attributes.PropertyAttributes;
                var isIdentity = attributes.Any(x => x is FromRouteAttribute);

                if (!isIdentity)
                {
                    continue;
                }

                var pathValue = bindingContext.HttpContext.Request.RouteValues.FirstOrDefault(x => x.Key.Equals(defaultData.Name, StringComparison.InvariantCultureIgnoreCase)).Value;
                if (pathValue != null)
                {
                    modelMetadataData.PropertySetter.Invoke(bindingContext.Result.Model, pathValue);
                }
            }
        }
    }
}