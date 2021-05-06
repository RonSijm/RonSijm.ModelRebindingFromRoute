# Model Rebinding From Route

This library provides an `IModelBinderProvider` and an `IModelBinder` which makes it easy to have REST-like paths, but still conserve all input data into a singular model.

## Problem Example

Consider you have an application with books, and books have pages. You want to POST text to a specific page.

Traditionally your API controller would look like this:

        [HttpPost("[controller]/book/{bookId}/page/{pageId}")]
        public string Echo(string bookId, string pageId, PageContent content)
	
This is fine, however if your project gets bigger and bigger, you'll probably start to add custom `IActionFilters` which will take input objects for processing purposes.

This is where the issue starts and the Model Rebinding comes in.

## Problem solution

With this library, you are able to write API controllers like so:

        [HttpPost("[controller]/book/{bookId}/page/{pageId}")]
        public string Echo(BookRequestModel bookRequestModel)
		
The BookRequestModel has the following implementation:

        public class BookRequestModel
        {
            [FromRoute]
            public string BookId { get; set; }
    
            [FromRoute]
            public string PageId { get; set; }
        }
		
Note that the property names must match the path names, that's how the model rebinder will know where to rebind which route value, and the properties must have an [FromRoute] attribute. Mainly for performance reasons, so that if the model is a traditional model without any rebinding properties, the default binders are used.

## Usage

Install the package:

    install-package RonSijm.ModelRebindingFromRoute
	
Wire the model rebinder into your startup like so:

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new ModelBindingProviderInterceptor(options.ModelBinderProviders));
            });
        }
		
Note: It's important to wire it by using Insert(0) - this way the rebinder is used with as first attempt. Otherwise a default binder will pick this up, and if it's capable of binding, the rebinder is skipped entirely.