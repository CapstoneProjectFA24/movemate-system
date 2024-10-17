using Fluid;

namespace MoveMate.Service.Helper
{
    public static class FluidHepler
    {
        public static string AddParam(this string source, object model)
        {
            string result;

            var parser = new FluidParser();

            if (parser.TryParse(source, out var template, out var error))
            {
                var context = new TemplateContext(model);

                result = template.Render(context);
            }
            else
            {
                return source;
            }

            return result;
        }
    }
}