using System.Text;

namespace HelloSourceGenerator
{
    public abstract class TemplateBase
    {
        protected StringBuilder GenerationEnvironment { get; } = new StringBuilder(512);

        public abstract string TransformText();

        public void Write(string text)
        {
            GenerationEnvironment.Append(text);
        }

        public class ToStringInstanceHelper
        {
            public string ToStringWithCulture(string value)
            {
                return value;
            }
        }

        public ToStringInstanceHelper ToStringHelper { get; } = new ToStringInstanceHelper();

    }
}