using Opten.Common.Parsers;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;

namespace Opten.Web.Http.Binders
{
	/// <summary>
	/// Converts an url like /2017-01-07/ to a Date.
	/// </summary>
	/// <seealso cref="System.Web.Http.ModelBinding.ModelBinderAttribute" />
	/// <seealso cref="System.Web.Http.ModelBinding.IModelBinder" />
	public class UrlDateBinderAttribute : ModelBinderAttribute, IModelBinder
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="UrlDateBinderAttribute"/> class.
		/// </summary>
		public UrlDateBinderAttribute() : base(typeof(UrlDateBinderAttribute)) { }

		/// <summary>
		/// Binds the model to a value by using the specified controller context and binding context.
		/// </summary>
		/// <param name="actionContext">The action context.</param>
		/// <param name="bindingContext">The binding context.</param>
		/// <returns>
		/// true if model binding is successful; otherwise, false.
		/// </returns>
		public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
		{
			string key = bindingContext.ModelName;

			ValueProviderResult value = bindingContext.ValueProvider.GetValue(key);

			if (value != null)
			{
				string attempt = value.AttemptedValue;

				if (string.IsNullOrWhiteSpace(attempt) == false &&
					attempt.Split(new[] { '-' }).Length == 3) // 2017-01-07
				{
					// DateBinder (not DateTime) --> so only Date is parsed
					bindingContext.Model = DateTimeParser.ParseUrlString(attempt); //TODO: UTC?!
				}

				return true;
			}

			return false;
		}

	}
}
