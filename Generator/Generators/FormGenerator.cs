using Generator.Model;
using System.IO;
using System.Linq;

namespace Generator.Generators
{
    internal class FormGenerator : Generator
    {
        public static void WriteForm(string path, Form form)
        {
            using (var file = new StreamWriter(path))
            {
                WriteHeaders(file, form);
                WriteProps(file, form);
                WriteClassStart(file, form);
                WriteRender(file, form);
                WriteClassEnd(file, form);
            }
        }

        private static void WriteClassEnd(TextWriter writer, Form form)
        {
            writer.WriteLine("}");

            if (form.NeedsStyles)
            {
                writer.WriteLine();
                writer.WriteLine($"const wrappedComponent = decorate({form.FormName});");
                writer.WriteLine();
                writer.WriteLine($"export {{ wrappedComponent as {form.FormName} }};");
            }
        }

        private static void WriteClassStart(TextWriter writer, Form form)
        {
            if (form.NeedsStyles)
            {
                writer.WriteLine("const style = createStyles({");
                if (form.Fields.Any(f => f.Type == FieldTypes.Date || f.Type == FieldTypes.Time))
                {
                    writer.WriteLine("  dateTimeField: {");
                    writer.WriteLine("    minWidth: 190");
                    writer.WriteLine("  },");
                }
                if (form.Fields.Any(f => f.Type == FieldTypes.LongText))
                {
                    writer.WriteLine("  multiLineFieldInput: {");
                    writer.WriteLine("    maxHeight: \"100%\"");
                    writer.WriteLine("  },");
                    writer.WriteLine("  multiLineFieldInputRoot: {");
                    writer.WriteLine("    maxHeight: 250");
                    writer.WriteLine("  },");
                    writer.WriteLine("  multiLineFieldRoot: {");
                    writer.WriteLine("    width: \"100%\"");
                    writer.WriteLine("  },");
                }
                writer.WriteLine("});");
                writer.WriteLine();
                writer.WriteLine("const decorate = withStyles(style);");
                writer.WriteLine();
            }

            writer.WriteLine($"@inject(\"{form.Store}\")");
            writer.WriteLine("@observer");

            if (form.NeedsStyles)
            {
                writer.WriteLine($"class {form.FormName} extends React.Component<");
            }
            else
            {
                writer.WriteLine($"export class {form.FormName} extends React.Component<");
            }

            writer.WriteLine($"  {form.PropsInterface}");
            if (form.NeedsStyles)
            {
                writer.WriteLine($"  & WithStyles<");

                if (form.Fields.Any(f => f.Type == FieldTypes.Date || f.Type == FieldTypes.Time))
                    writer.WriteLine($"    | \"dateTimeField\"");
                if (form.Fields.Any(f => f.Type == FieldTypes.LongText))
                {
                    writer.WriteLine($"    | \"multiLineFieldInput\"");
                    writer.WriteLine($"    | \"multiLineFieldInputRoot\"");
                    writer.WriteLine($"    | \"multiLineFieldRoot\"");
                }
                writer.WriteLine($"  >");
            }
            writer.WriteLine("> {");
        }

        private static void WriteHeaders(TextWriter writer, Form form)
        {
            var directivesPath = $"{PathToSourceRoot(form.PathLevel)}directives";
            var storeInterfacesPath = $"{PathToSourceRoot(form.PathLevel)}store/interfaces";

            writer.WriteLine("import Grid from \"@material-ui/core/Grid\";");
            writer.WriteLine("import TextField from \"@material-ui/core/TextField\";");
            writer.WriteLine("import * as React from \"react\";");

            if (form.Fields.Any(f => f.Type == FieldTypes.Bool))
                writer.WriteLine($"import {{ InputCheck }} from \"{directivesPath}/input-check\";");

            writer.WriteLine($"import {{ {form.StoreData} }} from \"{storeInterfacesPath}/{form.KebabName}\";");
            writer.WriteLine($"import {{ IStore }} from \"{storeInterfacesPath}/store\";");
            writer.WriteLine("import { inject, observer } from \"mobx-react\";");
            writer.WriteLine($"import {{ FormDialog }} from \"{directivesPath}/form-dialog\";");
            if (form.NeedsStyles)
            {
                writer.WriteLine("import withStyles, { WithStyles } from \"@material-ui/core/styles/withStyles\";");
                writer.WriteLine("import createStyles from \"@material-ui/core/styles/createStyles\";");
            }
            if (form.Fields.Any(f => f.Type == FieldTypes.Date))
                writer.WriteLine("import format from \"date-fns/format\";");

            writer.WriteLine();
        }

