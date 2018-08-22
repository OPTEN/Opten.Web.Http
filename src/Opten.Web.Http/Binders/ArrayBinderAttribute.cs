using System;
using System.ComponentModel;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;

namespace Opten.Web.Http.Binders
{
	/// <summary>
	/// Converts an url like /1,2,3/ to an Array
	/// </summary>
	/// <seealso cref="ModelBinderAttribute" />
	/// <seealso cref="IModelBinder" />
	public class ArrayBinderAttribute : ModelBinderAttribute, IModelBinder
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="ArrayBinderAttribute"/> class.
		/// </summary>
		public ArrayBinderAttribute() : base(typeof(ArrayBinderAttribute)) { }

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

				if (string.IsNullOrWhiteSpace(attempt) == false)
				{
					Type type = bindingContext.ModelType.GetElementType();
					TypeConverter converter = TypeDescriptor.GetConverter(type);

					object[] values = Array.ConvertAll(attempt.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
						o => { return converter.ConvertFromString(o?.Trim()); });

					Array array = Array.CreateInstance(type, values.Length);

					values.CopyTo(array, 0);

					bindingContext.Model = array;
				}

				return true;
			}

			return false;
		}

	}
}