        private static void WriteProps(TextWriter writer, Form form)
        {
            writer.WriteLine($"interface {form.PropsInterface} {{");
            writer.WriteLine($"  {form.Store}?: IStore<{form.StoreData}>;");
            writer.WriteLine($"}}");
            writer.WriteLine();
        }

        private static void WriteRender(TextWriter writer, Form form)
        {
            writer.WriteLine("  render() {");
            writer.WriteLine("    const {");
            writer.WriteLine($"      {form.Store},");
            if (form.NeedsStyles)
                writer.WriteLine($"      classes,");
            writer.WriteLine("    } = this.props;");
            writer.WriteLine("    const {");
            writer.WriteLine("      data,");
            writer.WriteLine("      handleInput,");
            writer.WriteLine("      validation,");
            writer.WriteLine("      show,");
            writer.WriteLine("      hide,");
            writer.WriteLine("      submit");
            writer.WriteLine($"    }} = {form.Store}!;");
            writer.WriteLine();
            writer.WriteLine("    if (!data) {");
            writer.WriteLine("      return null;");
            writer.WriteLine("    }");
            writer.WriteLine();
            writer.WriteLine("    const {");

            foreach (var field in form.Fields.OrderBy(f => f.Name))
            {
                writer.WriteLine($"      {field.Name},");
            }

            writer.WriteLine("    } = data;");
            writer.WriteLine();
            writer.WriteLine("    return (");
            writer.WriteLine("      <FormDialog");
            writer.WriteLine($"        formTitle=\"{form.Title}\"");
            writer.WriteLine("        submitButtonText=\"Save\"");
            writer.WriteLine("        onSubmit={submit}");
            writer.WriteLine("        show={show}");
            writer.WriteLine("        onHide={hide}");
            writer.WriteLine("        enableSubmit={validation.allValid}");
            writer.WriteLine("      >");
            writer.WriteLine("        <Grid container spacing={8}>");

            foreach (var field in form.Fields)
            {
                writer.WriteLine("          <Grid item xs={12}>");

                if (field.Type == FieldTypes.Bool)
                    writer.WriteLine("            <InputCheck");
                else
                    writer.WriteLine("            <TextField");

                writer.WriteLine($"              id=\"{field.Name}\"");
                writer.WriteLine($"              name=\"{field.Name}\"");

                if (field.Type == FieldTypes.Bool)
                    writer.WriteLine($"              checked={{{field.Name}}}");
                else if (field.Type == FieldTypes.Date)
                    writer.WriteLine($"              value={{format({field.Name}, \"YYYY-MM-DD\")}}");
                else
                    writer.WriteLine($"              value={{{field.Name}}}");

                writer.WriteLine($"              label=\"{field.Label}\"");
                writer.WriteLine("              onChange={handleInput}");

                if (!string.IsNullOrWhiteSpace(field.Disabled))
                    writer.WriteLine($"              disabled={{{field.Disabled}}}");

                if (field.Type != FieldTypes.Bool)
                    writer.WriteLine($"              error={{validation.{field.Name} === false}}");

                switch (field.Type)
                {
                    case FieldTypes.Tel:
                        writer.WriteLine("              type=\"tel\"");
                        break;

                    case FieldTypes.Number:
                        writer.WriteLine("              type=\"number\"");
                        break;

                    case FieldTypes.Date:
                        writer.WriteLine("              type=\"date\"");
                        break;

                    case FieldTypes.Time:
                        writer.WriteLine("              type=\"time\"");
                        break;

                    case FieldTypes.LongText:
                        writer.WriteLine("              multiline");
                        break;
                }

                if (field.Required)
                    writer.WriteLine($"              required");

                if (field.Type == FieldTypes.Date || field.Type == FieldTypes.Time)
                    writer.WriteLine("              className={classes.dateTimeField}");

                if (field.Type == FieldTypes.LongText)
                {
                    writer.WriteLine("              classes={{");
                    writer.WriteLine("                root: classes.multiLineFieldRoot");
                    writer.WriteLine("              }}");
                    writer.WriteLine("              InputProps={{");
                    writer.WriteLine("                classes: {");
                    writer.WriteLine("                  inputMultiline: classes.multiLineFieldInput,");
                    writer.WriteLine("                  root: classes.multiLineFieldInputRoot");
                    writer.WriteLine("                }");
                    writer.WriteLine("              }}");
                }

                writer.WriteLine("            />");
                writer.WriteLine("          </Grid>");
            }

            writer.WriteLine("        </Grid>");
            writer.WriteLine("      </FormDialog>");
            writer.WriteLine("    );");
            writer.WriteLine("  }");
        }
    }
}